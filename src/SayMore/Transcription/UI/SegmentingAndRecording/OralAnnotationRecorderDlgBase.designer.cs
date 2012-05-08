using SilTools.Controls;

namespace SayMore.Transcription.UI
{
	partial class OralAnnotationRecorderBaseDlg
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		#region Component Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.locExtender = new Localization.UI.LocalizationExtender(this.components);
			this._labelRecordButton = new System.Windows.Forms.Label();
			this._labelListenButton = new System.Windows.Forms.Label();
			this._pictureRecording = new System.Windows.Forms.PictureBox();
			this._labelErrorInfo = new System.Windows.Forms.Label();
			this._labelRecordHint = new System.Windows.Forms.Label();
			this._panelPeakMeter = new SilTools.Controls.SilPanel();
			this._labelListenHint = new System.Windows.Forms.Label();
			this._labelFinishedHint = new System.Windows.Forms.Label();
			this._scrollTimer = new System.Windows.Forms.Timer(this.components);
			this._cursorBlinkTimer = new System.Windows.Forms.Timer(this.components);
			this._tableLayoutRecordAnnotations = new System.Windows.Forms.TableLayoutPanel();
			this._tableLayoutMediaButtons = new System.Windows.Forms.TableLayoutPanel();
			this._checkForRecordingDevice = new System.Windows.Forms.Timer(this.components);
			((System.ComponentModel.ISupportInitialize)(this.locExtender)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._pictureRecording)).BeginInit();
			this._tableLayoutRecordAnnotations.SuspendLayout();
			this._tableLayoutMediaButtons.SuspendLayout();
			this.SuspendLayout();
			//
			// locExtender
			//
			this.locExtender.LocalizationManagerId = "SayMore";
			//
			// _labelRecordButton
			//
			this._labelRecordButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._tableLayoutRecordAnnotations.SetColumnSpan(this._labelRecordButton, 2);
			this._labelRecordButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(95)))), ((int)(((byte)(14)))));
			this._labelRecordButton.Image = global::SayMore.Properties.Resources.RecordOralAnnotation;
			this._labelRecordButton.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
			this.locExtender.SetLocalizableToolTip(this._labelRecordButton, "Hold button down to record");
			this.locExtender.SetLocalizationComment(this._labelRecordButton, null);
			this.locExtender.SetLocalizingId(this._labelRecordButton, "DialogBoxes.Transcription.OralAnnotationRecorderDlgBase.RecordButton");
			this._labelRecordButton.Location = new System.Drawing.Point(0, 30);
			this._labelRecordButton.Margin = new System.Windows.Forms.Padding(0, 10, 0, 10);
			this._labelRecordButton.Name = "_labelRecordButton";
			this._labelRecordButton.Size = new System.Drawing.Size(144, 65);
			this._labelRecordButton.TabIndex = 0;
			this._labelRecordButton.Text = "Speak";
			this._labelRecordButton.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			//
			// _labelListenButton
			//
			this._labelListenButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._labelListenButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(129)))), ((int)(((byte)(199)))));
			this._labelListenButton.Image = global::SayMore.Properties.Resources.ListenToOriginalRecording;
			this._labelListenButton.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
			this.locExtender.SetLocalizableToolTip(this._labelListenButton, "Hold button down to listen\\nto original recording");
			this.locExtender.SetLocalizationComment(this._labelListenButton, null);
			this.locExtender.SetLocalizingId(this._labelListenButton, "DialogBoxes.Transcription.OralAnnotationRecorderDlgBase.ListenButton");
			this._labelListenButton.Location = new System.Drawing.Point(0, 30);
			this._labelListenButton.Margin = new System.Windows.Forms.Padding(0, 10, 1, 10);
			this._labelListenButton.Name = "_labelListenButton";
			this._labelListenButton.Size = new System.Drawing.Size(145, 65);
			this._labelListenButton.TabIndex = 0;
			this._labelListenButton.Text = "Listen";
			this._labelListenButton.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			this._labelListenButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.HandleListenToOriginalMouseDown);
			//
			// _pictureRecording
			//
			this._pictureRecording.BackColor = System.Drawing.Color.Transparent;
			this._pictureRecording.Image = global::SayMore.Properties.Resources.BusyWheelSmall;
			this.locExtender.SetLocalizableToolTip(this._pictureRecording, null);
			this.locExtender.SetLocalizationComment(this._pictureRecording, null);
			this.locExtender.SetLocalizationPriority(this._pictureRecording, Localization.LocalizationPriority.NotLocalizable);
			this.locExtender.SetLocalizingId(this._pictureRecording, "pictureBox1.pictureBox1");
			this._pictureRecording.Location = new System.Drawing.Point(58, 5);
			this._pictureRecording.Name = "_pictureRecording";
			this._pictureRecording.Size = new System.Drawing.Size(16, 16);
			this._pictureRecording.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this._pictureRecording.TabIndex = 9;
			this._pictureRecording.TabStop = false;
			this._pictureRecording.Visible = false;
			//
			// _labelErrorInfo
			//
			this._labelErrorInfo.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._labelErrorInfo.AutoSize = true;
			this._labelErrorInfo.BackColor = System.Drawing.Color.Transparent;
			this._labelErrorInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._labelErrorInfo.ForeColor = System.Drawing.Color.White;
			this.locExtender.SetLocalizableToolTip(this._labelErrorInfo, null);
			this.locExtender.SetLocalizationComment(this._labelErrorInfo, null);
			this.locExtender.SetLocalizationPriority(this._labelErrorInfo, Localization.LocalizationPriority.NotLocalizable);
			this.locExtender.SetLocalizingId(this._labelErrorInfo, "OralAnnotationRecorderBaseDlg._labelListenHint");
			this._labelErrorInfo.Location = new System.Drawing.Point(8, 204);
			this._labelErrorInfo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this._labelErrorInfo.Name = "_labelErrorInfo";
			this._labelErrorInfo.Size = new System.Drawing.Size(215, 13);
			this._labelErrorInfo.TabIndex = 2;
			this._labelErrorInfo.Text = "This text will be set programmatically";
			//
			// _labelRecordHint
			//
			this._labelRecordHint.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._labelRecordHint.AutoSize = true;
			this._labelRecordHint.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._labelRecordHint.ForeColor = System.Drawing.Color.White;
			this.locExtender.SetLocalizableToolTip(this._labelRecordHint, null);
			this.locExtender.SetLocalizationComment(this._labelRecordHint, null);
			this.locExtender.SetLocalizingId(this._labelRecordHint, "DialogBoxes.Transcription.OralAnnotationRecorderDlgBase._labelRecordHint");
			this._labelRecordHint.Location = new System.Drawing.Point(9, 271);
			this._labelRecordHint.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this._labelRecordHint.Name = "_labelRecordHint";
			this._labelRecordHint.Size = new System.Drawing.Size(243, 13);
			this._labelRecordHint.TabIndex = 3;
			this._labelRecordHint.Text = "To record, press and hold the SPACE key";
			//
			// _panelPeakMeter
			//
			this._panelPeakMeter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._panelPeakMeter.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(171)))), ((int)(((byte)(173)))), ((int)(((byte)(179)))));
			this._panelPeakMeter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._panelPeakMeter.ClipTextForChildControls = true;
			this._panelPeakMeter.ControlReceivingFocusOnMnemonic = null;
			this._panelPeakMeter.DoubleBuffered = true;
			this._panelPeakMeter.DrawOnlyBottomBorder = false;
			this._panelPeakMeter.DrawOnlyTopBorder = false;
			this._panelPeakMeter.Font = new System.Drawing.Font("Segoe UI", 9F);
			this._panelPeakMeter.ForeColor = System.Drawing.SystemColors.ControlText;
			this.locExtender.SetLocalizableToolTip(this._panelPeakMeter, null);
			this.locExtender.SetLocalizationComment(this._panelPeakMeter, null);
			this.locExtender.SetLocalizingId(this._panelPeakMeter, "OralAnnotationRecorderBaseDlg._panelPeakMeter");
			this._panelPeakMeter.Location = new System.Drawing.Point(28, 108);
			this._panelPeakMeter.Margin = new System.Windows.Forms.Padding(8, 3, 8, 3);
			this._panelPeakMeter.MnemonicGeneratesClick = false;
			this._panelPeakMeter.Name = "_panelPeakMeter";
			this._panelPeakMeter.Padding = new System.Windows.Forms.Padding(0, 0, 0, 1);
			this._panelPeakMeter.PaintExplorerBarBackground = false;
			this._panelPeakMeter.Size = new System.Drawing.Size(108, 17);
			this._panelPeakMeter.TabIndex = 1;
			//
			// _labelListenHint
			//
			this._labelListenHint.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._labelListenHint.AutoSize = true;
			this._labelListenHint.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._labelListenHint.ForeColor = System.Drawing.Color.White;
			this.locExtender.SetLocalizableToolTip(this._labelListenHint, null);
			this.locExtender.SetLocalizationComment(this._labelListenHint, null);
			this.locExtender.SetLocalizingId(this._labelListenHint, "DialogBoxes.Transcription.OralAnnotationRecorderDlgBase._labelListenHint");
			this._labelListenHint.Location = new System.Drawing.Point(9, 234);
			this._labelListenHint.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this._labelListenHint.Name = "_labelListenHint";
			this._labelListenHint.Size = new System.Drawing.Size(376, 13);
			this._labelListenHint.TabIndex = 10;
			this._labelListenHint.Text = "To listen to the original recording, press and hold the SPACE key";
			//
			// _labelFinishedHint
			//
			this._labelFinishedHint.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._labelFinishedHint.AutoSize = true;
			this._labelFinishedHint.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._labelFinishedHint.ForeColor = System.Drawing.Color.Green;
			this.locExtender.SetLocalizableToolTip(this._labelFinishedHint, null);
			this.locExtender.SetLocalizationComment(this._labelFinishedHint, null);
			this.locExtender.SetLocalizingId(this._labelFinishedHint, "DialogBoxes.Transcription.OralAnnotationRecorderDlgBase._labelFinishedHint");
			this._labelFinishedHint.Location = new System.Drawing.Point(11, 306);
			this._labelFinishedHint.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this._labelFinishedHint.Name = "_labelFinishedHint";
			this._labelFinishedHint.Size = new System.Drawing.Size(58, 13);
			this._labelFinishedHint.TabIndex = 11;
			this._labelFinishedHint.Text = "Finished!";
			this._labelFinishedHint.Visible = false;
			//
			// _scrollTimer
			//
			this._scrollTimer.Interval = 500;
			//
			// _cursorBlinkTimer
			//
			this._cursorBlinkTimer.Interval = 600;
			this._cursorBlinkTimer.Tick += new System.EventHandler(this.HandleCursorBlinkTimerTick);
			//
			// _tableLayoutRecordAnnotations
			//
			this._tableLayoutRecordAnnotations.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._tableLayoutRecordAnnotations.ColumnCount = 2;
			this._tableLayoutRecordAnnotations.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this._tableLayoutRecordAnnotations.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._tableLayoutRecordAnnotations.Controls.Add(this._labelRecordButton, 0, 1);
			this._tableLayoutRecordAnnotations.Controls.Add(this._panelPeakMeter, 1, 2);
			this._tableLayoutRecordAnnotations.Location = new System.Drawing.Point(1, 163);
			this._tableLayoutRecordAnnotations.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
			this._tableLayoutRecordAnnotations.Name = "_tableLayoutRecordAnnotations";
			this._tableLayoutRecordAnnotations.RowCount = 3;
			this._tableLayoutRecordAnnotations.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this._tableLayoutRecordAnnotations.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutRecordAnnotations.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutRecordAnnotations.Size = new System.Drawing.Size(144, 150);
			this._tableLayoutRecordAnnotations.TabIndex = 1;
			//
			// _tableLayoutMediaButtons
			//
			this._tableLayoutMediaButtons.ColumnCount = 1;
			this._tableLayoutMediaButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._tableLayoutMediaButtons.Controls.Add(this._labelListenButton, 0, 1);
			this._tableLayoutMediaButtons.Controls.Add(this._tableLayoutRecordAnnotations, 0, 2);
			this._tableLayoutMediaButtons.Location = new System.Drawing.Point(357, 16);
			this._tableLayoutMediaButtons.Name = "_tableLayoutMediaButtons";
			this._tableLayoutMediaButtons.RowCount = 3;
			this._tableLayoutMediaButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this._tableLayoutMediaButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._tableLayoutMediaButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 150F));
			this._tableLayoutMediaButtons.Size = new System.Drawing.Size(146, 313);
			this._tableLayoutMediaButtons.TabIndex = 8;
			this._tableLayoutMediaButtons.Paint += new System.Windows.Forms.PaintEventHandler(this.HandleMediaButtonTableLayoutPaint);
			//
			// _checkForRecordingDevice
			//
			this._checkForRecordingDevice.Interval = 500;
			this._checkForRecordingDevice.Tick += new System.EventHandler(this.CheckForRecordingDevice);
			//
			// OralAnnotationRecorderBaseDlg
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(703, 412);
			this.Controls.Add(this._labelFinishedHint);
			this.Controls.Add(this._labelRecordHint);
			this.Controls.Add(this._pictureRecording);
			this.Controls.Add(this._labelListenHint);
			this.Controls.Add(this._labelErrorInfo);
			this.Controls.Add(this._tableLayoutMediaButtons);
			this.Cursor = System.Windows.Forms.Cursors.Default;
			this.locExtender.SetLocalizableToolTip(this, null);
			this.locExtender.SetLocalizationComment(this, "Localized in subclass");
			this.locExtender.SetLocalizationPriority(this, Localization.LocalizationPriority.NotLocalizable);
			this.locExtender.SetLocalizingId(this, "DialogBoxes.Transcription.CarefulSpeechAnnotationDlg.WindowTitle");
			this.MinimumSize = new System.Drawing.Size(330, 415);
			this.Name = "OralAnnotationRecorderBaseDlg";
			this.Opacity = 1D;
			this.Text = "Change my text";
			this.Controls.SetChildIndex(this._tableLayoutMediaButtons, 0);
			this.Controls.SetChildIndex(this._labelErrorInfo, 0);
			this.Controls.SetChildIndex(this._labelListenHint, 0);
			this.Controls.SetChildIndex(this._pictureRecording, 0);
			this.Controls.SetChildIndex(this._labelRecordHint, 0);
			this.Controls.SetChildIndex(this._labelFinishedHint, 0);
			((System.ComponentModel.ISupportInitialize)(this.locExtender)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._pictureRecording)).EndInit();
			this._tableLayoutRecordAnnotations.ResumeLayout(false);
			this._tableLayoutMediaButtons.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private Localization.UI.LocalizationExtender locExtender;
		private System.Windows.Forms.Timer _scrollTimer;
		private System.Windows.Forms.Timer _cursorBlinkTimer;
		protected System.Windows.Forms.TableLayoutPanel _tableLayoutRecordAnnotations;
		private System.Windows.Forms.Label _labelRecordButton;
		private System.Windows.Forms.Label _labelListenButton;
		protected System.Windows.Forms.TableLayoutPanel _tableLayoutMediaButtons;
		private System.Windows.Forms.PictureBox _pictureRecording;
		private System.Windows.Forms.Label _labelErrorInfo;
		private System.Windows.Forms.Label _labelRecordHint;
		private SilPanel _panelPeakMeter;
		private System.Windows.Forms.Label _labelListenHint;
		private System.Windows.Forms.Label _labelFinishedHint;
		private System.Windows.Forms.Timer _checkForRecordingDevice;
	}
}
