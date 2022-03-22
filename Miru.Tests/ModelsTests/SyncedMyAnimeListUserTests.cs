using Autofac.Extras.Moq;
using MiruLibrary.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Xunit;

namespace Miru.Tests.ModelsTests
{
    public class SyncedMyAnimeListUserTests
    {
        [Fact]
        public void Username_LongerThanMaxLength_ValidationFails()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var sut = mock.Create<SyncedMyAnimeListUser>();
                var valueToValidate = new string('x', 17);
                var usernameContext = new ValidationContext(sut) { MemberName = nameof(sut.Username) };
                var validationResults = new List<ValidationResult>();

                // Act
                var result = Validator.TryValidateProperty(valueToValidate, usernameContext, validationResults);

                // Assert
                Assert.False(result);
                Assert.NotNull(validationResults);
                Assert.Equal("The field Username must be a string or array type with a maximum length of '16'.", validationResults.First().ErrorMessage);
            }
        }
    }
}
