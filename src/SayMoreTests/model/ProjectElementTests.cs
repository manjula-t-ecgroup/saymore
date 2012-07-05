using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Palaso.IO;
using Palaso.Reporting;
using Palaso.TestUtilities;
using SayMore;
using SayMore.Model;
using SayMore.Model.Files;

namespace SayMoreTests.Model
{
	[TestFixture]
	public class ProjectElementTests
	{
		private TemporaryFolder _parentFolder;

		[SetUp]
		public void Setup()
		{
			ErrorReport.IsOkToInteractWithUser = false;
			_parentFolder = new TemporaryFolder("ProjectElementTest");
		}

		[TearDown]
		public void TearDown()
		{
			_parentFolder.Dispose();
			_parentFolder = null;
		}

		[Test]
		[Category("SkipOnTeamCity")]
		public void Save_NewlyCreated_CreatesMetaDataFile()
		{
			using (var person = CreatePerson())
			{
				person.Save();
				Assert.IsTrue(File.Exists(_parentFolder.Combine("xyz", "xyz.person")));
				Assert.AreEqual(1, Directory.GetFiles(_parentFolder.Combine("xyz")).Length);
			}
		}

		private Session CreateSession()
		{
			return CreateSession(_parentFolder.Path, "xyz");
		}

		public static Session CreateSession(string parentFolderPath, string name)
		{
			return new Session(parentFolderPath, name, null, new SessionFileType(() => null, () => null),
				MakeComponent, new FileSerializer(null), (w, x, y, z) =>
					new ProjectElementComponentFile(w, x, y, z, FieldUpdater.CreateMinimalFieldUpdaterForTests(null)),
					ApplicationContainer.ComponentRoles, null);
		}

		private Person CreatePerson()
		{
			return CreatePerson(_parentFolder.Path, "xyz");
		}

		public static Person CreatePerson(string parentFolderPath, string name)
		{
			return new Person(parentFolderPath, name, null, new PersonFileType(() => null),
				MakeComponent, new FileSerializer(null), (w, x, y, z) =>
					new ProjectElementComponentFile(w, x, y, z, FieldUpdater.CreateMinimalFieldUpdaterForTests(null)),
				new ComponentRole[] {});
		}

		private static ComponentFile MakeComponent(ProjectElement parentElement, string pathtoannotatedfile)
		{
			return ComponentFile.CreateMinimalComponentFileForTests(parentElement, pathtoannotatedfile);
		}

		public string SetValue(Person person, string key, string value)
		{
			string failureMessage;
			var suceeded = person.MetaDataFile.SetStringValue(key, value, out failureMessage);
			if (!string.IsNullOrEmpty(failureMessage))
			{
				throw new ApplicationException(failureMessage);
			}

			return suceeded;
		}

		[Test]
		[Category("SkipOnTeamCity")]
		public void Load_AfterSaveOfFactoryField_PreservesId()
		{
			using (var person = CreatePerson())
			{
				SetValue(person, "primaryLanguage", "Esperanto");
				person.Save();
				using (var person2 = CreatePerson())
				{
					person2.Load();
					Assert.AreEqual("Esperanto", person2.MetaDataFile.GetStringValue("primaryLanguage", "Swahili"));
					Assert.AreEqual(1, Directory.GetFiles(_parentFolder.Combine("xyz")).Length);
				}
			}
		}

		[Test]
		[Category("SkipOnTeamCity")]
		public void Load_AfterSave_ChangesUnknownIdToCustomId()
		{
			using (var person = CreatePerson())
			{
				SetValue(person, "color", "red");
				person.Save();
				using (var person2 = CreatePerson())
				{
					person2.Load();
					Assert.AreEqual("red", person2.MetaDataFile.GetStringValue("custom_color", "blue"));
					Assert.AreEqual(1, Directory.GetFiles(_parentFolder.Combine("xyz")).Length);
				}
			}
		}

