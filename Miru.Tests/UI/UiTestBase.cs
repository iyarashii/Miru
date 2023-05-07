// Copyright iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using FlaUI.Core.AutomationElements;
using FlaUI.Core.Input;
using FlaUI.UIA2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miru.Tests.UI
{
    public class UiTestBase : IDisposable
    {
        protected readonly UIA2Automation automation;
        protected readonly FlaUI.Core.Application app;
        protected readonly Window mainWindow;
        public UiTestBase()
        {
            app = FlaUI.Core.Application.Launch("G:\\repos\\Miru\\Miru\\bin\\Debug\\app.publish\\Miru.exe");
            automation = new UIA2Automation();
            // give time to load DataGrids
            Wait.UntilInputIsProcessed(new TimeSpan(0, 0, 5));
            mainWindow = app.GetMainWindow(automation);
        }
        public void Dispose()
        {
            automation.Dispose();
            app.Close();
        }
    }
}
