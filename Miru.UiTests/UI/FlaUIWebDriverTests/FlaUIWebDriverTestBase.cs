// Copyright iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using OpenQA.Selenium.Appium.Windows;

namespace Miru.UiTests.UI.FlaUIWebDriverTests
{
    public class FlaUIWebDriverTestBase
    {
        protected WindowsDriver driver;
        public FlaUIWebDriverTestBase()
        {
            var pathToApp = Environment.GetEnvironmentVariable("MIRU_PATH", EnvironmentVariableTarget.Machine);
            driver = new WindowsDriver(new Uri("http://localhost:4723"), FlaUIDriverOptions.ForApp(pathToApp));
        }
    }
}
