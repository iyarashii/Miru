using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Autofac.Extras.Moq;
using Miru.ViewModels;
using ModernWpf.Controls;

namespace Miru.Tests.ViewModelsTests
{
    public class SimpleContentDialogTests
    {
        [Fact]
        public void Config_ValidCall()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var cls = mock.Create<SimpleContentDialog>();
                var expectedTitle = "2137";

                // Act
                cls.Config(expectedTitle);

                // Assert
                Assert.Equal(expectedTitle, cls.Title);
                Assert.Equal("Yes", cls.PrimaryButtonText);
                Assert.Equal("No", cls.CloseButtonText);
                Assert.Equal(ContentDialogButton.Primary, cls.DefaultButton);
                Assert.Null(cls.Content);
            }
        }
    }
}
