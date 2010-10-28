using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Autofac;
using SayMore.Model;
using SayMore.Model.Files;
using SayMore.Model.Files.DataGathering;
using SayMore.UI.ComponentEditors;
using SayMore.UI.ElementListScreen;
using SayMore.UI.ProjectWindow;

namespace SayMore
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// TODO: it might be cleaner to remove this class and just have it all be in method
	/// on applicationContext
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public class ProjectContext : IDisposable
	{
		/// <summary>
		/// Any resources which belong only to this project will be tracked by this,
		/// and disposed of along with this ProjectContext class
		/// </summary>
		private ILifetimeScope _scope;

		public Project Project { get; private set; }
		public ProjectWindow ProjectWindow { get; private set; }

		private AudioVideoDataGatherer _audioVideoDataGatherer;
		private PresetGatherer _presetGatherer;
		private AutoCompleteValueGatherer _autoCompleteValueGatherer;
		private FieldGatherer _fieldGatherer;


		/// ------------------------------------------------------------------------------------
		public ProjectContext(string projectSettingsPath, IContainer parentContainer)
		{
			var rootDirectoryPath = Path.GetDirectoryName(projectSettingsPath);
			BuildSubContainerForThisProject(rootDirectoryPath, parentContainer);

			Project = _scope.Resolve<Func<string, Project>>()(projectSettingsPath);

			var eventRepoFactory = _scope.Resolve<ElementRepository<Event>.Factory>();
			eventRepoFactory(rootDirectoryPath, "Events", _scope.Resolve<EventFileType>());

			var peopleRepoFactory = _scope.Resolve<ElementRepository<Person>.Factory>();
			peopleRepoFactory(rootDirectoryPath, "People", _scope.Resolve<PersonFileType>());

			//Start the background operations
			_audioVideoDataGatherer = _scope.Resolve<AudioVideoDataGatherer>();
			_audioVideoDataGatherer.Start();

			_presetGatherer = _scope.Resolve<PresetGatherer>();
			_presetGatherer.Start();

			_autoCompleteValueGatherer = _scope.Resolve<AutoCompleteValueGatherer>();
			_autoCompleteValueGatherer.Start();

			_fieldGatherer = _scope.Resolve<FieldGatherer>();
			_fieldGatherer.Start();

			ProjectWindow = _scope.Resolve<ProjectWindow.Factory>()(projectSettingsPath);
		}

		/// ------------------------------------------------------------------------------------
		protected void BuildSubContainerForThisProject(string rootDirectoryPath, IContainer parentContainer)
		{
			_scope = parentContainer.BeginLifetimeScope(builder =>
			{
				builder.RegisterType<ElementRepository<Event>>().InstancePerLifetimeScope();
				builder.RegisterType<ElementRepository<Person>>().InstancePerLifetimeScope();
				builder.RegisterType<ElementListViewModel<Event>>().InstancePerLifetimeScope();
				builder.RegisterType<ElementListViewModel<Person>>().InstancePerLifetimeScope();
				builder.RegisterType<AudioVideoDataGatherer>().InstancePerLifetimeScope();
				builder.RegisterType<IEnumerable<FileType>>().InstancePerLifetimeScope();

				builder.RegisterType<EventFileType>().InstancePerLifetimeScope();
				builder.RegisterType<PersonFileType>().InstancePerLifetimeScope();

				//when something needs the list of filetypes, get them from this method
				builder.Register<IEnumerable<FileType>>(c => GetFilesTypes(c)).InstancePerLifetimeScope();

				//these needed to be done later (as delegates) because of the FileTypes dependency
				//there's maybe something I'm doing wrong that requires me to register this twice like this...
				builder.Register<IProvideAudioVideoFileStatistics>(
					c => new AudioVideoDataGatherer(rootDirectoryPath,
						c.Resolve<IEnumerable<FileType>>())).InstancePerLifetimeScope();

				builder.Register<AudioVideoDataGatherer>(c => c.Resolve(typeof(IProvideAudioVideoFileStatistics))
						as AudioVideoDataGatherer).InstancePerLifetimeScope();
				;
				//create a single PresetGatherer and stick it in the container
				//builder.RegisterInstance(parentContainer.Resolve<PresetGatherer.Factory>()(rootDirectoryPath));

				//using the factory gave stack overflow: builder.Register<PresetGatherer>(c => c.Resolve<PresetGatherer.Factory>()(rootDirectoryPath));
				builder.Register<PresetGatherer>(c => new PresetGatherer(rootDirectoryPath,
					c.Resolve<IEnumerable<FileType>>(), c.Resolve<PresetData.Factory>())).InstancePerLifetimeScope();

				builder.Register<AutoCompleteValueGatherer>(
					c => new AutoCompleteValueGatherer(rootDirectoryPath, c.Resolve<IEnumerable<FileType>>(),
						c.Resolve<ComponentFile.Factory>())).InstancePerLifetimeScope();

				builder.Register<FieldGatherer>(
					c => new FieldGatherer(rootDirectoryPath, c.Resolve<IEnumerable<FileType>>(),
						c.Resolve<FileTypeFields.Factory>())).InstancePerLifetimeScope();

				builder.Register<FieldUpdater>(c => new FieldUpdater(c.Resolve<FieldGatherer>())).InstancePerLifetimeScope();

				//make a lazy factory-getter to get around a mysterious circular dependency problem
				//NB: when we move to .net 4, we can remove this and instead use Lazy<Func<PersonBasicEditor.Factory> in the PersonFileType constructor
				builder.Register<Func<PersonBasicEditor.Factory>>(c => () => c.Resolve<PersonBasicEditor.Factory>());
				builder.Register<Func<EventBasicEditor.Factory>>(c => () => c.Resolve<EventBasicEditor.Factory>());
			});
		}

		/// ------------------------------------------------------------------------------------
		private IEnumerable<FileType> GetFilesTypes(IComponentContext context)
		{
			return new List<FileType>(new FileType[] {
									context.Resolve<EventFileType>(),
									context.Resolve<PersonFileType>(),
									context.Resolve<AudioFileType>(),
									context.Resolve<VideoFileType>(),
									context.Resolve<ImageFileType>(),
									context.Resolve<UnknownFileType>(),
									});
		}

		/// ------------------------------------------------------------------------------------
		public void Dispose()
		{
			_audioVideoDataGatherer.Dispose();
			_presetGatherer.Dispose();
			_autoCompleteValueGatherer.Dispose();
			_fieldGatherer.Dispose();
			_scope.Dispose();
			_scope = null;
		}

		/// ------------------------------------------------------------------------------------
		public T ResolveForTests<T>() where T: class
		{
			return _scope.Resolve<T>();
		}
	}
}