// Copyright iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using FlaUI.Core.AutomationElements;
using FlaUI.Core.Input;

namespace Miru.Tests.UI
{
    public class DarkModeTests : UiTestBase
    {
        [Fact]
        public void TurnOffDarkMode()
        {
            // Arrange
            var darkModeSwitchThumb = mainWindow.FindAllByXPath("/Button[5]/Thumb").FirstOrDefault();
            Assert.NotNull(darkModeSwitchThumb);

            // Act
            darkModeSwitchThumb.Click();

            // Assert 
            Wait.UntilInputIsProcessed(new TimeSpan(0, 0, 2));
            var darkModeButton = mainWindow.FindAllByXPath("/Button[5]").FirstOrDefault();
            Assert.False(darkModeButton.AsToggleButton().IsToggled);
        }

        [Fact]
        public void TurnOnDarkMode()
        {
            // Arrange
            var darkModeSwitchThumb = mainWindow.FindAllByXPath("/Button[5]/Thumb").FirstOrDefault();
            Assert.NotNull(darkModeSwitchThumb);

            // Act
            darkModeSwitchThumb.Click();
            darkModeSwitchThumb.Click();

            // Assert 
            Wait.UntilInputIsProcessed(new TimeSpan(0, 0, 2));
            var darkModeButton = mainWindow.FindAllByXPath("/Button[5]").FirstOrDefault();
            Assert.True(darkModeButton.AsToggleButton().IsToggled);
        }
    }
}
