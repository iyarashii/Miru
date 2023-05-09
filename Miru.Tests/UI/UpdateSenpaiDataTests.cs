// Copyright (c) 2023 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using FlaUI.Core.AutomationElements;
using Xunit;

namespace Miru.Tests.UI
{
    public class UpdateSenpaiDataTests : UiTestBase
    {
        [Fact]
        public void CheckDialogButtonsAfterPress()
        {
            var button = mainWindow.FindFirstChild(cf => cf.ByName("Update Senpai Data"))?.AsButton();
            Assert.NotNull(button);
            button.Invoke();
            var closeButton = mainWindow.FindFirstDescendant(cf => cf.ByName("No"))?.AsButton();
            var primaryButton = mainWindow.FindFirstDescendant(cf => cf.ByName("Yes"))?.AsButton();
            Assert.NotNull(closeButton);
            Assert.NotNull(primaryButton);
            closeButton.Invoke();
        }
    }
}
