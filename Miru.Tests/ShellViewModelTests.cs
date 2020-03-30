using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Miru;
using Miru.ViewModels;
using Xunit;
using Miru.Data;
using Autofac.Extras.Moq;

namespace Miru.Tests
{
    public class ShellViewModelTests
    {
        [Theory]
        [InlineData("ab", MiruAppStatus.Idle)]
        [InlineData("aaaaaa", MiruAppStatus.Idle)]
        [InlineData("aaaaaaaaaaaa", MiruAppStatus.Idle)]
        [InlineData("bb", MiruAppStatus.InternetConnectionProblems)]
        public void CanSyncUserAnimeList_CorrectUsernameLengthAndAppStatusShouldReturnTrue(string typedInUsername, MiruAppStatus appStatus)
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var cls = mock.Create<ShellViewModel>();

                // Act
                bool actual = cls.CanSyncUserAnimeList(typedInUsername, appStatus, true);

                // Assert
                Assert.True(actual); 
            }
        }

        [Theory]
        [InlineData("ab", MiruAppStatus.Syncing)]
        [InlineData("", MiruAppStatus.Idle)]
        [InlineData("", MiruAppStatus.Syncing)]
        [InlineData("ab", MiruAppStatus.Loading)]
        [InlineData("aaaaaaaaaaaa", MiruAppStatus.ClearingDatabase)]
        [InlineData("aaa", MiruAppStatus.CheckingInternetConnection)]
        public void CanSyncUserAnimeList_IncorrectUsernameLengthOrAppStatusShouldReturnFalse(string typedInUsername, MiruAppStatus appStatus)
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var cls = mock.Create<ShellViewModel>();

                // Act
                bool actual = cls.CanSyncUserAnimeList(typedInUsername, appStatus, true);

                // Assert
                Assert.False(actual);
            }
        }
    }
}
