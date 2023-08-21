// Copyright iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using Xunit;

namespace Miru.Tests.UI.AppiumTests
{
    public class CopyOpEdTests : AppiumUiTestBase
    {
        public HashSet<string> GetDistinctWordsBetweenSingleQuotes(string source)
        {
            return source.Substring(source.IndexOf("'") + 1, source.LastIndexOf("'") - 1).Replace('\n', ' ').Trim().Split(' ').ToHashSet();
        }

        [Fact]
        public void CheckDialogButtonsAfterRightClick()
        {
            //Arrange
            // wait for grids to load
            Thread.Sleep(2000);
            var animeTitleTextBox = appSession.FindElements(MobileBy.XPath("/Window/DataGrid[6]/DataItem[1]/Custom[1]/Text")).FirstOrDefault();
            Assert.NotNull(animeTitleTextBox);

            // Act
            //Actions actions = new Actions(appSession);
            //actions.SendKeys(animeTitleTextBox, Keys.Shift + Keys.F10).Perform();
            appSession.ExecuteScript("windows:click", new Dictionary<string, object>() { { "button", "right" },
                { "elementId", animeTitleTextBox.Id }/*{ "x", 1364}, { "y", 175 } */});
            //animeTitleTextBox.SendKeys(Keys.Shift,  Keys.F10); {X = 1364 Y = 175}
            //appSession.Mouse.ContextClick(animeTitleTextBox.Coordinates);s
            //animeTitleTextBox.("RightClick");
            //animeTitleTextBox.Execute("windows:click", new Dictionary<string, object>() { { "button", "right" },
            //    { "x", 1364}, { "y",175 } });
            //Assert
            var opButton = appSession.FindElement(MobileBy.Name("OP"));
            var edButton = appSession.FindElement(MobileBy.Name("ED"));
            var cancelButton = appSession.FindElement(MobileBy.Name("Cancel"));
            Assert.NotNull(opButton);
            Assert.NotNull(edButton);
            Assert.NotNull(cancelButton);
            cancelButton.Click();
        }
    }
}
