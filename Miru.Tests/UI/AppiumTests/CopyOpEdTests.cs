// Copyright iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

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
            Thread.Sleep(5000);
            var animeTitleTextBox = appSession.FindElementsByXPath("/Window/DataGrid[6]/DataItem[1]/Custom[1]/Text").FirstOrDefault();
            Assert.NotNull(animeTitleTextBox);

            // Act
            //Actions actions = new Actions(appSession);
            //actions.ContextClick(animeTitleTextBox).Perform();
            //appSession.Mouse.ContextClick(animeTitleTextBox.Coordinates);
            animeTitleTextBox.Execute("RightClick");

            //Assert
            var opButton = appSession.FindElementByName("OP");
            var edButton = appSession.FindElementByName("ED");
            var cancelButton = appSession.FindElementByName("Cancel");
            Assert.NotNull(opButton);
            Assert.NotNull(edButton);
            Assert.NotNull(cancelButton);
            cancelButton.Click();
        }
    }
}
