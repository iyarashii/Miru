// Copyright iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using System;
using Xunit;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using System.Diagnostics;
using FlaUI.UIA3;
using FlaUI.Core.Tools;
using FlaUI.Core.AutomationElements;
using System.Collections.Generic;

namespace Miru.Tests.UI.AppiumTests
{
    [Collection("Appium UI Tests")]
    public class AppiumUiTestBase : IDisposable
    {
        protected WindowsDriver appSession;
        private readonly Process appiumServerProcess;
        private readonly Window mainWindow;
        public AppiumUiTestBase()
        {
            var cmdsi = new ProcessStartInfo("pwsh.exe")
            {
                Arguments = "-noexit -command \"appium\""
            };
            appiumServerProcess = Process.Start(cmdsi);
            var flauiSP = FlaUI.Core.Application.Attach(appiumServerProcess);
            mainWindow = flauiSP.GetMainWindow(new UIA3Automation());
            var textArea = mainWindow.FindFirstDescendant("Text Area");
            Retry.WhileNull(() => textArea.Patterns.Text.Pattern.DocumentRange
                .FindText("No plugins have been installed.", false, true), 
                interval: TimeSpan.FromSeconds(1), 
                timeout: TimeSpan.FromMinutes(1));
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
            mainWindow.Close();
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
