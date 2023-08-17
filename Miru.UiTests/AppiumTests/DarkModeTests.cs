// Copyright iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using Xunit;

namespace Miru.Tests.UI.AppiumTests
{
    public class DarkModeTests : AppiumUiTestBase
    {
        [Fact]
        public async void TurnOffDarkMode()
        {
            //await SetupServerAsync();
            var darkModeSwitch = appSession.FindElementByClassName("ToggleSwitch");
            Assert.NotNull(darkModeSwitch);
            Assert.Contains("On", darkModeSwitch.Text);
            darkModeSwitch.Click();
            Assert.Contains("Off", darkModeSwitch.Text);
        }
    }
}
