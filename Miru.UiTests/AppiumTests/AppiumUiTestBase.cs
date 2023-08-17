// Copyright iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using System;
using Xunit;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Appium.Service;
using OpenQA.Selenium.Appium.Service.Options;
using System.Diagnostics;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Collections.ObjectModel;
using System.Security.Principal;

namespace Miru.Tests.UI.AppiumTests
{
    [Collection("Appium UI Tests")]
    public class AppiumUiTestBase : IDisposable
    {
        protected WindowsDriver<WindowsElement> appSession;
        public AppiumUiTestBase()
        {
            var data = new List<string>();

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $@"-noexit -command ""appium""",
                RedirectStandardOutput = true,
                UseShellExecute = false,
            };

            Process process = new Process { StartInfo = psi };

            process.Start();

            Thread.Sleep(5000);
            while (true)
            {
                var line = process.StandardOutput.ReadLine();
                if (line.Contains("No plugins have been installed."))
                    break;
            }
            //    if (output.Contains("\u001b[35m[Appium]\u001b[39m No plugins have been installed. Use the \"appium plugin\" command to install the one(s) you want to use."))
            AppiumOptions appCapabilities = new AppiumOptions();
            appCapabilities.AddAdditionalCapability("appium:app", "G:\\repos\\Miru\\Miru\\bin\\Debug\\app.publish\\Miru.exe");
            appCapabilities.AddAdditionalCapability("platformName", "Windows");
            appCapabilities.AddAdditionalCapability("appium:automationName", "Windows");
            appSession = new WindowsDriver<WindowsElement>(new Uri("http://127.0.0.1:4723/"), appCapabilities); // does not work probably because redirect standard output?????
            while (true)
            {
                var line = process.StandardOutput.ReadLine();
                if (line.Contains("No plugins have been installed."))
                    break;
            }
        }

        public async Task SetupServerAsync()
        {
            var ps = PowerShell.Create().AddCommand("appium");
            var invStart = ps.BeginInvoke();
            //Collection<PSObject> results = null;
            //var psThread = new Thread(new ParameterizedThreadStart(x => ps.Invoke()));
            //results = ps.Invoke();
            //try
            //{
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            //finally 
            //{ 
            //    //rs.Close();
            //    //Thread.CurrentPrincipal = old;
            //}
            //results = ps.EndInvoke();
            //psThread.Start();
            //psThread.Join();
            var results = ps.EndInvoke(invStart);
            bool appiumSetup = results.Select(x => x.BaseObject.ToString()).Contains("No plugins have been installed.");
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
    }
}