		[Test][Category("SkipOnTeamCity")]
		public void GetComponentFiles_AfterCreation_GivesASingleFile()
		{
			using (var person = CreatePerson())
			{
				IEnumerable<ComponentFile> componentFiles = person.GetComponentFiles();
				Assert.AreEqual(1, componentFiles.Count());
				Assert.AreEqual("xyz.person", componentFiles.First().FileName);
			}
		}

		[Test][Category("SkipOnTeamCity")]
		public void GetComponentFiles_SomeFiles_GivesThem()
		{
			using (var person = CreatePerson())
			{
				File.WriteAllText(_parentFolder.Combine("xyz", "test.txt"), @"hello");
				Assert.AreEqual(2, person.GetComponentFiles().Count());
			}
		}

		[Test][Category("SkipOnTeamCity")]
		public void RemoveInvalidFilesFromProspectiveFilesToAdd_AllValid_RemovesNone()
		{
			using (var fileToAdd1 = new TempFile())
			using (var fileToAdd2 = new TempFile())
			{
				using (var person = CreatePerson())
				{
					var list = person.RemoveInvalidFilesFromProspectiveFilesToAdd(
						new[] { fileToAdd1.Path, fileToAdd2.Path });

					Assert.That(list.Count(), Is.EqualTo(2));

					Assert.That(list.Contains(fileToAdd1.Path), Is.True);
					Assert.That(list.Contains(fileToAdd2.Path), Is.True);
				}
			}
		}

		[Test][Category("SkipOnTeamCity")]
		public void RemoveInvalidFilesFromProspectiveFilesToAdd_NullInput_ReturnsEmptyList()
		{
			using (var person = CreatePerson())
			{
				var list = person.RemoveInvalidFilesFromProspectiveFilesToAdd(null);
				Assert.That(list.Count(), Is.EqualTo(0));
			}
		}

		[Test][Category("SkipOnTeamCity")]
		public void RemoveInvalidFilesFromProspectiveFilesToAdd_EmptyListInput_ReturnsEmptyList()
		{
			using (var person = CreatePerson())
			{
				var list = person.RemoveInvalidFilesFromProspectiveFilesToAdd(new string[] { });
				Assert.That(list.Count(), Is.EqualTo(0));
			}
		}

		[Test][Category("SkipOnTeamCity")]
		public void RemoveInvalidFilesFromProspectiveFilesToAdd_SomeInvalid_RemovesThoseSome()
		{
			var invalidEndings = new StringCollection();
			invalidEndings.AddRange(new[] { ".aaa", ".bbb" });

			// This is a little brittle, but the generated property doesn't have a setter.
			SayMore.Properties.Settings.Default["ComponentFileEndingsNotAllowed"] = invalidEndings;

			using (var fileToAdd = new TempFile())
			{
				using (var person = CreatePerson())
				{
					var list = person.RemoveInvalidFilesFromProspectiveFilesToAdd(
						new[] { "stupidFile.aaa", fileToAdd.Path, "reallyStupidFile.bbb" });

					Assert.That(list.Count(), Is.EqualTo(1));
					Assert.That(list.Contains(fileToAdd.Path), Is.True);
				}
			}
		}

		[Test]
		[Category("SkipOnTeamCity")]
		public void AddComponentFile_SomeFile_AddsIt()
		{
			using (var person = CreatePerson())
			{
				Assert.That(person.GetComponentFiles().Count(), Is.EqualTo(1));

				using (var fileToAdd = new TempFile())
				{
					Assert.That(person.AddComponentFile(fileToAdd.Path), Is.True);
					var componentFiles = person.GetComponentFiles();
					Assert.That(componentFiles.Count(), Is.EqualTo(2));
					Assert.That(componentFiles.Select(x => x.FileName).Contains(Path.GetFileName(fileToAdd.Path)), Is.True);
				}
			}
		}

