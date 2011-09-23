using System;
using System.Drawing;
using System.Windows.Forms;
using SayMore.Model.Files;
using SayMore.Properties;
using SilTools;
using SilTools.Controls;

namespace SayMore.UI.ComponentEditors
{
	/// ----------------------------------------------------------------------------------------
	public partial class ImageViewer : EditorBase
	{
		private readonly SilPanel _panelImage;
		private ImageViewerViewModel _model;

		/// ------------------------------------------------------------------------------------
		public ImageViewer(ComponentFile file, string tabText)
			: base(file, tabText, "Image")
		{
			InitializeComponent();
			Name = "ImageViewer";

			_labelZoom.Font = SystemFonts.IconTitleFont;
			_zoomTrackBar.BackColor = BackColor;

			_panelImage = new SilPanel();
			_panelImage.Dock = DockStyle.Fill;
			_panelImage.AutoScroll = true;
			_panelImage.Cursor = Cursors.Hand;
			Controls.Add(_panelImage);
			_panelImage.BringToFront();
			_panelImage.Paint += HandleImagePanelPaint;
			_panelImage.Scroll += HandleImagePanelScroll;
			_panelImage.MouseClick += HandleImagePanelMouseClick;
			_panelImage.MouseDoubleClick += HandleImagePanelMouseClick;

			SetComponentFile(file);
		}

		/// ------------------------------------------------------------------------------------
		protected override void OnBackColorChanged(EventArgs e)
		{
			base.OnBackColorChanged(e);

			if (_zoomTrackBar != null)
				_zoomTrackBar.BackColor = BackColor;
		}

		/// ------------------------------------------------------------------------------------
		private void Initialize(string imageFileName)
		{
			var clickZoomPercentages = PortableSettingsProvider.GetIntArrayFromString(
				Settings.Default.ImageViewerClickImageZoomPercentages);

			_model = new ImageViewerViewModel(imageFileName, clickZoomPercentages);
		}

		/// ------------------------------------------------------------------------------------
		public override void SetComponentFile(ComponentFile file)
		{
			base.SetComponentFile(file);
			Initialize(file.PathToAnnotatedFile);

			if (_zoomTrackBar != null && _panelImage != null)
			{
				_zoomTrackBar.Value = _model.GetPercentOfImageSizeToFitSize(100,
					_zoomTrackBar.Minimum, _panelImage.ClientSize);

				_panelImage.AutoScrollMinSize = _model.GetScaledSize(_zoomTrackBar.Value);
				_panelImage.Invalidate();
			}
		}

		/// ------------------------------------------------------------------------------------
		void HandleImagePanelMouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left)
				return;

			_zoomTrackBar.Value = _model.GetNextClickPercent(_zoomTrackBar.Value);

			if (!_zoomTrackBar.Focused)
				_zoomTrackBar.Focus();
		}

		/// ------------------------------------------------------------------------------------
		void HandleImagePanelScroll(object sender, ScrollEventArgs e)
		{
			_panelImage.Invalidate();
		}

		/// ------------------------------------------------------------------------------------
		void HandleImagePanelPaint(object sender, PaintEventArgs e)
		{
			var sz = _panelImage.AutoScrollMinSize;

			var dx = (sz.Width > _panelImage.ClientSize.Width ?
				_panelImage.AutoScrollPosition.X : (_panelImage.ClientSize.Width - sz.Width) / 2);

			var dy = (sz.Height > _panelImage.ClientSize.Height ?
				_panelImage.AutoScrollPosition.Y : (_panelImage.ClientSize.Height - sz.Height) / 2);

			var rc = new Rectangle(new Point(dx, dy), sz);

			e.Graphics.DrawImage(_model.Image, rc);
		}

		/// ------------------------------------------------------------------------------------
		private void HandleZoomTrackBarValueChanged(object sender, EventArgs e)
		{
			// REVIEW: Perhaps this information in a tooltip may be helpful.
			//var fmt = LocalizationManager.LocalizeString("ImageViewer.ZoomValueFormat", "{0}%");
			//_labelZoomPercent.Text = string.Format(fmt, _zoomTrackBar.Value);

			_panelImage.AutoScrollMinSize = _model.GetScaledSize(_zoomTrackBar.Value);
			_panelImage.Invalidate();
		}
	}
}
