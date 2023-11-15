// Copyright iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using System.Diagnostics;
using System.Net;

namespace Miru.Tests.UI.AppiumTests
{
    [Collection("Appium UI Tests")]
    public class AppiumUiTestBase : IDisposable
    {
        protected WindowsDriver appSession;
        public AppiumUiTestBase()
        {
            var cmdsi = new ProcessStartInfo("wt.exe")
            {
                Arguments = "-p \"appium\""
            };
            Process.Start(cmdsi);
            var httpClient = new HttpClient();
            bool serverIsResponding = false;
            int count = 0;
            Thread.Sleep(1000);
            while(serverIsResponding == false)
            {
                try
                {
                    var appiumRequest = httpClient.GetAsync("http://127.0.0.1:4723/status").Result;
                    if (appiumRequest.StatusCode == HttpStatusCode.OK)
                    {
                        serverIsResponding = true;
                    }
                }
                catch (Exception)
                {
                    Thread.Sleep(1000);
                    count++;
                    if (count == 100)
                        throw new Exception("Failed after 100 tries");
                }
            }

            AppiumOptions appCapabilities = new AppiumOptions
            {
                App = Environment.GetEnvironmentVariable("MIRU_PATH", EnvironmentVariableTarget.Machine),
                PlatformName = "Windows",
                AutomationName = "Windows"
            };
            appSession = new WindowsDriver(new Uri("http://127.0.0.1:4723/"), appCapabilities);
        }
        public void Dispose()
        {
            appSession.Close();
            Process.GetProcessesByName("WindowsTerminal").ToList().ForEach(x => x.Kill());
        }

        public void RightClick(string elementId)
        {
            appSession.ExecuteScript(
                "windows: click", new Dictionary<string, object>() 
                { 
                    { "button", "right" },
                    { "elementId", elementId }
                });
        }
    }
}
