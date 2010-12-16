using Localization;
using Localization.UI;
using SilUtils.Controls;

namespace SayMore.UI.LowLevelControls
{
	partial class ListPanel
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
			this._outerPanel = new SilUtils.Controls.SilPanel();
			this._buttonsFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
			this._buttonNew = new System.Windows.Forms.Button();
			this._buttonDelete = new System.Windows.Forms.Button();
			this._headerLabel = new SilUtils.Controls.HeaderLabel();
			this._buttonColChooser = new SilUtils.Controls.XButton();
			this.locExtender = new Localization.UI.LocalizationExtender(this.components);
			this._toolTip = new System.Windows.Forms.ToolTip(this.components);
			this._outerPanel.SuspendLayout();
			this._buttonsFlowLayoutPanel.SuspendLayout();
			this._headerLabel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.locExtender)).BeginInit();
			this.SuspendLayout();
			// 
			// _outerPanel
			// 
			this._outerPanel.BackColor = System.Drawing.SystemColors.Window;
			this._outerPanel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(166)))), ((int)(((byte)(170)))));
			this._outerPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._outerPanel.ClipTextForChildControls = true;
			this._outerPanel.ControlReceivingFocusOnMnemonic = null;
			this._outerPanel.Controls.Add(this._buttonsFlowLayoutPanel);
			this._outerPanel.Controls.Add(this._headerLabel);
			this._outerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._outerPanel.DoubleBuffered = true;
			this._outerPanel.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World);
			this.locExtender.SetLocalizableToolTip(this._outerPanel, null);
			this.locExtender.SetLocalizationComment(this._outerPanel, null);
			this.locExtender.SetLocalizationPriority(this._outerPanel, Localization.LocalizationPriority.NotLocalizable);
			this.locExtender.SetLocalizingId(this._outerPanel, "ListPanel._outerPanel");
			this._outerPanel.Location = new System.Drawing.Point(0, 0);
			this._outerPanel.MnemonicGeneratesClick = false;
			this._outerPanel.Name = "_outerPanel";
			this._outerPanel.PaintExplorerBarBackground = false;
			this._outerPanel.Size = new System.Drawing.Size(170, 277);
			this._outerPanel.TabIndex = 1;
			// 
			// _buttonsFlowLayoutPanel
			// 
			this._buttonsFlowLayoutPanel.AutoSize = true;
			this._buttonsFlowLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._buttonsFlowLayoutPanel.Controls.Add(this._buttonNew);
			this._buttonsFlowLayoutPanel.Controls.Add(this._buttonDelete);
			this._buttonsFlowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this._buttonsFlowLayoutPanel.Location = new System.Drawing.Point(0, 239);
			this._buttonsFlowLayoutPanel.Name = "_buttonsFlowLayoutPanel";
			this._buttonsFlowLayoutPanel.Padding = new System.Windows.Forms.Padding(2);
			this._buttonsFlowLayoutPanel.Size = new System.Drawing.Size(168, 36);
			this._buttonsFlowLayoutPanel.TabIndex = 3;
			this._buttonsFlowLayoutPanel.TabStop = true;
			this._buttonsFlowLayoutPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.HandleButtonPanelPaint);
			// 
			// _buttonNew
			// 
			this._buttonNew.AutoSize = true;
			this._buttonNew.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.locExtender.SetLocalizableToolTip(this._buttonNew, null);
			this.locExtender.SetLocalizationComment(this._buttonNew, null);
			this.locExtender.SetLocalizingId(this._buttonNew, "ListPanel._newButton");
			this._buttonNew.Location = new System.Drawing.Point(5, 5);
			this._buttonNew.MinimumSize = new System.Drawing.Size(75, 26);
			this._buttonNew.Name = "_buttonNew";
			this._buttonNew.Size = new System.Drawing.Size(75, 26);
			this._buttonNew.TabIndex = 0;
			this._buttonNew.Text = "&New";
			this._buttonNew.UseVisualStyleBackColor = true;
			this._buttonNew.Click += new System.EventHandler(this.btnNew_Click);
			// 
			// _buttonDelete
			// 
			this._buttonDelete.AutoSize = true;
			this._buttonDelete.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.locExtender.SetLocalizableToolTip(this._buttonDelete, null);
			this.locExtender.SetLocalizationComment(this._buttonDelete, null);
			this.locExtender.SetLocalizingId(this._buttonDelete, "ListPanel._deleteButton");
			this._buttonDelete.Location = new System.Drawing.Point(86, 5);
			this._buttonDelete.MinimumSize = new System.Drawing.Size(75, 26);
			this._buttonDelete.Name = "_buttonDelete";
			this._buttonDelete.Size = new System.Drawing.Size(75, 26);
			this._buttonDelete.TabIndex = 1;
			this._buttonDelete.Text = "&Delete";
			this._buttonDelete.UseVisualStyleBackColor = true;
			this._buttonDelete.Click += new System.EventHandler(this.btnDelete_Click);
			// 
			// _headerLabel
			// 
			this._headerLabel.ClipTextForChildControls = true;
			this._headerLabel.ControlReceivingFocusOnMnemonic = null;
			this._headerLabel.Controls.Add(this._buttonColChooser);
			this._headerLabel.Dock = System.Windows.Forms.DockStyle.Top;
			this._headerLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World);
			this.locExtender.SetLocalizableToolTip(this._headerLabel, null);
			this.locExtender.SetLocalizationComment(this._headerLabel, "Localized in controls that host this one.");
			this.locExtender.SetLocalizationPriority(this._headerLabel, Localization.LocalizationPriority.NotLocalizable);
			this.locExtender.SetLocalizingId(this._headerLabel, "ListPanel._headerLabel");
			this._headerLabel.Location = new System.Drawing.Point(0, 0);
			this._headerLabel.MinimumSize = new System.Drawing.Size(165, 0);
			this._headerLabel.MnemonicGeneratesClick = false;
			this._headerLabel.Name = "_headerLabel";
			this._headerLabel.ShowWindowBackgroudOnTopAndRightEdge = false;
			this._headerLabel.Size = new System.Drawing.Size(168, 23);
			this._headerLabel.TabIndex = 0;
			this._headerLabel.Text = "Items";
			this._headerLabel.Paint += new System.Windows.Forms.PaintEventHandler(this.HandleHeaderPanelPaint);
			// 
			// _buttonColChooser
			// 
			this._buttonColChooser.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this._buttonColChooser.BackColor = System.Drawing.Color.Transparent;
			this._buttonColChooser.CanBeChecked = false;
			this._buttonColChooser.Checked = false;
			this._buttonColChooser.DrawEmpty = false;
			this._buttonColChooser.DrawLeftArrowButton = false;
			this._buttonColChooser.DrawRightArrowButton = false;
			this._buttonColChooser.Font = new System.Drawing.Font("Marlett", 9F);
			this._buttonColChooser.Image = global::SayMore.Properties.Resources.ColumnChooser;
			this.locExtender.SetLocalizableToolTip(this._buttonColChooser, null);
			this.locExtender.SetLocalizationComment(this._buttonColChooser, null);
			this.locExtender.SetLocalizingId(this._buttonColChooser, "xButton1.xButton1");
			this._buttonColChooser.Location = new System.Drawing.Point(146, 2);
			this._buttonColChooser.Name = "_buttonColChooser";
			this._buttonColChooser.Size = new System.Drawing.Size(18, 18);
			this._buttonColChooser.TabIndex = 0;
			this._toolTip.SetToolTip(this._buttonColChooser, "Choose Columns");
			// 
			// locExtender
			// 
			this.locExtender.LocalizationGroup = "Misc. Controls";
			// 
			// ListPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._outerPanel);
			this.DoubleBuffered = true;
			this.locExtender.SetLocalizableToolTip(this, null);
			this.locExtender.SetLocalizationComment(this, null);
			this.locExtender.SetLocalizingId(this, "ListPanel.ListPanel");
			this.MinimumSize = new System.Drawing.Size(125, 0);
			this.Name = "ListPanel";
			this.Size = new System.Drawing.Size(170, 277);
			this._outerPanel.ResumeLayout(false);
			this._outerPanel.PerformLayout();
			this._buttonsFlowLayoutPanel.ResumeLayout(false);
			this._buttonsFlowLayoutPanel.PerformLayout();
			this._headerLabel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.locExtender)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private SilUtils.Controls.SilPanel _outerPanel;
		private HeaderLabel _headerLabel;
		private LocalizationExtender locExtender;
		public System.Windows.Forms.Button _buttonDelete;
		public System.Windows.Forms.Button _buttonNew;
		private System.Windows.Forms.FlowLayoutPanel _buttonsFlowLayoutPanel;
		private SilUtils.Controls.XButton _buttonColChooser;
		private System.Windows.Forms.ToolTip _toolTip;
	}
}
