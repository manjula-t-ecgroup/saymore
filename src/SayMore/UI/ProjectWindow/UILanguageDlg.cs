using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Localization;

namespace SayMore.UI.ProjectWindow
{
	public partial class UILanguageDlg : Form
	{
		public string UILanguage { get; private set; }

		/// ------------------------------------------------------------------------------------
		public UILanguageDlg()
		{
			InitializeComponent();

			_labelLanguage.Font = SystemFonts.IconTitleFont;
			_linkIWantToLocalize.Font = SystemFonts.IconTitleFont;
			_linkHelpOnLocalizing.Font = SystemFonts.IconTitleFont;
			_comboUILanguage.Font = SystemFonts.IconTitleFont;
			_comboUILanguage.SelectedItem = CultureInfo.GetCultureInfo(LocalizationManager.UILanguageId);
			DialogResult = DialogResult.Cancel;
		}

		/// ------------------------------------------------------------------------------------
		private void HandleIWantToLocalizeLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			LocalizationManager.ShowLocalizationDialogBox();
			_comboUILanguage.RefreshList();
		}

		/// ------------------------------------------------------------------------------------
		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			if (DialogResult == DialogResult.OK)
				UILanguage = ((CultureInfo)_comboUILanguage.SelectedItem).TwoLetterISOLanguageName;

			base.OnFormClosing(e);
		}
	}
}