// Copyright (c) 2023 iyarashii @ https://github.com/iyarashii 
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
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Miru.Tests.UI
{
    public class UpdateSenpaiDataTests
    {
        [Fact]
        public void CheckDialogButtonsAfterPress()
        {
            var app = FlaUI.Core.Application.Launch("G:\\repos\\Miru\\Miru\\bin\\Debug\\app.publish\\Miru.exe");
            using (var automation = new UIA2Automation())
            {
                var window = app.GetMainWindow(automation);
                var conFac = new ConditionFactory(new UIA2PropertyLibrary());
                var button = window.FindFirstChild(conFac.ByName("Update Senpai Data"))?.AsButton();
                Assert.NotNull(button);
                button.Invoke();
                var closeButton = window.FindFirstDescendant(cf => cf.ByName("No"))?.AsButton();
                var primaryButton = window.FindFirstDescendant(cf => cf.ByName("Yes"))?.AsButton();
                Assert.NotNull(closeButton);
                Assert.NotNull(primaryButton);
                closeButton.Invoke();
            }
            app.Close();
        }
    }
}
