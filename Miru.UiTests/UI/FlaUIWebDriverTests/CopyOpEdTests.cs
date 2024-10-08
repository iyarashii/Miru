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
    }
}
