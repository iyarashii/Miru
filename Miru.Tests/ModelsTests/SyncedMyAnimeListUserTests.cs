using Autofac.Extras.Moq;
using MiruLibrary.Models;
using Moq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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

        [Fact]
        public void Username_ShorterThanMinLength_ValidationFails()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var sut = mock.Create<SyncedMyAnimeListUser>();
                var valueToValidate = new string('x', 1);
                var usernameContext = new ValidationContext(sut) { MemberName = nameof(sut.Username) };
                var validationResults = new List<ValidationResult>();

                // Act
                var result = Validator.TryValidateProperty(valueToValidate, usernameContext, validationResults);

                // Assert
                Assert.False(result);
                Assert.NotNull(validationResults);
                Assert.Equal("The field Username must be a string or array type with a minimum length of '2'.", validationResults.First().ErrorMessage);
            }
        }

        [Theory]
        [InlineData(2)]
        [InlineData(16)]
        [InlineData(12)]
        [InlineData(9)]
        public void Username_WithinLengthLimits_ValidationSucceeds(int usernameLength)
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var sut = mock.Create<SyncedMyAnimeListUser>();
                var valueToValidate = new string(It.IsAny<char>(), usernameLength);
                var usernameContext = new ValidationContext(sut) { MemberName = nameof(sut.Username) };
                var validationResults = new List<ValidationResult>();

                // Act
                var result = Validator.TryValidateProperty(valueToValidate, usernameContext, validationResults);

                // Assert
                Assert.True(result);
                Assert.NotNull(validationResults);
            }
        }
    }
}
