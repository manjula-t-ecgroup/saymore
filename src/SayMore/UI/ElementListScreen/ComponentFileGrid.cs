using System;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Media;
using System.Windows.Forms;
using SilTools;
using SayMore.Model.Files;
using SayMore.Properties;

namespace SayMore.UI.ElementListScreen
{
	/// ----------------------------------------------------------------------------------------
	public partial class ComponentFileGrid : UserControl
	{
		private IEnumerable<ComponentFile> _files;
		private string _gridColSettingPrefix;

		/// <summary>
		/// When the user selects a different component, this is called
		/// </summary>
		public Action<int> AfterComponentSelected;

		/// <summary>
		/// When the user chooses a menu command, this is called after the command is issued.
		/// </summary>
		public Action<string> PostMenuCommandRefreshAction;

		public Func<string[], DragDropEffects> FilesBeingDraggedOverGrid;
		public Func<string[], bool> FilesDroppedOnGrid;
		public Func<string[], bool> FilesAdded;
		public Func<ComponentFile, bool> FileDeletionAction;
		public Func<bool> IsOKToSelectDifferentFile;
		public Func<bool> IsOKToDoFileOperation;

		public bool ShowContextMenu { get; set; }

		/// ------------------------------------------------------------------------------------
		public ComponentFileGrid()
		{
			ShowContextMenu = true;

			InitializeComponent();
			Font = SystemFonts.IconTitleFont;
			_toolStripActions.Renderer = new NoToolStripBorderRenderer();

			try
			{
				// Setting AllowDrop when tests are running throws an exception and
				// I'm not quite sure why, even after using reflector to look at the
				// code behind setting the property. Nonetheless, I've seen this before
				// so I'm just going to ignore any exception thrown when enabling drag
				// and drop. The worst that could happen by ignorning an exception
				// when the user runs the program (which should never happen), is that
				// they won't have the drag/drop feature.
				_grid.AllowDrop = true;
			}
			catch { }

			_grid.DragEnter += HandleFileGridDragEnter;
			_grid.DragDrop += HandleFileGridDragDrop;
			_grid.CellMouseClick += HandleFileGridCellMouseClick;
			_grid.CellValueNeeded += HandleFileGridCellValueNeeded;
			_grid.CellDoubleClick += HandleFileGridCellDoubleClick;
			_grid.CurrentRowChanged += HandleFileGridCurrentRowChanged;
			_grid.Paint += HandleFileGridPaint;
			_grid.CellPainting += HandleFileGridCellPainting;
			_grid.ClientSizeChanged += HandleFileGridClientSizeChanged;
			_grid.Font = SystemFonts.IconTitleFont;

			_grid.IsOkToChangeRows = (() => (IsOKToSelectDifferentFile == null || IsOKToSelectDifferentFile()));

			var clr = ColorHelper.CalculateColor(Color.White,
				 _grid.DefaultCellStyle.SelectionBackColor, 140);

			_grid.FullRowFocusRectangleColor = _grid.DefaultCellStyle.SelectionBackColor;
			_grid.DefaultCellStyle.SelectionBackColor = clr;
			_grid.DefaultCellStyle.SelectionForeColor = _grid.DefaultCellStyle.ForeColor;

			_menuDeleteFile.Click += ((s, e) => DeleteFile());
		}

		/// ------------------------------------------------------------------------------------
		public void InitializeGrid(string settingPrefix, string addFileButtonTooltipText)
		{
			_buttonAddFiles.ToolTipText = addFileButtonTooltipText;

			_gridColSettingPrefix = settingPrefix;

			if (Settings.Default[_gridColSettingPrefix + "ComponentGrid"] != null)
				((GridSettings)Settings.Default[_gridColSettingPrefix + "ComponentGrid"]).InitializeGrid(_grid);

			_grid.AutoResizeColumnHeadersHeight();
			_grid.ColumnHeadersHeight += 8;
		}

		/// ------------------------------------------------------------------------------------
		protected override void OnHandleDestroyed(EventArgs e)
		{
			if (!DesignMode)
				Settings.Default[_gridColSettingPrefix + "ComponentGrid"] = GridSettings.Create(_grid);

			base.OnHandleDestroyed(e);
		}

