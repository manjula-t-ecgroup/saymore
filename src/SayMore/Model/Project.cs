using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.Serialization;
using L10NSharp;
using Palaso.Reporting;
using Palaso.UI.WindowsForms;
using SIL.Archiving;
using SIL.Archiving.Generic;
using SIL.Archiving.IMDI;
using SayMore.Properties;
using SayMore.Transcription.Model;
using SayMore.Model.Files;

namespace SayMore.Model
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// A project corresponds to a single folder (with subfolders) on the disk.
	/// In that folder is a file which persists the settings, then a folder of
	/// people, and another of sessions.
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public class Project : IAutoSegmenterSettings, IIMDIArchivable
	{
		private readonly ElementRepository<Session>.Factory _sessionsRepoFactory;
		private readonly SessionFileType _sessionFileType;

		public delegate Project Factory(string desiredOrExistingFilePath);

		public string Name { get; protected set; }

		public Font TranscriptionFont { get; set; }
		public Font FreeTranslationFont { get; set; }

		public int AutoSegmenterMinimumSegmentLengthInMilliseconds { get; set; }
		public int AutoSegmenterMaximumSegmentLengthInMilliseconds { get; set; }
		public int AutoSegmenterPreferrerdPauseLengthInMilliseconds { get; set; }
		public double AutoSegmenterOptimumLengthClampingFactor { get; set; }

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// can be used whether the project exists already, or not
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public Project(string desiredOrExistingSettingsFilePath,
			ElementRepository<Session>.Factory sessionsRepoFactory, SessionFileType sessionFileType)
		{
			_sessionsRepoFactory = sessionsRepoFactory;
			_sessionFileType = sessionFileType;
			SettingsFilePath = desiredOrExistingSettingsFilePath;
			Name = Path.GetFileNameWithoutExtension(desiredOrExistingSettingsFilePath);
			var projectDirectory = Path.GetDirectoryName(desiredOrExistingSettingsFilePath);
			var parentDirectoryPath = Path.GetDirectoryName(projectDirectory);

			if (File.Exists(desiredOrExistingSettingsFilePath))
			{
				RenameEventsToSessions(projectDirectory);
				Load();
			}
			else
			{
				if (!Directory.Exists(parentDirectoryPath))
					Directory.CreateDirectory(parentDirectoryPath);

				if (!Directory.Exists(projectDirectory))
					Directory.CreateDirectory(projectDirectory);

				Save();
			}

			if (TranscriptionFont == null)
				TranscriptionFont = Program.DialogFont;

			if (FreeTranslationFont == null)
				FreeTranslationFont = Program.DialogFont;

			if (AutoSegmenterMinimumSegmentLengthInMilliseconds < Settings.Default.MinimumSegmentLengthInMilliseconds ||
				AutoSegmenterMaximumSegmentLengthInMilliseconds <= 0 ||
				AutoSegmenterMinimumSegmentLengthInMilliseconds >= AutoSegmenterMaximumSegmentLengthInMilliseconds ||
				AutoSegmenterPreferrerdPauseLengthInMilliseconds <= 0 ||
				AutoSegmenterPreferrerdPauseLengthInMilliseconds > AutoSegmenterMaximumSegmentLengthInMilliseconds ||
				AutoSegmenterOptimumLengthClampingFactor <= 0)
			{
				var saveNeeded = AutoSegmenterMinimumSegmentLengthInMilliseconds != 0 || AutoSegmenterMaximumSegmentLengthInMilliseconds != 0 ||
					AutoSegmenterPreferrerdPauseLengthInMilliseconds != 0 || !AutoSegmenterOptimumLengthClampingFactor.Equals(0);

				AutoSegmenterMinimumSegmentLengthInMilliseconds = Settings.Default.DefaultAutoSegmenterMinimumSegmentLengthInMilliseconds;
				AutoSegmenterMaximumSegmentLengthInMilliseconds = Settings.Default.DefaultAutoSegmenterMaximumSegmentLengthInMilliseconds;
				AutoSegmenterPreferrerdPauseLengthInMilliseconds = Settings.Default.DefaultAutoSegmenterPreferrerdPauseLengthInMilliseconds;
				AutoSegmenterOptimumLengthClampingFactor = Settings.Default.DefaultAutoSegmenterOptimumLengthClampingFactor;
				if (saveNeeded)
					Save();
			}
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Renames the project's Events folder to Sessions; rename's all its session files
		/// to have "session" extensions rather than "event" extensions; renames the Event
		/// tags in those files to "Session".
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public void RenameEventsToSessions(string projectDirectory)
		{
			var eventFolder = Directory.GetDirectories(projectDirectory,
				"Events", SearchOption.TopDirectoryOnly).FirstOrDefault();

			if (string.IsNullOrEmpty(eventFolder))
				return;

			var oldFolder = Path.Combine(projectDirectory, "Events");
			var newFolder = Path.Combine(projectDirectory, Session.kFolderName);

			if (Directory.Exists(newFolder))
			{
				if (Directory.EnumerateFiles(newFolder).Any() || Directory.EnumerateDirectories(newFolder).Any())
				{
					var backupSessionsFolder = newFolder + " BAK";
					Directory.Move(newFolder, backupSessionsFolder);
					ErrorReport.NotifyUserOfProblem("In order to upgrade this project, SayMore renamed Events to " + Session.kFolderName +
						". Because a " + Session.kFolderName +
						"folder already existed, SayMore renamed it to " + Path.GetDirectoryName(backupSessionsFolder) + "." + Environment.NewLine +
						"Project path: " + projectDirectory + Environment.NewLine + Environment.NewLine +
						"We recommend you request technical support to decide what to do with the contents of the folder: " + backupSessionsFolder);
				}
				else
					Directory.Delete(newFolder);
			}

			//try
			//{
			Directory.Move(oldFolder, newFolder);
			//}
			//catch (Exception)
			//{
			//    // TODO: should probably be more informative and give the user
			//    // a chance to "unlock" the folder and retry.
			//    //Palaso.Reporting.ErrorReport.ReportFatalException(e);
			//    throw;  //by rethrowing, we allow the higher levels to do what they are supposed to, which is to
			//    //say "sorry, couldn't open that." If we have more info to give here, we could do that via a non-fatal error.
			//}

			foreach (var file in Directory.GetFiles(newFolder, "*.event", SearchOption.AllDirectories))
				RenameEventFileToSessionFile(file);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Renames a single project's session file to an event file, including modifying the
		/// Session tags inside the file.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void RenameEventFileToSessionFile(string oldFile)
		{
			// TODO: Should probably put some error checking in here. Although,
			// I'm not sure what I would do with a failure along the way.
			var evnt = XElement.Load(oldFile);
			var session = new XElement("Session", evnt.Nodes());
			var newFile = Path.ChangeExtension(oldFile, Settings.Default.SessionFileExtension);
			session.Save(newFile);
			File.Delete(oldFile);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes the sessions for the project.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public void InitializeSessions()
		{
			if (!Directory.Exists(SessionsFolder))
				Directory.CreateDirectory(SessionsFolder);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the full path to the folder in which the project's session folders are stored.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[XmlIgnore]
		public string SessionsFolder
		{
			get { return Path.Combine(ProjectFolder, Session.kFolderName); }
		}

		/// ------------------------------------------------------------------------------------
		[XmlIgnore]
		protected string ProjectFolder
		{
			get { return Path.GetDirectoryName(SettingsFilePath); }
		}

		/// ------------------------------------------------------------------------------------
		public void Save()
		{
			var project = new XElement("Project");
			project.Add(new XElement("Iso639Code", Iso639Code));

			if (TranscriptionFont != Program.DialogFont)
				project.Add(new XElement("transcriptionFont", FontHelper.FontToString(TranscriptionFont)));

			if (FreeTranslationFont != Program.DialogFont)
				project.Add(new XElement("freeTranslationFont", FontHelper.FontToString(FreeTranslationFont)));

			if (AutoSegmenterMinimumSegmentLengthInMilliseconds != Settings.Default.DefaultAutoSegmenterMinimumSegmentLengthInMilliseconds ||
				AutoSegmenterMaximumSegmentLengthInMilliseconds != Settings.Default.DefaultAutoSegmenterMaximumSegmentLengthInMilliseconds ||
				AutoSegmenterPreferrerdPauseLengthInMilliseconds != Settings.Default.DefaultAutoSegmenterPreferrerdPauseLengthInMilliseconds ||
				!AutoSegmenterOptimumLengthClampingFactor.Equals(Settings.Default.DefaultAutoSegmenterOptimumLengthClampingFactor))
			{
				var autoSegmenterSettings = new XElement("AutoSegmentersettings");
				project.Add(autoSegmenterSettings);
				autoSegmenterSettings.Add(new XAttribute("minSegmentLength", AutoSegmenterMinimumSegmentLengthInMilliseconds));
				autoSegmenterSettings.Add(new XAttribute("maxSegmentLength", AutoSegmenterMaximumSegmentLengthInMilliseconds));
				autoSegmenterSettings.Add(new XAttribute("preferrerdPauseLength", AutoSegmenterPreferrerdPauseLengthInMilliseconds));
				autoSegmenterSettings.Add(new XAttribute("optimumLengthClampingFactor", AutoSegmenterOptimumLengthClampingFactor));
			}

			project.Save(SettingsFilePath);
		}

		/// ------------------------------------------------------------------------------------
		public void Load()
		{
			var project = XElement.Load(SettingsFilePath);
			var elements = project.Descendants("Iso639Code").ToArray();

			if (elements.Length == 0)
				elements = project.Descendants("IsoCode").ToArray(); //old value when we were called "Sponge"

			Iso639Code = elements.First().Value;

			elements = project.Descendants("transcriptionFont").ToArray();
			if (elements.Length > 0)
				TranscriptionFont = FontHelper.MakeFont(elements.First().Value);

			elements = project.Descendants("freeTranslationFont").ToArray();
			if (elements.Length > 0)
				FreeTranslationFont = FontHelper.MakeFont(elements.First().Value);

			var autoSegmenterSettings = project.Element("AutoSegmentersettings");
			if (autoSegmenterSettings != null)
			{
				AutoSegmenterMinimumSegmentLengthInMilliseconds = GetIntAttributeValue(autoSegmenterSettings,
					"minSegmentLength");
				AutoSegmenterMaximumSegmentLengthInMilliseconds = GetIntAttributeValue(autoSegmenterSettings,
					"maxSegmentLength");
				AutoSegmenterPreferrerdPauseLengthInMilliseconds = GetIntAttributeValue(autoSegmenterSettings,
					"preferrerdPauseLength");
				AutoSegmenterOptimumLengthClampingFactor = GetDoubleAttributeValue(autoSegmenterSettings,
					"optimumLengthClampingFactor");
			}
		}

		/// ------------------------------------------------------------------------------------
		private int GetIntAttributeValue(XElement project, string attribName)
		{
			var attrib = project.Attribute(attribName);
			int val;
			return (attrib != null && Int32.TryParse(attrib.Value, out val)) ? val : default(int);
		}

		/// ------------------------------------------------------------------------------------
		private double GetDoubleAttributeValue(XElement project, string attribName)
		{
			var attrib = project.Attribute(attribName);
			double val;
			return (attrib != null && Double.TryParse(attrib.Value, out val)) ? val : default(double);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// This is, roughly, the "ethnologue code", taken either from 639-2 (2 letters),
		/// or, more often, 639-3 (3 letters)
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public string Iso639Code { get; set; }

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Note: while the folder name will match the settings file name when it is first
		/// created, it needn't remain that way. A user can copy the project folder, rename
		/// it "blah (old)", whatever, and this will still work.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[XmlIgnore]
		public string FolderPath
		{
			get { return Path.GetDirectoryName(SettingsFilePath); }
		}

		[XmlIgnore]
		public string SettingsFilePath { get; set; }

		/// ------------------------------------------------------------------------------------
		/// Gets the SayMore project settings file extension (without the leading period)
		/// ------------------------------------------------------------------------------------
		public static string ProjectSettingsFileExtension
		{
			get { return Settings.Default.ProjectFileExtension.TrimStart('.'); }
		}

		/// ------------------------------------------------------------------------------------
		public static string ComputePathToSettings(string parentFolderPath, string newProjectName)
		{
			var p = Path.Combine(parentFolderPath, newProjectName);
			return Path.Combine(p, newProjectName + "." + ProjectSettingsFileExtension);
		}

		/// ------------------------------------------------------------------------------------
		public static string[] GetAllProjectSettingsFiles(string path)
		{
			return Directory.GetFiles(path, "*." + ProjectSettingsFileExtension, SearchOption.AllDirectories);
		}

		internal IEnumerable<Session> GetAllSessions()
		{
			ElementRepository<Session> sessionRepo = _sessionsRepoFactory(Path.GetDirectoryName(SettingsFilePath), Session.kFolderName, _sessionFileType);
			sessionRepo.RefreshItemList();

			return sessionRepo.AllItems;
		}

		#region Archiving
		/// ------------------------------------------------------------------------------------
		public string ArchiveInfoDetails
		{
			get
			{
				return LocalizationManager.GetString("DialogBoxes.ArchivingDlg.ProjectArchivingInfoDetails",
					"The archive corpus will include all required files and data related to this project.",
					"This sentence is inserted as a parameter in DialogBoxes.ArchivingDlg.IMDIOverviewText");
			}
		}

		/// ------------------------------------------------------------------------------------
		public string Title
		{
			get { return Name; }
		}

		/// ------------------------------------------------------------------------------------
		public string Id
		{
			get { return Name; }
		}

		/// ------------------------------------------------------------------------------------
		public void InitializeModel(IMDIArchivingDlgViewModel model)
		{
			//Set project metadata here.
			model.OverrideDisplayInitialSummary = fileLists => DisplayInitialArchiveSummary(fileLists, model);
			ArchivingHelper.SetIMDIMetadataToArchive(this, model);
		}

		/// ------------------------------------------------------------------------------------
		private void DisplayInitialArchiveSummary(IDictionary<string, Tuple<IEnumerable<string>, string>> fileLists, ArchivingDlgViewModel model)
		{
			if (fileLists.Count > 1)
			{
				model.DisplayMessage(LocalizationManager.GetString("DialogBoxes.ArchivingDlg.PrearchivingStatusMsg1",
					"The following session and contributor files will be added to your archive."), ArchivingDlgViewModel.MessageType.Normal);
			}
			else
			{
				model.DisplayMessage(LocalizationManager.GetString("DialogBoxes.ArchivingDlg.NoContributorsForSessionMsg",
					"There are no contributors for this session."), ArchivingDlgViewModel.MessageType.Warning);

				model.DisplayMessage(LocalizationManager.GetString("DialogBoxes.ArchivingDlg.PrearchivingStatusMsg2",
					"The following session files will be added to your archive."), ArchivingDlgViewModel.MessageType.Progress);
			}

			var fmt = LocalizationManager.GetString("DialogBoxes.ArchivingDlg.ArchivingProgressMsg", "     {0}: {1}",
				"The first parameter is 'Session' or 'Contributor'. The second parameter is the session or contributor name.");

			foreach (var kvp in fileLists)
			{
				var element = (kvp.Key.StartsWith("\n") ?
					LocalizationManager.GetString("DialogBoxes.ArchivingDlg.SessionElementName", "Contributor") :
					LocalizationManager.GetString("DialogBoxes.ArchivingDlg.ContributorElementName", "Session"));

				model.DisplayMessage(string.Format(fmt, element, (kvp.Key.StartsWith("\n") ? kvp.Key.Substring(1) : kvp.Key)),
					ArchivingDlgViewModel.MessageType.Progress);

				foreach (var file in kvp.Value.Item1)
					model.DisplayMessage(Path.GetFileName(file), ArchivingDlgViewModel.MessageType.Bullet);
			}
		}

		/// ------------------------------------------------------------------------------------
		internal void ArchiveProjectUsingIMDI(Form parentForm)
		{
			ArchivingHelper.ArchiveUsingIMDI(this);
		}

		/// ------------------------------------------------------------------------------------
		public void SetFilesToArchive(ArchivingDlgViewModel model)
		{
			Dictionary<string, HashSet<string>> contributorFiles = new Dictionary<string, HashSet<string>>();
			Type archiveType = model.GetType();
			foreach (var session in GetAllSessions())
			{
				model.AddFileGroup(session.Id, session.GetSessionFilesToArchive(archiveType), session.AddingSessionFilesProgressMsg);
				foreach (var person in session.GetParticipantFilesToArchive(model.GetType()))
				{
					if (!contributorFiles.ContainsKey(person.Key))
						contributorFiles.Add(person.Key, new HashSet<string>());

					foreach (var file in person.Value)
						contributorFiles[person.Key].Add(file);
				}

				IArchivingSession s = model.AddSession(session.Id);
				s.Location.Address = session.MetaDataFile.GetStringValue(SessionFileType.kLocationFieldName, string.Empty);
				s.Title = session.Title;
			}

			foreach (var key in contributorFiles.Keys)
			{
				model.AddFileGroup("\n" + key, contributorFiles[key],
					LocalizationManager.GetString("DialogBoxes.ArchivingDlg.AddingContributorFilesToIMDIArchiveProgressMsg",
					"Adding Files for Contributors..."));
			}
		}
		#endregion
	}
}
