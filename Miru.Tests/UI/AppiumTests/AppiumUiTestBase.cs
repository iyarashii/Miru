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

namespace Miru.Tests.UI.AppiumTests
{
    [Collection("Appium UI Tests")]
    public class AppiumUiTestBase : IDisposable
    {
        protected WindowsDriver<WindowsElement> appSession;
        //private static AppiumLocalService service;
        private Process server;
        public AppiumUiTestBase()
        {
            //var argCollector = new OptionCollector()
            //            .AddArguments(new System.Collections.Generic.KeyValuePair<string, string>("", ""));
            //var builder = new AppiumServiceBuilder().WithAppiumJS().WithArguments(argCollector);
            //var builder = new AppiumServiceBuilder().WithAppiumJS(new System.IO.FileInfo("C:\\Users\\Iyarashii\\AppData\\Roaming\\nvm\\v20.5.1\\appium.cmd"));
            //service = builder.Build();
            //service.Start();
            //server = Process.Start("C:\\Users\\Iyarashii\\AppData\\Roaming\\nvm\\v20.5.1\\appium.cmd");
            var cmdsi = new ProcessStartInfo("pwsh.exe");
            cmdsi.Arguments = "-noexit -command \"appium\"";
            //cmdsi.Arguments = "appium";
            //cmdsi.RedirectStandardOutput = true;
            //cmdsi.RedirectStandardError = true;
            //cmdsi.UseShellExecute = false;
            //cmdsi.RedirectStandardOutput = true;
            //cmdsi.WindowStyle = ProcessWindowStyle.Maximized;
            server = Process.Start(cmdsi);
            Task.Delay(5000).Wait();
            var output = string.Empty;
            //while(output.Contains("[Appium] No plugins have been installed. Use the \"appium plugin\" command to install the one(s) you want to use.") == false)
            //{
            //    try
            //    {
            //        Task.Delay(1000).Wait();
            //        output = server.StandardOutput.ReadToEnd();
            //    }
            //    catch (Exception ex)
            //    {
            //    }
            //}
            AppiumOptions appCapabilities = new AppiumOptions();
            appCapabilities.AddAdditionalCapability("appium:app", "G:\\repos\\Miru\\Miru\\bin\\Debug\\app.publish\\Miru.exe");
            appCapabilities.AddAdditionalCapability("platformName", "Windows");
            appCapabilities.AddAdditionalCapability("appium:automationName", "Windows");
            appSession = new WindowsDriver<WindowsElement>(new Uri("http://127.0.0.1:4723/"), appCapabilities);
        }
        public void Dispose()
        {
            appSession.Close();
            server.CloseMainWindow();
        }
    }
}