		/// ------------------------------------------------------------------------------------
		public bool AddButtonEnabled
		{
			get { return _buttonAddFiles.Enabled; }
			set { _buttonAddFiles.Enabled = value; }
		}

		/// ------------------------------------------------------------------------------------
		public bool AddButtonVisible
		{
			get { return _buttonAddFiles.Visible; }
			set { _buttonAddFiles.Visible = value; }
		}

		/// ------------------------------------------------------------------------------------
		public bool CreateAnnotationFileButtonVisible
		{
			get { return _buttonCreateAnnotationFile.Visible; }
			set { _buttonCreateAnnotationFile.Visible = value; }
		}

		/// ------------------------------------------------------------------------------------
		public bool RenameButtonVisible
		{
			get { return _buttonRename.Visible; }
			set { _buttonRename.Visible = value; }
		}

		/// ------------------------------------------------------------------------------------
		public bool ConvertButtonVisible
		{
			get { return _buttonConvert.Visible; }
			set { _buttonConvert.Visible = value; }
		}

		/// ------------------------------------------------------------------------------------
		[Browsable(false)]
		public DataGridView Grid
		{
			get { return _grid; }
		}

		/// ------------------------------------------------------------------------------------
		[Browsable(false)]
		public ToolStripButton AddButton
		{
			get { return _buttonAddFiles; }
		}