		[Test]
		[Category("SkipOnTeamCity")]
		public void AddComponentFiles_SomeFiles_AddsThem()
		{
			using (var person = CreatePerson())
			{
				Assert.That(person.GetComponentFiles().Count(), Is.EqualTo(1));

				using (var fileToAdd1 = new TempFile())
				using (var fileToAdd2 = new TempFile())
				{
					Assert.That(person.AddComponentFiles(new[] { fileToAdd1.Path, fileToAdd2.Path }), Is.True);
					var componentFiles = person.GetComponentFiles();
					Assert.That(componentFiles.Count(), Is.EqualTo(3));
					Assert.That(componentFiles.Select(x => x.FileName).Contains(Path.GetFileName(fileToAdd1.Path)), Is.True);
					Assert.That(componentFiles.Select(x => x.FileName).Contains(Path.GetFileName(fileToAdd2.Path)), Is.True);
				}
			}
		}

		[Test]
		[Category("SkipOnTeamCity")]
		public void AddComponentFile_FileAlreadyExistsInDest_DoesNotAdd()
		{
			using (var person = CreatePerson())
			{
				Assert.AreEqual(1, person.GetComponentFiles().Count());

				using (var fileToAdd = new TempFile())
				{
					var fileName = Path.GetFileName(fileToAdd.Path);
					person.RefreshComponentFiles();
					File.CreateText(Path.Combine(person.FolderPath, fileName)).Close();
					Assert.AreEqual(2, person.GetComponentFiles().Count());
					Assert.IsFalse(person.AddComponentFile(fileToAdd.Path));
					Assert.AreEqual(2, person.GetComponentFiles().Count());
				}
			}
		}

		[Test]
		[Category("SkipOnTeamCity")]
		public void AddComponentFiles_AtLeastOneFileAlreadyExistsInDest_AddsOneNotOther()
		{
			using (var person = CreatePerson())
			{
				Assert.AreEqual(1, person.GetComponentFiles().Count());

				using (var fileToAdd1 = new TempFile())
				using (var fileToAdd2 = new TempFile())
				{
					var fileName = Path.GetFileName(fileToAdd1.Path);
					File.CreateText(Path.Combine(person.FolderPath, fileName)).Close();
					person.RefreshComponentFiles();
					Assert.AreEqual(2, person.GetComponentFiles().Count());

					Assert.IsTrue(person.AddComponentFiles(new[] { fileToAdd1.Path, fileToAdd2.Path }));

					var componentFiles = person.GetComponentFiles();
					Assert.AreEqual(3, componentFiles.Count());
					Assert.IsTrue(componentFiles.Select(x => x.FileName).Contains(Path.GetFileName(fileToAdd1.Path)));
					Assert.IsTrue(componentFiles.Select(x => x.FileName).Contains(Path.GetFileName(fileToAdd2.Path)));
				}
			}
		}

		[Test]
		[Category("SkipOnTeamCity")]
		public void GetNewDefaultElementName_NoClashOnFirstTry_GivesName()
		{
			using (var person = CreatePerson())
			{
				Assert.AreEqual(person.DefaultElementNamePrefix + " 01", person.GetNewDefaultElementName());
			}
		}

		[Test]
		[Category("SkipOnTeamCity")]
		public void GetNewDefaultElementName_FindsClash_GivesName()
		{
			using (var person = CreatePerson())
			{
				Directory.CreateDirectory(Path.Combine(_parentFolder.Path, person.DefaultElementNamePrefix + " 01"));
				Directory.CreateDirectory(Path.Combine(_parentFolder.Path, person.DefaultElementNamePrefix + " 02"));
				Assert.AreEqual(person.DefaultElementNamePrefix + " 03", person.GetNewDefaultElementName());
			}
		}

		[Test]
		[Category("SkipOnTeamCity")]
		public void GetShowAsNormalComponentFile_IsElanPrefFile_ReturnsFalse()
		{
			using (var person = CreatePerson())
				Assert.IsFalse(person.GetShowAsNormalComponentFile("carrots.wav.pfsx"));

			using (var person = CreatePerson())
				Assert.IsFalse(person.GetShowAsNormalComponentFile("CARROTS.WAV.PFSX"));
		}

