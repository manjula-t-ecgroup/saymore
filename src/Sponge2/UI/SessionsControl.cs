using System;
using System.Linq;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using SilUtils;
using Sponge2.Model;

namespace Sponge2.UI
{
	/// ----------------------------------------------------------------------------------------
	public partial class SessionsControl : UserControl
	{
		private readonly SessionsViewModel _model;

		/// ------------------------------------------------------------------------------------
		public SessionsControl(SessionsViewModel presentationModel)
		{
			_model = presentationModel;
			InitializeComponent();

			if (DesignMode)
				return;

			_componentEditorsTabControl.Font = SystemFonts.IconTitleFont;
			_componentGrid.Font = SystemFonts.IconTitleFont;
			LoadSessionList();
		}

		/// ------------------------------------------------------------------------------------
		private void LoadSessionList()
		{
			var sessions = _model.Sessions.Cast<object>().ToList();

			_sessionsListPanel.AddRange(sessions);

			if (sessions.Count > 0)
				_sessionsListPanel.SelectItem(sessions[0], true);
		}

		/// ------------------------------------------------------------------------------------
		private void UpdateDisplay()
		{
		}

		/// ------------------------------------------------------------------------------------
		private void UpdateComponentList()
		{
			_componentGrid.RowCount = _model.ComponentsOfSelectedSession.Count();
			_componentGrid.Invalidate();
			_componentEditorsTabControl.TabPages.Clear();

			//TODO: editor tab (for now, just the first page) isn't currently
			//being displayed, even though the first component shows as highlighted.
		}

		/// ------------------------------------------------------------------------------------
		private object HandleNewSessionButtonClicked(object sender)
		{
			_model.SetSelectedSession(_model.CreateNewSession());
			_sessionsListPanel.AddItem(_model.SelectedSession, true, true);
			return null;
		}

		/// ------------------------------------------------------------------------------------
		private void HandleSelectedSessionChanged(object sender, object newItem)
		{
			_model.SetSelectedSession(newItem as Session);
			UpdateComponentList();
		}

		/// ------------------------------------------------------------------------------------
		private void _componentGrid_RowEnter(object sender, DataGridViewCellEventArgs e)
		{
			_model.SetSelectedComponentFile(e.RowIndex);
			UpdateComponentEditors();
		}

		private void UpdateComponentEditors()
		{
			_componentEditorsTabControl.TabPages.Clear();

			// At this point, we're just making the tabs and naming them,
			//	so that the entire controls
			// don't need to be build until the user actually tabs to them

			foreach (var provider in _model.GetComponentEditorProviders())
			{
				var page = new TabPage();
				page.Text = provider.TabName;
				page.Tag = provider;
				_componentEditorsTabControl.TabPages.Add(page);
			}

			//TODO: this isn't actually leading to the editor being installed down
			// in _sessionComponentTab_SelectedIndexChanged
			if(_componentEditorsTabControl.TabPages.Count > 0)
			{
				_componentEditorsTabControl.SelectedTab = _componentEditorsTabControl.TabPages[0];
			}
		}

		/// ------------------------------------------------------------------------------------
		private void _componentGrid_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
		{
			var dataPropName = _componentGrid.Columns[e.ColumnIndex].DataPropertyName;
			var currSessionFile = _model.GetComponentFile(e.RowIndex);

			e.Value = (currSessionFile == null ? null :
				ReflectionHelper.GetProperty(currSessionFile, dataPropName));
		}

		private void HandleAfterSessionAdded(object sender, object itemBeingAdded)
		{

		}

		private void _sessionComponentTab_SelectedIndexChanged(object sender, EventArgs e)
		{
			if(_componentEditorsTabControl.SelectedIndex <0)
				return;

			if(_componentEditorsTabControl.Controls.Count > 2)
			{
				return;//already has it
			}

			//TODO: this is getting called each time we select it, so I screwed up somewhere

			var provider = (EditorProvider) _componentEditorsTabControl.SelectedTab.Tag;
			var control =provider.GetEditor(_model.SelectedComponentFile);
			control.Dock = DockStyle.Fill;
			_componentEditorsTabControl.SelectedTab.Controls.Add(control);
		}



	}
}
