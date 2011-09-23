using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Windows.Forms;
using Localization.UI;
using Palaso.IO;
using Palaso.Media;
using Palaso.Reporting;
using SayMore.Properties;
using SayMore.UI.ElementListScreen;
using SayMore.UI.Overview;
using SilTools;

namespace SayMore.UI.ProjectWindow
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Class for the main window of the application.
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public partial class ProjectWindow : Form
	{
		public delegate ProjectWindow Factory(string projectPath); //autofac uses this

		private readonly string _projectName;
		private readonly IEnumerable<ICommand> _commands;

		public bool UserWantsToOpenADifferentProject { get; set; }

		/// ------------------------------------------------------------------------------------
		private ProjectWindow()
		{
			InitializeComponent();
		}

		/// ------------------------------------------------------------------------------------
		public ProjectWindow(string projectPath, EventsListScreen eventsScreen,
			PersonListScreen personsScreen, ProgressScreen progressScreen,
			IEnumerable<ICommand> commands) : this()
		{
			if (Settings.Default.ProjectWindow == null)
			{
				StartPosition = FormStartPosition.CenterScreen;
				Settings.Default.ProjectWindow = FormSettings.Create(this);
			}

			var asm = Assembly.GetExecutingAssembly();
			Icon = new Icon (asm.GetManifestResourceStream("SayMore.SayMore.ico"));

			_projectName = Path.GetFileNameWithoutExtension(projectPath);
			_commands = commands;

			_viewTabGroup.AddTab("Events", eventsScreen);
			_viewTabGroup.AddTab("People", personsScreen);
			_viewTabGroup.AddTab("Progress", progressScreen);
			//_viewTabGroup.AddTab("Send/Receive", new SendReceiveScreen());

			Program.RegisterForLocalization(_viewTabGroup.Tabs[0], "ProjectWindow.EventsTab");
			Program.RegisterForLocalization(_viewTabGroup.Tabs[1], "ProjectWindow.PeopleTab");
			Program.RegisterForLocalization(_viewTabGroup.Tabs[2], "ProjectWindow.ProgressTab");

			if (eventsScreen.MainMenuItem != null)
			{
				eventsScreen.MainMenuItem.Enabled = false;
				_mainMenuStrip.Items.Insert(_mainMenuStrip.Items.IndexOf(_mainMenuHelp), eventsScreen.MainMenuItem);
			}

			if (personsScreen.MainMenuItem != null)
			{
				personsScreen.MainMenuItem.Enabled = false;
				_mainMenuStrip.Items.Insert(_mainMenuStrip.Items.IndexOf(_mainMenuHelp), personsScreen.MainMenuItem);
			}

			if (progressScreen.MainMenuItem != null)
			{
				progressScreen.MainMenuItem.Enabled = false;
				_mainMenuStrip.Items.Insert(_mainMenuStrip.Items.IndexOf(_mainMenuHelp), progressScreen.MainMenuItem);
			}

			SetWindowText();
			LocalizeItemDlg.StringsLocalized += SetWindowText;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Sets the localized window title texts.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void SetWindowText()
		{
			var ver = Assembly.GetExecutingAssembly().GetName().Version;
			var fmt = Program.GetString("ProjectWindow.WindowTitleWithProject", "{0} - SayMore {1}.{2}.{3}");
			Text = string.Format(fmt, _projectName, ver.Major, ver.Minor, ver.Build);
		}

		/// ------------------------------------------------------------------------------------
		protected override void OnLoad(EventArgs e)
		{
			Settings.Default.ProjectWindow.InitializeForm(this);
			base.OnLoad(e);

			_viewTabGroup.SetActiveView(_viewTabGroup.Tabs[0]);

			if (!MediaInfo.HaveNecessaryComponents)
				new MissingFFmpegPopup().Show();
		}

		/// ------------------------------------------------------------------------------------
		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			if (!_viewTabGroup.IsOKToCloseGroup)
			{
				e.Cancel = true;
				SystemSounds.Beep.Play();
			}

			base.OnFormClosing(e);

			if (!e.Cancel)
				LocalizeItemDlg.StringsLocalized -= SetWindowText;
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Show the welcome dialog to allow the user to choose another project.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void HandleOpenProjectClick(object sender, EventArgs e)
		{
			UserWantsToOpenADifferentProject = true;
			Close();
		}

		/// ------------------------------------------------------------------------------------
		private void HandleExitClick(object sender, EventArgs e)
		{
			UserWantsToOpenADifferentProject = false;
			Close();
		}

		/// ------------------------------------------------------------------------------------
		private void HandleHelpAboutClick(object sender, EventArgs e)
		{
			using (var dlg = new AboutDialog())
				dlg.ShowDialog();
		}
		/// ------------------------------------------------------------------------------------
		private void HandleHelpClick(object sender, EventArgs e)
		{
			//nb: when the file is in our source code, and not in the program directory, windows security will squak and then not show content.
			var path = FileLocator.GetFileDistributedWithApplication(false,"SayMore.chm");
			try
			{
				Process.Start(path);
			}
			catch (Exception)
			{
				//user cancelling a security warning here shouldn't lead to a crash
			}
			UsageReporter.SendNavigationNotice("Help");
		}

		/// ------------------------------------------------------------------------------------
		private void HandleCommandMenuItemClick(object sender, EventArgs e)
		{
			var handler = _commands.First(c => c.Id == (string)((ToolStripMenuItem)sender).Tag);
			handler.Execute();
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Draw a subtle line at the bottom of the main menu to visually separate
		/// the main tabs from the main menu.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void HandleMainMenuPaint(object sender, PaintEventArgs e)
		{
			var clr = Color.FromArgb(30, Color.Black);
			using (var pen = new Pen(clr))
			{
				var rc = _mainMenuStrip.ClientRectangle;
				e.Graphics.DrawLine(pen, 0, rc.Bottom - 1, rc.Right, rc.Bottom - 1);
			}
		}

		/// ------------------------------------------------------------------------------------
		private void HandleViewActivated(ViewTabGroup sender, ViewTab activatedTab)
		{
			var view = activatedTab.View as ISayMoreView;
			if (view == null)
				return;

			if (view.MainMenuItem != null)
				view.MainMenuItem.Enabled = true;

			UsageReporter.SendNavigationNotice(view.NameForUsageReporting);
		}

		/// ------------------------------------------------------------------------------------
		private void HandleViewDeactivated(ViewTabGroup sender, ViewTab deactivatedTab)
		{
			var view = deactivatedTab.View as ISayMoreView;
			if (view != null && view.MainMenuItem != null)
				view.MainMenuItem.Enabled = false;
		}
	}
}
