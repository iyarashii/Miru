﻿// Copyright iyarashii @ https://github.com/iyarashii 
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

namespace Miru.Tests.UI.AppiumTests
{
    [Collection("Appium UI Tests")]
    public class AppiumUiTestBase : IDisposable
    {
        protected WindowsDriver<WindowsElement> appSession;
        private Process appiumServerProcess;
        private Window mainWindow;
        public AppiumUiTestBase()
        {
            var cmdsi = new ProcessStartInfo("pwsh.exe");
            cmdsi.Arguments = "-noexit -command \"appium\"";
            appiumServerProcess = Process.Start(cmdsi);
            var flauiSP = FlaUI.Core.Application.Attach(appiumServerProcess);
            mainWindow = flauiSP.GetMainWindow(new UIA3Automation());
            var textArea = mainWindow.FindFirstDescendant("Text Area");
            Retry.WhileNull(() => textArea.Patterns.Text.Pattern.DocumentRange.FindText("No plugins have been installed.", false, true), interval: TimeSpan.FromSeconds(1), timeout: TimeSpan.FromMinutes(1));
            AppiumOptions appCapabilities = new AppiumOptions();
            appCapabilities.AddAdditionalCapability("appium:app", "G:\\repos\\Miru\\Miru\\bin\\Debug\\app.publish\\Miru.exe");
            appCapabilities.AddAdditionalCapability("platformName", "Windows");
            appCapabilities.AddAdditionalCapability("appium:automationName", "Windows");
            appSession = new WindowsDriver<WindowsElement>(new Uri("http://127.0.0.1:4723/"), appCapabilities);
        }
        public void Dispose()
        {
            appSession.Close();
            mainWindow.Close();
        }
    }
}
