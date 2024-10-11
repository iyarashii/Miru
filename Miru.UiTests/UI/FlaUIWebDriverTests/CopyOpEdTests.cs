// Copyright iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using OpenQA.Selenium;

namespace Miru.UiTests.UI.FlaUIWebDriverTests
{
    public class CopyOpEdTests : FlaUIWebDriverTestBase
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
            var animeTitleTextBox = driver.FindElements(By.XPath("/DataGrid[6]/DataItem[2]/Custom[1]/Text")).FirstOrDefault();
            Assert.NotNull(animeTitleTextBox);

            // Act
            animeTitleTextBox.Click();

            // Assert
            var toastText = driver.FindElements(By.XPath("/Window/Custom/Text")).FirstOrDefault().Text;
            var animeTitleWords = GetAnimeTitleWords(toastText);
            foreach (var word in animeTitleWords)
            {
                Assert.Contains(word, animeTitleTextBox.Text);
            }
        }

        [Fact]
        public void CheckDialogButtonsAfterRightClick()
        {
            // Arrange
            // wait for grids to load
            Thread.Sleep(2000);
            var animeTitleTextBox = driver.FindElements(By.XPath("/DataGrid[6]/DataItem[1]/Custom[1]/Text")).FirstOrDefault();
            Assert.NotNull(animeTitleTextBox);

            // Act
            RightClick(animeTitleTextBox.Id);

            // Assert
            var opButton = driver.FindElement(By.Name("OP"));
            var edButton = driver.FindElement(By.Name("ED"));
            var cancelButton = driver.FindElement(By.Name("Cancel"));
            Assert.NotNull(opButton);
            Assert.NotNull(edButton);
            Assert.NotNull(cancelButton);
            cancelButton.Click();
        }

        [Fact]
        public void CheckToastAfterOpButtonClicked()
        {
            // Arrange
            var animeTitleTextBox = driver.FindElements(By.XPath("/DataGrid[6]/DataItem[2]/Custom[1]/Text")).FirstOrDefault();
            Assert.NotNull(animeTitleTextBox);
            RightClick(animeTitleTextBox.Id);
            var opButton = driver.FindElement(By.Name("OP"));
            Assert.NotNull(opButton);
            var opEdDialogContent = driver.FindElement(By.Id("OpEdTextBox")).Text;

            // Act
            opButton.Click();
            Thread.Sleep(1000);

            // Assert
            var toastText = driver.FindElement(By.XPath("/Window/Custom/Text")).Text;
            var songTitlesAndArtistNames = GetAnimeTitleWords(toastText);
            foreach (var word in songTitlesAndArtistNames)
            {
                Assert.Contains(word, opEdDialogContent);
            }
        }
    }
}
