using OpenQA.Selenium;

namespace Miru.UiTests.UI.FlaUIWebDriverTests
{
    public class UpdateSenpaiDataButtonTests : FlaUIWebDriverTestBase
    {
        [Fact]
        public void CheckDialogButtonsAfterPress()
        {
            var button = driver.FindElement(By.Name("Update Senpai Data"));
            Assert.NotNull(button);
            button.Click();
            var closeButton = driver.FindElement(By.Name("No"));
            var primaryButton = driver.FindElement(By.Name("Yes"));
            Assert.NotNull(closeButton);
            Assert.NotNull(primaryButton);
            closeButton.Click();
        }
    }
}
