using System;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using SayMore.UI.ProjectWindow;

namespace SayMoreTests.UI.ProjectWindow
{
	public class ReleaseNotesCommandTests
	{
		[Test]
		public void Execute_LaunchAndClose_DoesNotCrash()
		{
			ModalFormTester tester = new ModalFormTester();
			var buttonTester = new ButtonTester("_close");
			tester.ExpectModal("ShowHtmlDialog", () => buttonTester.FireEvent("Click"));
			var cmd = new ReleaseNotesCommand();
			cmd.Execute();
			tester.Verify();
		}

	}
}