		[Test]
		[Category("SkipOnTeamCity")]
		public void GetShowAsNormalComponentFile_HasAnnotationFileExtButNotSuffix_ReturnsTrue()
		{
			File.OpenWrite(_parentFolder.Combine("peas.mp3")).Close();

			using (var person = CreatePerson())
				Assert.IsTrue(person.GetShowAsNormalComponentFile("peas.mp3.eaf"));

			using (var person = CreatePerson())
				Assert.IsTrue(person.GetShowAsNormalComponentFile("PEAS.MP3.EAF"));
		}

		[Test]
		[Category("SkipOnTeamCity")]
		public void GetShowAsNormalComponentFile_HasAnnotationFileExtAndSuffix_ReturnsFalse()
		{
			File.OpenWrite(_parentFolder.Combine("corn.mp3")).Close();

			using (var person = CreatePerson())
				Assert.IsFalse(person.GetShowAsNormalComponentFile(_parentFolder.Combine("corn.mp3.ANNotations.eaf")));

			using (var person = CreatePerson())
				Assert.IsFalse(person.GetShowAsNormalComponentFile(_parentFolder.Combine("CORN.MP3.annotations.EAF")));
		}

		[Test]
		[Category("SkipOnTeamCity")]
		public void GetShowAsNormalComponentFile_IsMetaFile_ReturnsFalse()
		{
			using (var person = CreatePerson())
				Assert.IsFalse(person.GetShowAsNormalComponentFile("beans.wav.meta"));

			using (var person = CreatePerson())
				Assert.IsFalse(person.GetShowAsNormalComponentFile("BEANS.WAV.META"));
		}

		[Test]
		[Category("SkipOnTeamCity")]
		public void GetShowAsNormalComponentFile_IsThumbsFile_ReturnsFalse()
		{
			using (var person = CreatePerson())
				Assert.IsFalse(person.GetShowAsNormalComponentFile("thumbs.db"));

			using (var person = CreatePerson())
				Assert.IsFalse(person.GetShowAsNormalComponentFile("THUMBS.DB"));
		}

		[Test]
		[Category("SkipOnTeamCity")]
		public void GetShowAsNormalComponentFile_IsPersonFile_ReturnsFalse()
		{
			using (var person = CreatePerson())
				Assert.IsFalse(person.GetShowAsNormalComponentFile("broccoli.person"));

			using (var person = CreatePerson())
				Assert.IsFalse(person.GetShowAsNormalComponentFile("BROCCOLI.PERSON"));
		}

		[Test]
		[Category("SkipOnTeamCity")]
		public void GetShowAsNormalComponentFile_IsEventFile_ReturnsFalse()
		{
			using (var session = CreateSession())
				Assert.IsFalse(session.GetShowAsNormalComponentFile("corn.session"));

			using (var session = CreateSession())
				Assert.IsFalse(session.GetShowAsNormalComponentFile("CORN.SESSION"));
		}

		//[Test]
		//public void GetTotalMediaDuration_HasMediaFiles_ReturnsCorrectDuration()
		//{
		//    var file1 = new Mock<ComponentFile>();
		//    file1.Setup(f => f.DurationString).Returns("00:03:22");

		//    var file2 = new Mock<ComponentFile>();
		//    file2.Setup(f => f.DurationString).Returns(null as string);

		//    var file3 = new Mock<ComponentFile>();
		//    file3.Setup(f => f.DurationString).Returns("00:04:14");

		//    var session = new Mock<Session>();
		//    session.Setup(e => e.GetComponentFiles()).Returns(new[] { file1.Object, file2.Object, file3.Object });

		//    Assert.AreEqual(new TimeSpan(0, 7, 36), session.Object.GetTotalMediaDuration());
		//}
	}
}
