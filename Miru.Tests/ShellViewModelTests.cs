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
using ToastNotifications.Core;
using System.Collections.ObjectModel;
using Caliburn.Micro;
using System.Threading;

namespace Miru.Tests
{
    public class ShellViewModelTests
    {
        #region methods tests
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
                mock.Mock<ISimpleContentDialog>()
                    .Setup(x => x.ShowAsync())
                    .ReturnsAsync(ContentDialogResult.Primary);

                mock.Mock<ISimpleContentDialog>()
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
                mock.Mock<ISimpleContentDialog>()
                    .Verify(x => x.Config
                    (
                        "Clear the database?",
                        "Yes",
                        "No",
                        ContentDialogButton.Primary
                    ),
                    Times.Once);

                mock.Mock<ISimpleContentDialog>().Verify(x => x.ShowAsync(), Times.Once);
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
                mock.Mock<ISimpleContentDialog>()
                    .Setup(x => x.ShowAsync())
                    .ReturnsAsync(clickedButton);

                mock.Mock<ISimpleContentDialog>()
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
                mock.Mock<ISimpleContentDialog>()
                    .Verify(x => x.Config
                    (
                        "Update data from senpai.moe?",
                        "Yes",
                        "No",
                        ContentDialogButton.Primary
                    ),
                    Times.Once);

                mock.Mock<ISimpleContentDialog>().Verify(x => x.ShowAsync(), Times.Once);

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
                //mock.Mock<IProcessProxy>().Setup(x => x.Start(It.IsAny<string>()));
                mock.Mock<IProcessProxy>().Setup(x => x.Start()).Returns(true);
                mock.Mock<IProcessProxy>().SetupGet(x => x.StartInfo).Returns(new System.Diagnostics.ProcessStartInfo());

                var cls = mock.Create<ShellViewModel>();

                // Act
                cls.OpenAnimeURL(It.IsAny<string>());

