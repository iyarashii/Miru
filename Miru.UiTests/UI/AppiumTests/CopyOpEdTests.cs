// Copyright iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using OpenQA.Selenium.Appium;

namespace Miru.Tests.UI.AppiumTests
{
    public class CopyOpEdTests : AppiumUiTestBase
    {
        public HashSet<string> GetAnimeTitleWords(string source)
        {
            return source.Split('\n').First().Trim().Split(' ').ToHashSet();
        }

        [Fact]
        public void CopyAnimeTitleValidateToast()
        {
            // Arrange
            Thread.Sleep(2000);
            var animeTitleTextBox = appSession.FindElements(MobileBy.XPath("/Window/DataGrid[6]/DataItem[2]/Custom[1]/Text")).FirstOrDefault();
            Assert.NotNull(animeTitleTextBox);

            // Act
            animeTitleTextBox.Click();

            // Assert
            var toastText = appSession.FindElements(MobileBy.XPath("/Window/Window/Custom/Text")).FirstOrDefault().Text;
            var animeTitleWords = GetAnimeTitleWords(toastText);
            foreach (var word in animeTitleWords)
            {
                Assert.Contains(word, animeTitleTextBox.Text);
            }
        }

        //[Fact]
        public void CheckDialogButtonsAfterRightClick()
        {
            // Arrange
            // wait for grids to load
            Thread.Sleep(2000);
            var animeTitleTextBox = appSession.FindElements(MobileBy.XPath("/Window/DataGrid[6]/DataItem[1]/Custom[1]/Text")).FirstOrDefault();
            Assert.NotNull(animeTitleTextBox);

            // Act
            //appSession.ExecuteScript("windows: click", new Dictionary<string, object>() { { "button", "right" },
            //// not sure why X is 3400 and Y is 700 instead of 1364 and 175 like properties in the found element maybe there is issue with multiple displays (3 screens 1920x1080)
            //    { "x", 3400}, { "y", 700 } });

            // this works only if you dont have screen that is different height - probably WinAppDriver bug
            RightClick(animeTitleTextBox.Id);

            // this should work but throws only pen & touch are supported exception probably WinAppDriver bug
            //new Actions(appSession).ContextClick(animeTitleTextBox).Perform();

            // Assert
            var opButton = appSession.FindElement(MobileBy.Name("OP"));
            var edButton = appSession.FindElement(MobileBy.Name("ED"));
            var cancelButton = appSession.FindElement(MobileBy.Name("Cancel"));
            Assert.NotNull(opButton);
            Assert.NotNull(edButton);
            Assert.NotNull(cancelButton);
            cancelButton.Click();
        }

        //[Fact]
        public void CheckToastAfterOpButtonClicked()
        {
            // Arrange
            var animeTitleTextBox = appSession.FindElements(MobileBy.XPath("/Window/DataGrid[6]/DataItem[2]/Custom[1]/Text")).FirstOrDefault();
            Assert.NotNull(animeTitleTextBox);
            RightClick(animeTitleTextBox.Id);
            var opButton = appSession.FindElement(MobileBy.Name("OP"));
            Assert.NotNull(opButton);
            var opEdDialogContent = appSession.FindElement(MobileBy.XPath("/Window/Text[13]")).Text;

            // Act
            opButton.Click();

            // Assert
            var toastText = appSession.FindElement(MobileBy.XPath("/Window/Window/Custom/Text")).Text;
            var songTitlesAndArtistNames = GetAnimeTitleWords(toastText);
            foreach (var word in songTitlesAndArtistNames)
            {
                Assert.Contains(word, opEdDialogContent);
            }
        }

        //[Fact]
        public void CheckToastAfterEdButtonClicked()
        {
            // Arrange
            var animeTitleTextBox = appSession.FindElements(MobileBy.XPath("/Window/DataGrid[6]/DataItem[2]/Custom[1]/Text")).FirstOrDefault();
            Assert.NotNull(animeTitleTextBox);
            RightClick(animeTitleTextBox.Id);
            var edButton = appSession.FindElement(MobileBy.Name("ED"));
            Assert.NotNull(edButton);
            var opEdDialogContent = appSession.FindElement(MobileBy.XPath("/Window/Text[13]")).Text;

            // Act
            edButton.Click();

            // Assert
            var toastText = appSession.FindElement(MobileBy.XPath("/Window/Window/Custom/Text")).Text;
            var songTitlesAndArtistNames = GetAnimeTitleWords(toastText);
            foreach (var word in songTitlesAndArtistNames)
            {
                Assert.Contains(word, opEdDialogContent);
            }
        }
    }
}
