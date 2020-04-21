using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Miru;
using Miru.ViewModels;
using Xunit;
using Miru.Data;
using Miru.Models;
using Autofac.Extras.Moq;
using ModernWpf;
using ModernWpf.Controls;
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
        [InlineData("ab", MiruAppStatus.Busy)]
        [InlineData("", MiruAppStatus.Idle)]
        [InlineData("      ", MiruAppStatus.Idle)]
        [InlineData("", MiruAppStatus.Busy)]
        [InlineData("spa ce", MiruAppStatus.InternetConnectionProblems)]
        [InlineData("aaaaaaaaaaaa", MiruAppStatus.Busy)]
        [InlineData("aaa", MiruAppStatus.Busy)]
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

        [Theory]
        [InlineData("iyarashii777", true)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void IsSynced_ShouldReturnCorrectValue(string currentMalUsername, bool expected)
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var cls = mock.Create<ShellViewModel>();
                cls.MalUserName = currentMalUsername;

                // Act and Assert
                Assert.Equal(expected, cls.IsSynced);
            }
        }

        [Fact]
        public void ClearAppData_ValidCall()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                mock.Mock<IMiruDbService>().Setup(x => x.ClearDb());
                mock.Mock<IMiruDbService>().Setup(x => x.ClearLocalImageCache());

                var cls = mock.Create<ShellViewModel>();

                cls.MalUserName = "test";
                cls.TypedInUsername = "test";
                cls.CurrentAnimeNameFilter = "test";

                // Act
                cls.ClearAppData();

                // Assert
                Assert.Equal(string.Empty, cls.MalUserName);
                Assert.Equal(string.Empty, cls.TypedInUsername);
                Assert.Equal(string.Empty, cls.CurrentAnimeNameFilter);
                mock.Mock<IMiruDbService>().Verify(x => x.ClearDb(), Times.Once);
                mock.Mock<IMiruDbService>().Verify(x => x.ClearLocalImageCache(), Times.Once);
            }
        }

        [Theory]
        [InlineData(MiruAppStatus.Busy, null, MiruAppStatus.Busy, "Miru -- Busy...")]
        [InlineData(MiruAppStatus.Idle, null, MiruAppStatus.Idle, "Miru -- Idle")]
        [InlineData(MiruAppStatus.InternetConnectionProblems, null, MiruAppStatus.InternetConnectionProblems, "Miru -- Problems with internet connection!")]
        [InlineData(MiruAppStatus.Busy, "custom message with Busy", MiruAppStatus.Busy, "Miru -- custom message with Busy")]
        [InlineData(MiruAppStatus.Idle, "custom message with idle", MiruAppStatus.Idle, "Miru -- custom message with idle")]
        [InlineData(MiruAppStatus.InternetConnectionProblems, "custom message with ICP", MiruAppStatus.InternetConnectionProblems, "Miru -- custom message with ICP")]
        public void UpdateAppStatus_ShouldSetCorrectValues(MiruAppStatus newAppStatus, string detailedAppStatusDescription, 
            MiruAppStatus expectedAppStatus, string expectedAppStatusText)
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var cls = mock.Create<ShellViewModel>();

                // Act
                cls.UpdateAppStatus(newAppStatus, detailedAppStatusDescription);

                // Assert
                Assert.Equal(expectedAppStatus, cls.AppStatus);
                Assert.Equal(expectedAppStatusText, cls.AppStatusText);
            }
        }

        [Fact]
        public void OpenClearDatabaseDialog_ValidCall()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                mock.Mock<IContentDialogWrapper>()
                    .Setup(x => x.ShowAsync())
                    .ReturnsAsync(ContentDialogResult.Primary);

                mock.Mock<IContentDialogWrapper>()
                    .Setup(x => x.Config
                    (
                        "Clear the database?",
                        "Yes",
                        "No",
                        ContentDialogButton.Primary
                    ));
                
                var cls = mock.Create<ShellViewModel>();

                // Act
                cls.OpenClearDatabaseDialog().Wait();

                // Assert
                mock.Mock<IContentDialogWrapper>()
                    .Verify(x => x.Config
                    (
                        "Clear the database?",
                        "Yes",
                        "No",
                        ContentDialogButton.Primary
                    ),
                    Times.Once);

                mock.Mock<IContentDialogWrapper>().Verify(x => x.ShowAsync(), Times.Once);
            }
        }

        [Theory]
        [InlineData(ContentDialogResult.Primary, 1)]
        [InlineData(ContentDialogResult.Secondary, 0)]
        public void UpdateSenpaiData_ShouldUpdateOnlyOnPrimaryButton(ContentDialogResult clickedButton, int expected)
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                mock.Mock<IContentDialogWrapper>()
                    .Setup(x => x.ShowAsync())
                    .ReturnsAsync(clickedButton);

                mock.Mock<IContentDialogWrapper>()
                    .Setup(x => x.Config
                    (
                        "Update data from senpai.moe?",
                        "Yes",
                        "No",
                        ContentDialogButton.Primary
                    ));

                mock.Mock<IMiruDbService>().Setup(x => x.UpdateSenpaiData());

                var cls = mock.Create<ShellViewModel>();

                // Act
                cls.UpdateSenpaiData().Wait();

                // Assert
                mock.Mock<IContentDialogWrapper>()
                    .Verify(x => x.Config
                    (
                        "Update data from senpai.moe?",
                        "Yes",
                        "No",
                        ContentDialogButton.Primary
                    ),
                    Times.Once);

                mock.Mock<IContentDialogWrapper>().Verify(x => x.ShowAsync(), Times.Once);

                mock.Mock<IMiruDbService>().Verify(x => x.UpdateSenpaiData(), Times.Exactly(expected));
            }
        }

        public static IEnumerable<object[]> GenerateValuesForSyncUserAnimeList()
        {
            yield return new object[] { true, (true, "error message"), true, true, 1, 1, 1 };

            yield return new object[] { false, (true, "error message"), true, true, 0, 1, 1 };

            yield return new object[] { true, (false, "error message"), true, true, 0, 0, 0 };

            yield return new object[] { true, (true, "error message"), false, true, 1, 0, 0 };

            yield return new object[] { true, (true, "error message"), true, false, 1, 1, 1 };

            yield return new object[] { false, (true, "error message"), true, false, 0, 1, 1 };
        }


        [Theory]
        [MemberData(nameof(GenerateValuesForSyncUserAnimeList))]
        public void SyncUserAnimeList_ValidCall
            (
            bool isSeasonSyncOn, 
            (bool Success, string ErrorMessage) getCurrentUserAnimeListResult, 
            bool getCurrentSeasonListResult,
            bool saveDetailedAnimeListDataResult,
            int expectedGetCurrentSeasonListTimesCalled,
            int expectedSaveDetailedAnimeListDataTimesCalled,
            int expectedSaveSyncedUserDataTimesCalled
            )
        {
            using(var mock = AutoMock.GetLoose())
            {

                // Arrange
                var cls = mock.Create<ShellViewModel>();

                mock.Mock<ICurrentUserAnimeListModel>()
                    .Setup(x => x.GetCurrentUserAnimeList(null))
                    .ReturnsAsync(getCurrentUserAnimeListResult);

                mock.Mock<ICurrentSeasonModel>()
                    .Setup(x => x.GetCurrentSeasonList(It.IsAny<int>()))
                    .ReturnsAsync(getCurrentSeasonListResult);

                mock.Mock<IMiruDbService>()
                    .Setup(x => x.SaveDetailedAnimeListData(isSeasonSyncOn))
                    .ReturnsAsync(saveDetailedAnimeListDataResult);

                mock.Mock<IMiruDbService>().Setup(x => x.SaveSyncedUserData());

                var currentUserAnimeListMock = mock.Create<ICurrentUserAnimeListModel>();
                var currentSeasonMock = mock.Create<ICurrentSeasonModel>();

                mock.Mock<IMiruDbService>().SetupGet(x => x.CurrentUserAnimeList).Returns(currentUserAnimeListMock);
                mock.Mock<IMiruDbService>().SetupGet(x => x.CurrentSeason).Returns(currentSeasonMock);


                // Act
                cls.SyncUserAnimeList(null, It.IsAny<MiruAppStatus>(), isSeasonSyncOn).Wait();

                // Assert
                mock.Mock<ICurrentUserAnimeListModel>().Verify(x => x.GetCurrentUserAnimeList(null), Times.Once);

                mock.Mock<ICurrentSeasonModel>().Verify(x => x.GetCurrentSeasonList(It.IsAny<int>()), Times.Exactly(expectedGetCurrentSeasonListTimesCalled));

                mock.Mock<IMiruDbService>().Verify(x => x.SaveDetailedAnimeListData(isSeasonSyncOn), Times.Exactly(expectedSaveDetailedAnimeListDataTimesCalled));

                mock.Mock<IMiruDbService>().Verify(x => x.SaveSyncedUserData(), Times.Exactly(expectedSaveSyncedUserDataTimesCalled));
            }
        }

        [Fact]
        public void OpenAnimeURL_ValidCall()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                mock.Mock<IProcessProxy>().Setup(x => x.Start(It.IsAny<string>()));

                var cls = mock.Create<ShellViewModel>();

                // Act
                cls.OpenAnimeURL(It.IsAny<string>());

                // Assert
                mock.Mock<IProcessProxy>().Verify(x => x.Start(It.IsAny<string>()), Times.Once);
            }
        }

    }
}
