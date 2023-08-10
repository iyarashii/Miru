// Copyright iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using System;
using Xunit;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;

namespace Miru.Tests.UI.AppiumTests
{
    public class AppiumUiTestBase
    {
        private WindowsDriver<WindowsElement> appSession;
        [Fact]
        public void TurnOffDarkMode()
        {
            AppiumOptions appCapabilities = new AppiumOptions();
            appCapabilities.AddAdditionalCapability("appium:app", "G:\\repos\\Miru\\Miru\\bin\\Debug\\app.publish\\Miru.exe");
            appCapabilities.AddAdditionalCapability("platformName", "Windows");
            appCapabilities.AddAdditionalCapability("appium:automationName", "Windows");
            appSession = new WindowsDriver<WindowsElement>(new Uri("http://127.0.0.1:4723/"), appCapabilities);
            //var darkModeSwitch = appSession.FindElementByAccessibilityId("SwitchThumb");
            var darkModeSwitch = appSession.FindElementByClassName("ToggleSwitch");
            Assert.NotNull(darkModeSwitch);
            Assert.Contains("On", darkModeSwitch.Text);
            darkModeSwitch.Click();
            Assert.Contains("Off", darkModeSwitch.Text);
            appSession.Close();
        }
    }
}
