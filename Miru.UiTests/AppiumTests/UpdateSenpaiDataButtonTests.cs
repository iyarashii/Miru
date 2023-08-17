// Copyright iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using Xunit;

namespace Miru.Tests.UI.AppiumTests
{
    public class UpdateSenpaiDataButtonTests : AppiumUiTestBase
    {
        [Fact]
        public void CheckDialogButtonsAfterPress()
        {
            var button = appSession.FindElementByName("Update Senpai Data");
            Assert.NotNull(button);
            button.Click();
            var closeButton = appSession.FindElementByName("No");
            var primaryButton = appSession.FindElementByName("Yes");
            Assert.NotNull(closeButton);
            Assert.NotNull(primaryButton);
            closeButton.Click();
        }
    }
}