                // Assert
                //mock.Mock<IProcessProxy>().Verify(x => x.Start(It.IsAny<string>()), Times.Once);
                mock.Mock<IProcessProxy>().Verify(x => x.Start(), Times.Once);
            }
        }

        [Fact]
        public void CopyAnimeTitleToClipboard_ValidCall()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var cls = mock.Create<ShellViewModel>();
                mock.Mock<IClipboardWrapper>().Setup(x => x.SetText(It.IsAny<string>()));
                mock.Mock<IToastNotifierWrapper>().Setup(x => x.ShowInformation(It.IsAny<string>(), It.IsAny<MessageOptions>()));

                // Act
                cls.CopyAnimeTitleToClipboard(It.IsAny<string>());

                // Assert
                mock.Mock<IClipboardWrapper>().Verify(x => x.SetText(It.IsAny<string>()), Times.Once);
                mock.Mock<IToastNotifierWrapper>().Verify(x => x.ShowInformation(It.IsAny<string>(), It.IsAny<MessageOptions>()), Times.Once);
            }
        }
        #endregion methods tests

        #region properties tests

        [Fact]
        public void ToastNotifierWrapper_ReturnsCorrectValue()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var fakeToastNotifier = new ToastNotifierWrapper();
                mock.Mock<IShellViewModel>().SetupGet(x => x.ToastNotifierWrapper).Returns(fakeToastNotifier);

                // Act & Assert
                Assert.Equal(fakeToastNotifier, mock.Mock<IShellViewModel>().Object.ToastNotifierWrapper);
            }
        }

        [Fact]
        public void ClipboardWrapper_ReturnsCorrectValue()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var fakeClipboard = new ClipboardWrapper();
                mock.Mock<IShellViewModel>().SetupGet(x => x.ClipboardWrapper).Returns(fakeClipboard);

                // Act & Assert
                Assert.Equal(fakeClipboard, mock.Mock<IShellViewModel>().Object.ClipboardWrapper);
            }
        }

        [Fact]
        public void AnimeURLProcessProxy_ReturnsCorrectValue()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var fakeProcessProxy = new ProcessProxy();
                mock.Mock<IShellViewModel>().SetupGet(x => x.AnimeURLProcessProxy).Returns(fakeProcessProxy);

                // Act & Assert
                Assert.Equal(fakeProcessProxy, mock.Mock<IShellViewModel>().Object.AnimeURLProcessProxy);
            }
        }

        [Fact]
        public void ContentDialog_ReturnsCorrectValue()
        {
            Thread STAThread = new Thread(() =>
            {
                using (var mock = AutoMock.GetLoose())
                {
                    // Arrange
                    //SimpleContentDialog fakeContentDialog = new SimpleContentDialog();
                    SimpleContentDialog fakeContentDialog = It.IsAny<SimpleContentDialog>();
                    mock.Mock<IShellViewModel>().SetupGet(x => x.ContentDialog).Returns(fakeContentDialog);

                    // Act & Assert
                    Assert.Equal(fakeContentDialog, mock.Mock<IShellViewModel>().Object.ContentDialog);
                }
            });
            STAThread.SetApartmentState(ApartmentState.STA);
            STAThread.Start();
            STAThread.Join();
        }

        [Fact]
        public void DbService_ReturnsCorrectValue()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var fakeDbService = mock.Create<MiruDbService>();
                mock.Mock<IShellViewModel>().SetupGet(x => x.DbService).Returns(fakeDbService);

                // Act & Assert
                Assert.Equal(fakeDbService, mock.Mock<IShellViewModel>().Object.DbService);
            }
        }

        [Fact]
        public void TimeZones_ReturnsCorrectValue()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var fakeTimeZones = new ReadOnlyCollection<TimeZoneInfo>(new List<TimeZoneInfo>());
                mock.Mock<IShellViewModel>().SetupGet(x => x.TimeZones).Returns(fakeTimeZones);

                // Act & Assert
                Assert.Equal(fakeTimeZones, mock.Mock<IShellViewModel>().Object.TimeZones);
            }
        }

        [Fact]
        public void SyncDate_StoresCorrectly()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var cls = mock.Create<ShellViewModel>();
                var testDate = It.IsAny<DateTime>();

                // Act
                cls.SyncDate = testDate;

                // Assert
                Assert.Equal(testDate, cls.SyncDate);
            }
        }

        [Fact]
        public void IsDarkModeOn_StoresCorrectly()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var cls = mock.Create<ShellViewModel>();
                var testValue = It.IsAny<bool>();

                // Act
                cls.IsDarkModeOn = testValue;

                // Assert
                Assert.Equal(testValue, cls.IsDarkModeOn);
            }
        }

        [Fact]
        public void SelectedDisplayedAnimeList_StoresCorrectly()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var cls = mock.Create<ShellViewModel>();
                var testValue = AnimeListType.Watching;
                mock.Mock<IMiruDbService>()
                    .Setup(x => x.ChangeDisplayedAnimeList(testValue, cls.SelectedTimeZone, cls.SelectedDisplayedAnimeType, cls.CurrentAnimeNameFilter));

                // Act
                cls.SelectedDisplayedAnimeList = testValue;

                // Assert
                Assert.Equal(testValue, cls.SelectedDisplayedAnimeList);
                mock.Mock<IMiruDbService>()
                    .Verify(x => 
                    x.ChangeDisplayedAnimeList(testValue, cls.SelectedTimeZone, cls.SelectedDisplayedAnimeType, cls.CurrentAnimeNameFilter), Times.Once);
            }
        }

        [Fact]
        public void SelectedDisplayedAnimeType_StoresCorrectly()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var cls = mock.Create<ShellViewModel>();
                var testValue = AnimeType.TV;
                mock.Mock<IMiruDbService>()
                    .Setup(x => x.ChangeDisplayedAnimeList(cls.SelectedDisplayedAnimeList, cls.SelectedTimeZone, testValue, cls.CurrentAnimeNameFilter));

                // Act
                cls.SelectedDisplayedAnimeType = testValue;

                // Assert
                Assert.Equal(testValue, cls.SelectedDisplayedAnimeType);
                mock.Mock<IMiruDbService>()
                    .Verify(x =>
                    x.ChangeDisplayedAnimeList(cls.SelectedDisplayedAnimeList, cls.SelectedTimeZone, testValue, cls.CurrentAnimeNameFilter), Times.Once);
            }
        }

        [Fact]
        public void SelectedTimeZone_StoresCorrectly()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var cls = mock.Create<ShellViewModel>();
                var testValue = It.IsAny<TimeZoneInfo>();
                mock.Mock<IMiruDbService>()
                    .Setup(x => x.ChangeDisplayedAnimeList(cls.SelectedDisplayedAnimeList, testValue, cls.SelectedDisplayedAnimeType, cls.CurrentAnimeNameFilter));

                // Act
                cls.SelectedTimeZone = testValue;

                // Assert
                Assert.Equal(testValue, cls.SelectedTimeZone);
                mock.Mock<IMiruDbService>()
                    .Verify(x =>
                    x.ChangeDisplayedAnimeList(cls.SelectedDisplayedAnimeList, testValue, cls.SelectedDisplayedAnimeType, cls.CurrentAnimeNameFilter), Times.Once);
            }
        }

        [Fact]
        public void CurrentAnimeNameFilter_StoresCorrectly()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var cls = mock.Create<ShellViewModel>();
                var testValue = "TEST STRING";
                mock.Mock<IMiruDbService>()
                    .Setup(x => x.ChangeDisplayedAnimeList(cls.SelectedDisplayedAnimeList, cls.SelectedTimeZone, cls.SelectedDisplayedAnimeType, testValue));

                // Act
                cls.CurrentAnimeNameFilter = testValue;

                // Assert
                Assert.Equal(testValue, cls.CurrentAnimeNameFilter);
                mock.Mock<IMiruDbService>()
                    .Verify(x =>
                    x.ChangeDisplayedAnimeList(cls.SelectedDisplayedAnimeList, cls.SelectedTimeZone, cls.SelectedDisplayedAnimeType, testValue), Times.Once);
            }
        }

        [Fact]
        public void DaysOfTheWeekBrush_StoresCorrectly()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var cls = mock.Create<ShellViewModel>();
                var testValue = It.IsAny<SolidColorBrush>();

                // Act
                cls.DaysOfTheWeekBrush = testValue;

                // Assert
                Assert.Equal(testValue, cls.DaysOfTheWeekBrush);
            }
        }

        [Fact]
        public void CurrentApplicationTheme_StoresCorrectly()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var cls = mock.Create<ShellViewModel>();
                var testValue = It.IsAny<ApplicationTheme>();

                // Act
                cls.CurrentApplicationTheme = testValue;

                // Assert
                Assert.Equal(testValue, cls.CurrentApplicationTheme);
                Assert.Equal(testValue == ApplicationTheme.Dark, cls.IsDarkModeOn);
            }
        }

        [Fact]
        public void SortedAnimeListEntries_StoresCorrectly()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var cls = mock.Create<ShellViewModel>();
                var testValue = It.IsAny<ISortedAnimeListEntries>();

                // Act
                cls.SortedAnimeListEntries = testValue;

                // Assert
                Assert.Equal(testValue, cls.SortedAnimeListEntries);
            }
        }

        [Fact]
        public void AppStatusText_StoresCorrectly()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var cls = mock.Create<ShellViewModel>();
                var testValue = It.IsAny<string>();

                // Act
                typeof(ShellViewModel).GetProperty("AppStatusText").SetValue(cls, testValue);

                // Assert
                Assert.Equal($"Miru -- { testValue }", cls.AppStatusText);
            }
        }

        [Fact]
        public void AppStatus_StoresCorrectly()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var cls = mock.Create<ShellViewModel>();
                var testValue = It.IsAny<MiruAppStatus>();
                var expectedAppStatusTextValue = "Miru -- ";
                expectedAppStatusTextValue += testValue == MiruAppStatus.Idle ? "Idle" : testValue == MiruAppStatus.Busy ? "Busy..." : "Problems with internet connection!";

                // Act
                typeof(ShellViewModel).GetProperty("AppStatus").SetValue(cls, testValue);

                // Assert
                Assert.Equal(testValue, cls.AppStatus);
                Assert.Equal(expectedAppStatusTextValue, cls.AppStatusText);
                Assert.Equal(testValue != MiruAppStatus.Busy, cls.CanChangeDisplayedAnimeList);
            }
        }

        [Fact]
        public void TypedInUsername_StoresCorrectly()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var cls = mock.Create<ShellViewModel>();
                var testValue = It.IsAny<string>();

                // Act
                cls.TypedInUsername = testValue;

                // Assert
                Assert.Equal(testValue, cls.TypedInUsername);
            }
        }

        [Fact]
        public void MalUserName_StoresCorrectly()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var cls = mock.Create<ShellViewModel>();
                var testValue = It.IsAny<string>();

                // Act
                cls.MalUserName = testValue;

                // Assert
                Assert.Equal(testValue, cls.MalUserName);
            }
        }

        [Theory]
        [InlineData(true, "not empty")]
        [InlineData(false, null)]
        public void IsSynced_ReturnsCorrectValue(bool expected, string malUserName)
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var cls = mock.Create<ShellViewModel>();

                // Act
                cls.MalUserName = malUserName;

                // Assert
                Assert.Equal(expected, cls.IsSynced);
            }
        }

        [Fact]
        public void UserAnimeListURL_StoresCorrectly()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var cls = mock.Create<ShellViewModel>();
                var testValue = It.IsAny<string>();

                // Act
                cls.UserAnimeListURL = testValue;

                // Assert
                Assert.Equal($@"https://myanimelist.net/animelist/{ testValue }", cls.UserAnimeListURL);
            }
        }

        [Fact]
        public void CanChangeDisplayedAnimeList_StoresCorrectly()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var cls = mock.Create<ShellViewModel>();
                var testValue = It.IsAny<bool>();

                // Act
                cls.CanChangeDisplayedAnimeList = testValue;

                // Assert
                Assert.Equal(testValue, cls.CanChangeDisplayedAnimeList);
            }
        }

        #endregion properties tests
    }
}