		/// ------------------------------------------------------------------------------------
		private void HandleFileGridClientSizeChanged(object sender, EventArgs e)
		{
			// This will recenter the grid's hint message.
			if (_grid.RowCount == 1)
				_grid.Invalidate();
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Paints a message on the files grid.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void HandleFileGridPaint(object sender, PaintEventArgs e)
		{
			if (_grid.RowCount == 1)
			{
				var rcRow = _grid.GetRowDisplayRectangle(0, false);

				var msg = Program.GetString("ComponentFileGrid.AddFilesPrompt",
					"Add additional files related to this event by\ndragging them here or clicking the 'Add Files' button.");

				_grid.DrawMessageInCenterOfGrid(e.Graphics, msg, rcRow.Height);
			}
		}

		/// ------------------------------------------------------------------------------------
		void HandleFileGridCellPainting(object sender, DataGridViewCellPaintingEventArgs e)
		{
			if (e.ColumnIndex < 0 || e.ColumnIndex > 1 || e.RowIndex < 0)
				return;

			var file = _files.ElementAt(e.RowIndex);

			if (e.ColumnIndex == 0)
				DrawIconColumnCell(file.DisplayIndentLevel > 0, e);
			else if (file.DisplayIndentLevel > 0)
				DrawIndentedFileName(file, e);

			if (_grid.Focused)
				_grid.DrawFocusRectangle(e);
		}

		/// ------------------------------------------------------------------------------------
		private void DrawIndentedFileName(ComponentFile file, DataGridViewCellPaintingEventArgs e)
		{
			// TODO: Account for indent levels greater than 1 if we ever need to.

			e.Handled = true;

			var selected = ((e.State & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected);
			var clrFore = (selected ? e.CellStyle.SelectionForeColor : e.CellStyle.ForeColor);

			var rc = e.CellBounds;
			var paintParts = e.PaintParts;
			paintParts &= ~DataGridViewPaintParts.ContentForeground;
			e.Paint(rc, paintParts);

			// Draw the icon.
			var img = _grid[0, e.RowIndex].Value as Image;
			rc.X += 8;
			rc.Y += ((rc.Height - img.Height) / 2);
			rc.Height = img.Height;
			rc.Width = img.Width;
			e.Graphics.DrawImage(img, rc);

			// Draw the file name, indented.
			rc = e.CellBounds;
			rc.X += (img.Width + 12);
			rc.Width -= (img.Width + 12);
			rc.Height--;

			TextRenderer.DrawText(e.Graphics, e.Value as string, e.CellStyle.Font, rc,
				clrFore, TextFormatFlags.EndEllipsis | TextFormatFlags.VerticalCenter);
		}

		/// ------------------------------------------------------------------------------------
		private void DrawIconColumnCell(bool drawConnectorLine, DataGridViewCellPaintingEventArgs e)
		{
			e.Handled = true;
			var rc = e.CellBounds;
			var paintParts = e.PaintParts;

			if (drawConnectorLine)
				paintParts &= ~DataGridViewPaintParts.ContentForeground;

			e.Paint(rc, paintParts);

			var selected = ((e.State & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected);
			var clrFore = (selected ? e.CellStyle.SelectionForeColor : e.CellStyle.ForeColor);
			var clrBack = (selected ? e.CellStyle.SelectionBackColor : e.CellStyle.BackColor);

			// Don't paint the border on the right edge of the icon column.
			using (var pen = new Pen(clrBack))
			{
				e.Graphics.DrawLine(pen, new Point(rc.Right - 1, rc.Y),
					new Point(rc.Right - 1, rc.Bottom));
			}

			if (!drawConnectorLine)
				return;

			// Draw a dotted, right-angle line linking the row to the one above.
			using (var pen = new Pen(ColorHelper.CalculateColor(Color.White, clrFore, 130)))
			{
				var dx = rc.X + (rc.Width / 2);
				var dy = rc.Y + (rc.Height / 2) + 1;

				pen.DashStyle = DashStyle.Dot;
				e.Graphics.DrawLines(pen, new[]
				{
					new Point(dx, rc.Y + 1),
					new Point(dx, dy),
					new Point(rc.Right, dy)
				});

				// If the following row is also indented, then draw the connector so it spans
				// the height of the cell, rather tha stop where the horizontal line starts.
				if (e.RowIndex + 1 < _grid.RowCount && _files.ElementAt(e.RowIndex + 1).DisplayIndentLevel > 0)
					e.Graphics.DrawLine(pen, dx, rc.Y + 1, dx, rc.Y + rc.Height - 1);
			}
		}

		/// ------------------------------------------------------------------------------------
		private void HandleFileGridCellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
		{
			if (e.Button != MouseButtons.Right || (IsOKToDoFileOperation != null && !IsOKToDoFileOperation()))
				return;

			if (e.RowIndex != _grid.CurrentCellAddress.Y)
				SelectComponent(e.RowIndex);

			if (!ShowContextMenu)
				return;

			var file = _files.ElementAt(e.RowIndex);
			var deleteMenu = _menuDeleteFile;
			_contextMenuStrip.Items.Clear();
			_menuDeleteFile = deleteMenu;
			_contextMenuStrip.Items.AddRange(file.GetMenuCommands(PostMenuCommandRefreshAction).ToArray());

			if (GetIsOKToDeleteCurrentFile())
			{
				_contextMenuStrip.Items.Add(new ToolStripSeparator());
				_contextMenuStrip.Items.Add(_menuDeleteFile);
			}

			_contextMenuStrip.Show(MousePosition);
		}

		/// ------------------------------------------------------------------------------------
		private void HandleFileGridCellDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			// Checking for greater-than-or-equal to zero, throws out double-clicks
			// on the column headings, in which case the row index is -1.
			if (e.RowIndex >= 0)
			{
				_grid.CurrentCell = _grid[e.ColumnIndex, e.RowIndex];
				var file = _files.ElementAt(e.RowIndex);
				file.HandleDoubleClick();
			}
		}

		/// ----------------------------------------------------------------------------------------
		private void HandleFileGridColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
		{
			_grid.AutoResizeColumnHeadersHeight();
			_grid.ColumnHeadersHeight += 8;
		}

		/// ------------------------------------------------------------------------------------
		protected virtual void HandleFileGridCurrentRowChanged(object sender, EventArgs e)
		{
			if (Disposing)
				return;

			BuildMenuCommands(_grid.CurrentCellAddress.Y);

			var file = (_files.Count() > 0 && _grid.CurrentCellAddress.Y >= 0 ?
				_files.ElementAt(_grid.CurrentCellAddress.Y) : null);

			_buttonCreateAnnotationFile.Enabled = (file != null &&
				file.GetCanHaveAnnotationFile() && !file.GetDoesHaveAnnotationFile());

			if (null != AfterComponentSelected && _grid.CurrentCellAddress.Y >= 0)
				AfterComponentSelected(_grid.CurrentCellAddress.Y);
		}

		/// ------------------------------------------------------------------------------------
		protected virtual void HandleFileGridCellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
		{
			var propName = _grid.Columns[e.ColumnIndex].DataPropertyName;
			var currFile = _files.ElementAt(e.RowIndex);
			e.Value = (currFile == null ? null : ReflectionHelper.GetProperty(currFile, propName));
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Highlights the component file at the specified index. If the index is out of
		/// range, then it is ignored.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public void SelectComponent(int index)
		{
			_grid.CurrentCell = (index >= 0 && index < _files.Count() ? _grid[0, index] : null);
			BuildMenuCommands(index);
		}

		/// ------------------------------------------------------------------------------------
		private void BuildMenuCommands(int index)
		{
			_buttonOpen.DropDown.Items.Clear();
			_buttonConvert.DropDown.Items.Clear();
			_buttonRename.DropDown.Items.Clear();

			var file = (index >= 0 && index < _files.Count() ? _files.ElementAt(index) : null);

			if (file != null)
			{
				foreach (var item in file.GetMenuCommands(PostMenuCommandRefreshAction))
				{
					switch (item.Tag as string)
					{
						case "open": _buttonOpen.DropDown.Items.Add(item); break;
						case "rename": _buttonRename.DropDown.Items.Add(item); break;
						case "convert": _buttonConvert.DropDown.Items.Add(item); break;
					}
				}
			}

			_buttonOpen.Enabled = (_buttonOpen.DropDown.Items.Count > 0);
			_buttonConvert.Enabled = (_buttonConvert.DropDown.Items.Count > 0);
			_buttonRename.Enabled = (_buttonRename.DropDown.Items.Count > 0);
		}

		/// ------------------------------------------------------------------------------------
		public void TrySetComponent(string file)
		{
			if (string.IsNullOrEmpty(file))
				return;

			file = Path.GetFileName(file);
			int i = 0;
			foreach (var f in _files)
			{
				if (Path.GetFileName(f.PathToAnnotatedFile) == file)
				{
					SelectComponent(i);
					return;
				}

				i++;
			}
		}

		/// ------------------------------------------------------------------------------------
		public void UpdateComponentFileList(IEnumerable<ComponentFile> componentFiles)
		{
			UpdateComponentFileList(componentFiles, null);
		}

		/// ------------------------------------------------------------------------------------
		private void UpdateComponentFileList(IEnumerable<ComponentFile> componentFiles,
			ComponentFile fileToSelectAfterUpdate)
		{
			if (fileToSelectAfterUpdate == null)
			{
				fileToSelectAfterUpdate = (_grid.CurrentCellAddress.Y >= 0 && _files.Count() > 0 ?
					_files.ElementAt(_grid.CurrentCellAddress.Y) : null);
			}

			_files = componentFiles;

			// I (David) think there's a bug in the grid that fires the cell value needed
			// event in the process of changing the row count but it fires it for rows that
			// are no longer supposed to exist. This tends to happen when the row count was
			// previously higher than the new value.
			_grid.CellValueNeeded -= HandleFileGridCellValueNeeded;
			_grid.CurrentCell = null;
			_grid.RowCount = _files.Count();
			_grid.CellValueNeeded += HandleFileGridCellValueNeeded;
			_grid.Invalidate();

			// Try to select the same file that was selected before updating the list.
			if (fileToSelectAfterUpdate != null)
				TrySetComponent(fileToSelectAfterUpdate.PathToAnnotatedFile);
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Draw a line between the buttons and the grid.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void HandleToolStripActionsPaint(object sender, PaintEventArgs e)
		{
			var dy = _toolStripActions.ClientSize.Height - _toolStripActions.Padding.Bottom - 1;

			using (var pen = new Pen(_panelOuter.BorderColor))
				e.Graphics.DrawLine(pen, 0, dy, _toolStripActions.ClientSize.Width, dy);
		}

		/// ------------------------------------------------------------------------------------
		private void HandleFileGridDragEnter(object sender, DragEventArgs e)
		{
			e.Effect = DragDropEffects.None;

			if (e.Data.GetFormats().Contains(DataFormats.FileDrop) && FilesBeingDraggedOverGrid != null)
				e.Effect = FilesBeingDraggedOverGrid(e.Data.GetData(DataFormats.FileDrop) as string[]);
		}

		/// ------------------------------------------------------------------------------------
		private void HandleFileGridDragDrop(object sender, DragEventArgs e)
		{
			if (!GetIsOKToPerformFileOperation())
				return;

			if (e.Data.GetFormats().Contains(DataFormats.FileDrop) && FilesDroppedOnGrid != null)
				FilesDroppedOnGrid(e.Data.GetData(DataFormats.FileDrop) as string[]);
		}

		/// ------------------------------------------------------------------------------------
		private void HandleActionsDropDownOpening(object sender, EventArgs e)
		{
			bool operationOK = GetIsOKToPerformFileOperation();

			var dropdown = sender as ToolStripDropDownButton;
			foreach (ToolStripItem item in dropdown.DropDownItems)
				item.Visible = operationOK;
		}

		/// ------------------------------------------------------------------------------------
		private void HandleCreateAnnotationFileButtonClick(object sender, EventArgs e)
		{
			_files.ElementAt(_grid.CurrentCellAddress.Y).CreateAnnotationFile(PostMenuCommandRefreshAction);
		}

		/// ------------------------------------------------------------------------------------
		private void HandleAddButtonClick(object sender, EventArgs e)
		{
			if (!GetIsOKToPerformFileOperation())
				return;

			using (var dlg = new OpenFileDialog())
			{
				dlg.Title = Program.GetString("ComponentFileGrid.AddFilesDlgCaption", "Add Files");
				var prjFilterText = Program.GetString("ComponentFileGrid.AddFilesFileType", "All Files (*.*)");

				var folder = Settings.Default.LastFolderForComponentFileAdd;
				if (folder == null || !Directory.Exists(folder))
					folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

				dlg.Filter = prjFilterText + "|*.*";
				dlg.InitialDirectory = folder;
				dlg.CheckFileExists = true;
				dlg.CheckPathExists = true;
				if (dlg.ShowDialog(this) == DialogResult.OK && FilesAdded != null)
					FilesAdded(dlg.FileNames);

				if (!string.IsNullOrEmpty(dlg.FileName))
					Settings.Default.LastFolderForComponentFileAdd = Path.GetDirectoryName(dlg.FileName);
			}
		}

		/// ------------------------------------------------------------------------------------
		private void HandleFileGridKeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete && GetIsOKToDeleteCurrentFile())
				DeleteFile();
		}

		/// ------------------------------------------------------------------------------------
		private void DeleteFile()
		{
			var index = _grid.CurrentCellAddress.Y;
			var currFile = _files.ElementAt(index);

			if (currFile == null || FileDeletionAction == null)
				return;

			var annotationFile = currFile.GetAnnotationFile();

			if (!FileDeletionAction(currFile))
				return;

			var newList = _files.ToList();
			newList.Remove(currFile);

			if (annotationFile != null)
				newList.Remove(annotationFile);

			if (index == newList.Count)
				index--;

			UpdateComponentFileList(newList, newList[index]);
			HandleFileGridCurrentRowChanged(null, null);
		}

		/// ------------------------------------------------------------------------------------
		private bool GetIsOKToPerformFileOperation()
		{
			if (IsOKToDoFileOperation == null || IsOKToDoFileOperation())
				return true;

			SystemSounds.Beep.Play();
			return false;
		}

		/// ------------------------------------------------------------------------------------
		private bool GetIsOKToDeleteCurrentFile()
		{
			if (_grid.CurrentCellAddress.Y < 0 || _grid.CurrentCellAddress.Y >= _files.Count())
				return false;

			var currFile = _files.ElementAt(_grid.CurrentCellAddress.Y);
			return !(currFile is ProjectElementComponentFile);
		}
	}

	/// ----------------------------------------------------------------------------------------
	internal class InternalComponentFileGrid : SilGrid
	{
		private int _prevRow = -1;

		internal Func<bool> IsOkToChangeRows;

		/// ----------------------------------------------------------------------------------------
		protected override void OnCellMouseDown(DataGridViewCellMouseEventArgs e)
		{
			if (_prevRow >= 0 && _prevRow != e.RowIndex && e.RowIndex >= 0 &&
				IsOkToChangeRows != null && !IsOkToChangeRows())
			{
				return;
			}

			_prevRow = e.RowIndex;
			base.OnCellMouseDown(e);
		}
	}
}
