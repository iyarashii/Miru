// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.Core.Input;
using FlaUI.UIA2;
using System;
using System.Linq;
using Xunit;

namespace Miru.Tests.UI
{
    public class CopyOpEdTests : IDisposable
    {
        UIA2Automation automation;
        FlaUI.Core.Application app;
        Window mainWindow;
        public CopyOpEdTests()
        {
            app = FlaUI.Core.Application.Launch("G:\\repos\\Miru\\Miru\\bin\\Debug\\app.publish\\Miru.exe");
            automation = new UIA2Automation();
            // give time to load DataGrids
            Wait.UntilInputIsProcessed(new TimeSpan(0, 0, 5));
            mainWindow = app.GetMainWindow(automation);
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

            // Act
            opButton.Invoke();

            // Assert
            Wait.UntilInputIsProcessed(new TimeSpan(0, 0, 2));
            var toast = mainWindow.FindAllByXPath("/Window/Custom/Text").FirstOrDefault();
            Assert.NotNull(toast);
            Assert.Equal("'Karei One Turn (華麗ワンターン) TrySail\n' copied to the clipboard!", toast.Name);
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

            // Act
            edButton.Invoke();

            // Assert
            Wait.UntilInputIsProcessed(new TimeSpan(0, 0, 2));
            var toast = mainWindow.FindAllByXPath("/Window/Custom/Text").FirstOrDefault();
            Assert.NotNull(toast);
            Assert.Equal("'Karei One Turn (華麗ワンターン) TrySail\nMukyuu Platonic (無窮プラトニック) VALIS\n' copied to the clipboard!", toast.Name);
        }

        public void Dispose()
        {
            automation.Dispose();
            app.Close();
        }
    }
}
