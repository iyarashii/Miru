// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.Core.Input;
using FlaUI.UIA2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Miru.Tests.UI
{
    public class CopyOpEdTests
    {
        [Fact]
        public void CheckDialogButtonsAfterRightClick()
        {
            var app = FlaUI.Core.Application.Launch("G:\\repos\\Miru\\Miru\\bin\\Debug\\app.publish\\Miru.exe");
            using (var automation = new UIA2Automation())
            {
                // give time to load DataGrids
                Wait.UntilInputIsProcessed(new TimeSpan(0, 0, 5));
                var window = app.GetMainWindow(automation);
                var conFac = new ConditionFactory(new UIA2PropertyLibrary());
                var animeTitleTextBox = window.FindAllByXPath("/DataGrid[6]/DataItem[2]/Custom[1]/Text").FirstOrDefault();
                Assert.NotNull(animeTitleTextBox);
                animeTitleTextBox.RightClick();
                var opButton = window.FindFirstDescendant(cf => cf.ByName("OP"))?.AsButton();
                var edButton = window.FindFirstDescendant(cf => cf.ByName("ED"))?.AsButton();
                var cancelButton = window.FindFirstDescendant(cf => cf.ByName("Cancel"))?.AsButton();
                Assert.NotNull(opButton);
                Assert.NotNull(edButton);
                Assert.NotNull(cancelButton);
                cancelButton.Invoke();
            }
            app.Close();
        }
    }
}
