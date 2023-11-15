// Copyright iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using OpenQA.Selenium.Appium;

namespace Miru.Tests.UI.AppiumTests
{
    public class UpdateSenpaiDataButtonTests : AppiumUiTestBase
    {
        [Fact]
        public void CheckDialogButtonsAfterPress()
        {
            var button = appSession.FindElement(MobileBy.Name("Update Senpai Data"));
            Assert.NotNull(button);
            button.Click();
            var closeButton = appSession.FindElement(MobileBy.Name("No"));
            var primaryButton = appSession.FindElement(MobileBy.Name("Yes"));
            Assert.NotNull(closeButton);
            Assert.NotNull(primaryButton);
            closeButton.Click();
        }
    }
}
