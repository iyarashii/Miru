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
using ModernWpf;
using System.Windows.Media;
using Moq;

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
        [InlineData("      ", MiruAppStatus.Idle)]
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

        public static IEnumerable<object[]> GetBrushColors()
        {
            yield return new object[] { ApplicationTheme.Dark, Brushes.SeaGreen };
            yield return new object[] { ApplicationTheme.Light, Brushes.Lime };
            yield return new object[] { int.MaxValue, Brushes.Red };
        }

        [Theory]
        [MemberData(nameof(GetBrushColors))]
        public void UpdateBrushColors_ShouldSetDifferentColorsForDifferentThemes(ApplicationTheme applicationTheme, SolidColorBrush expected)
        {

            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var cls = mock.Create<ShellViewModel>();
                ThemeManager.Current.ApplicationTheme = applicationTheme;

                // Act
                cls.UpdateBrushColors();

                // Assert
                 Assert.Equal(expected, cls.DaysOfTheWeekBrush);
            }
        }

        [Theory]
        [InlineData(ApplicationTheme.Dark, ApplicationTheme.Light)]
        [InlineData(ApplicationTheme.Light, ApplicationTheme.Dark)]
        public void ChangeTheme_ShouldChangeThemeToDifferentOneThanTheCurrent(ApplicationTheme currentTheme, ApplicationTheme expected)
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var cls = mock.Create<ShellViewModel>();
                cls.CurrentApplicationTheme = currentTheme;

                // Act
                cls.ChangeTheme();

                // Assert
                Assert.Equal(expected, cls.CurrentApplicationTheme);
            }
        }


    }
}
