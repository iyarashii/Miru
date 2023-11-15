// Copyright iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using System;
using Xunit;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using System.Diagnostics;
using FlaUI.UIA2;
using FlaUI.Core.Tools;
using FlaUI.Core.AutomationElements;
using System.Collections.Generic;
using FlaUI.UIA3;
using Namotion.Reflection;
using OpenQA.Selenium;
using System.Net;
using System.Net.Http.Json;

namespace Miru.Tests.UI.AppiumTests
{
    [Collection("Appium UI Tests")]
    public class AppiumUiTestBase : IDisposable
    {
        protected WindowsDriver appSession;
        private readonly Process appiumServerProcess;
        //private readonly Window mainWindow;
        public AppiumUiTestBase()
        {
            var cmdsi = new ProcessStartInfo("wt.exe")
            {
                Arguments = "-p \"appium\""
            };
            // commented out code is for powershell without windowsterminal.exe installed
            //var cmdsi = new ProcessStartInfo("pwsh.exe")
            //{
            //    Arguments = "-noexit -command \"appium\""
            //};
            appiumServerProcess = Process.Start(cmdsi);
            //var flauiSP = FlaUI.Core.Application.Attach(appiumServerProcess);
            //var flauiSP = Retry.WhileException(() => FlaUI.Core.Application.Attach("WindowsTerminal.exe"),
            //   interval: TimeSpan.FromSeconds(1),
            //   timeout: TimeSpan.FromMinutes(1)).Result;
            //mainWindow = flauiSP.GetMainWindow(new UIA3Automation());
            //var textArea = mainWindow.FindFirstDescendant(x => x.ByName("appium"));
            //textArea.busy();
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
                //var app = Process.Start(Environment.GetEnvironmentVariable("MIRU_PATH", EnvironmentVariableTarget.Machine));
                //Wait.UntilInputIsProcessed(TimeSpan.FromSeconds(5));
                App = Environment.GetEnvironmentVariable("MIRU_PATH", EnvironmentVariableTarget.Machine),
                //appCapabilities.AddAdditionalAppiumOption("appium:appTopLevelWindow", app.MainWindowHandle.ToString("x"));
                PlatformName = "Windows",
                AutomationName = "Windows"
            };
            // old way to add appium options below
            //appCapabilities.AddAdditionalAppiumOption("platformName", "Windows");
            //appCapabilities.AddAdditionalAppiumOption("appium:automationName", "Windows");
            appSession = new WindowsDriver(new Uri("http://127.0.0.1:4723/"), appCapabilities);
        }
        public void Dispose()
        {
            appSession.Close();
            Process.GetProcessesByName("WindowsTerminal").ToList().ForEach(x => x.Kill());
            //mainWindow.Close();
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
