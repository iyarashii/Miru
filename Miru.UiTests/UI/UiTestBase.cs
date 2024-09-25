// Copyright iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using FlaUI.Core.AutomationElements;
using FlaUI.Core.Input;
using FlaUI.UIA2;

namespace Miru.Tests.UI
{
    [Collection("UI Tests")]
    public class UiTestBase : IDisposable
    {
        protected readonly UIA2Automation automation;
        protected readonly FlaUI.Core.Application app;
        protected readonly Window mainWindow;
        public UiTestBase()
        {
            app = FlaUI.Core.Application.Launch(Environment.GetEnvironmentVariable("MIRU_PATH", EnvironmentVariableTarget.Machine));
            automation = new UIA2Automation();
            // give time to load DataGrids
            Wait.UntilInputIsProcessed(TimeSpan.FromSeconds(5));
            mainWindow = app.GetMainWindow(automation);
        }
        public void Dispose()
        {
            automation.Dispose();
            app.Close();
        }
    }
}
