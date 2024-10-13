// Copyright iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using OpenQA.Selenium.Appium.Windows;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Miru.UiTests.UI.FlaUIWebDriverTests
{
    [Collection("FlaUI WebDriver UI Tests")]
    public class FlaUIWebDriverTestBase : IDisposable
    {
        protected WindowsDriver driver;
        private Process flaUiWebDriverProcess;
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        public FlaUIWebDriverTestBase()
        {
            var pathToFlaUIWebDriver = Environment.GetEnvironmentVariable("FLAUI_WEB_DRIVER_PATH", EnvironmentVariableTarget.Machine);
            var pathToApp = Environment.GetEnvironmentVariable("MIRU_PATH", EnvironmentVariableTarget.Machine);
            flaUiWebDriverProcess = Process.Start(pathToFlaUIWebDriver);
            driver = new WindowsDriver(new Uri("http://localhost:5000"), FlaUIDriverOptions.ForApp(pathToApp));
            // Wait for the application to start and get the main window handle
            var appProcess = Process.GetProcessesByName("Miru").FirstOrDefault();
            if (appProcess != null)
            {
                SetForegroundWindow(appProcess.MainWindowHandle);
            }
        }

        public void RightClick(string elementId)
        {
            driver.ExecuteScript(
                "windows: click", new Dictionary<string, object>()
                {
                    { "button", "right" },
                    { "elementId", elementId }
                });
        }

        public void Dispose()
        {
            driver.Close();
            flaUiWebDriverProcess.Kill();
        }
    }
}
