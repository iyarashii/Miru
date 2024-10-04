// Copyright iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using OpenQA.Selenium;

namespace Miru.UiTests.UI.FlaUIWebDriverTests
{
    public class DarkModeTests : FlaUIWebDriverTestBase
    {
        [Fact]
        public void TurnOffDarkMode()
        {
            var darkModeSwitch = driver.FindElement(By.ClassName("ToggleSwitch"));
            Assert.NotNull(darkModeSwitch);
            Assert.Contains("On", darkModeSwitch.Text);
            darkModeSwitch.Click();
            Assert.Contains("Off", darkModeSwitch.Text);
        }
    }
}
