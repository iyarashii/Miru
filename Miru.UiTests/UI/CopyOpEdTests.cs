// Copyright iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using FlaUI.Core.AutomationElements;
using FlaUI.Core.Input;

namespace Miru.Tests.UI
{
    public class CopyOpEdTests : UiTestBase
    {
        public HashSet<string> GetAnimeTitleWords(string source)
        {
            return source.Split('\n').First().Trim().Split(' ').ToHashSet();
        }

        [Fact]
        public void CopyAnimeTitleValidateToast()
        {
            //Arrange
            var animeTitleTextBox = mainWindow.FindAllByXPath("/DataGrid[6]/DataItem[2]/Custom[1]/Text").FirstOrDefault();
            Assert.NotNull(animeTitleTextBox);

            // Act
            animeTitleTextBox.Click();

            // Assert
            Wait.UntilInputIsProcessed(TimeSpan.FromSeconds(2));
            var toast = mainWindow.FindAllByXPath("/Window/Custom/Text").FirstOrDefault();
            var animeTitleWords = GetAnimeTitleWords(toast.Name);
            Assert.NotNull(toast);
            foreach (var word in animeTitleWords)
            {
                Assert.Contains(word, animeTitleTextBox.Name);
            }
        }

        [Fact]
        public void CheckDialogButtonsAfterRightClick()
        {
            //Arrange
            var animeTitleTextBox = mainWindow.FindAllByXPath("/DataGrid[6]/DataItem[2]/Custom[1]/Text").FirstOrDefault();
            Assert.NotNull(animeTitleTextBox);

            // Act
            animeTitleTextBox.RightClick();

            //Assert
            var opButton = mainWindow.FindFirstDescendant(cf => cf.ByName("OP"))?.AsButton();
            var edButton = mainWindow.FindFirstDescendant(cf => cf.ByName("ED"))?.AsButton();
            var cancelButton = mainWindow.FindFirstDescendant(cf => cf.ByName("Cancel"))?.AsButton();
            Assert.NotNull(opButton);
            Assert.NotNull(edButton);
            Assert.NotNull(cancelButton);
            cancelButton.Invoke();
        }

        [Fact]
        public void CheckToastAfterOpButtonClicked()
        {
            // Arrange
            var animeTitleTextBox = mainWindow.FindAllByXPath("/DataGrid[6]/DataItem[2]/Custom[1]/Text").FirstOrDefault();
            Assert.NotNull(animeTitleTextBox);
            animeTitleTextBox.RightClick();
            var opButton = mainWindow.FindFirstDescendant(cf => cf.ByName("OP"))?.AsButton();
            Assert.NotNull(opButton);
            var opEdDialogContent = mainWindow.FindFirstDescendant("OpEdTextBox").AsTextBox().Text;
            
            // Act
            opButton.Invoke();

            // Assert
            Wait.UntilInputIsProcessed(TimeSpan.FromSeconds(2));
            var toast = mainWindow.FindAllByXPath("/Window/Custom/Text").FirstOrDefault();
            Assert.NotNull(toast);
            var songTitlesAndArtistNames = GetAnimeTitleWords(toast.Name);
            foreach (var word in songTitlesAndArtistNames)
            {
                Assert.Contains(word, opEdDialogContent);
            }
        }

        [Fact]
        public void CheckToastAfterEdButtonClicked()
        {
            // Arrange
            var animeTitleTextBox = mainWindow.FindAllByXPath("/DataGrid[6]/DataItem[2]/Custom[1]/Text").FirstOrDefault();
            Assert.NotNull(animeTitleTextBox);
            animeTitleTextBox.RightClick();
            var edButton = mainWindow.FindFirstDescendant(cf => cf.ByName("ED"))?.AsButton();
            Assert.NotNull(edButton);
            var opEdDialogContent = mainWindow.FindFirstDescendant("OpEdTextBox").AsTextBox().Text;

            // Act
            edButton.Invoke();

            // Assert
            Wait.UntilInputIsProcessed(TimeSpan.FromSeconds(2));
            var toast = mainWindow.FindAllByXPath("/Window/Custom/Text").FirstOrDefault();
            Assert.NotNull(toast);
            var songTitlesAndArtistNames = GetAnimeTitleWords(toast.Name);
            foreach (var word in songTitlesAndArtistNames)
            {
                Assert.Contains(word, opEdDialogContent);
            }
        }
    }
}
