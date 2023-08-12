// Copyright iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using System;
using Xunit;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;

namespace Miru.Tests.UI.AppiumTests
{
    [Collection("Appium UI Tests")]
    public class AppiumUiTestBase : IDisposable
    {
        protected WindowsDriver<WindowsElement> appSession;
        public AppiumUiTestBase()
        {
            AppiumOptions appCapabilities = new AppiumOptions();
            appCapabilities.AddAdditionalCapability("appium:app", "G:\\repos\\Miru\\Miru\\bin\\Debug\\app.publish\\Miru.exe");
            appCapabilities.AddAdditionalCapability("platformName", "Windows");
            appCapabilities.AddAdditionalCapability("appium:automationName", "Windows");
            appSession = new WindowsDriver<WindowsElement>(new Uri("http://127.0.0.1:4723/"), appCapabilities);
        }
        public void Dispose()
        {
            appSession.Close();
        }

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
