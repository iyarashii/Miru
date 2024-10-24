// Copyright iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using OpenQA.Selenium;

namespace Miru.UiTests.UI.FlaUIWebDriverTests
{
    public class ClearCacheTests : FlaUIWebDriverTestBase
    {
        [Fact]
        public void CheckClearCacheDialogButtons()
        {
            var button = driver.FindElement(By.Name("Clear Cache"));
            Assert.NotNull(button);
            button.Click();
            var closeButton = driver.FindElement(By.Name("No"));
            var primaryButton = driver.FindElement(By.Name("Yes"));
            Assert.NotNull(closeButton);
            Assert.NotNull(primaryButton);
            closeButton.Click();
        }
    }
}
