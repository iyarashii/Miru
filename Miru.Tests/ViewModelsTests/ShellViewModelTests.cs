// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using Autofac;
using Autofac.Extras.Moq;
using Microsoft.Win32;
using Miru.ViewModels;
using MiruDatabaseLogicLayer;
using MiruLibrary;
using MiruLibrary.Models;
using MiruLibrary.Services;
using ModernWpf;
using ModernWpf.Controls;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using Xunit;
// CODE COVERAGE commands used for .NET Framework
// command for code coverage generation - run in Package Manager Console inside Test.csproj directory
// dotnet test --no-build --collect:"XPlat Code Coverage" 
// command for coverage report generation
// G:\NuGetPackages\reportgenerator\5.0.4\tools\net47\ReportGenerator.exe "-reports:G:\repos\Miru\Miru.Tests\TestResults\091fcb43-46cf-4638-bbe6-dc5ab617672a\coverage.cobertura.xml" "-targetdir:G:\repos\Miru\coveragereport" -reporttypes:Html
namespace Miru.Tests
{
    public class ShellViewModelTests
    {
        #region methods tests
        private const string MIN_LENGTH_USERNAME = "xx";
        private const string CORRECT_LENGTH_USERNAME = "xxxxxx";
        private const string MAX_LENGTH_USERNAME = "xxxxxxxxxxxx";
        private const string EMPTY_USERNAME = "";
        private const string WHITESPACE_ONLY_USERNAME = "      ";
        private const string USERNAME_WITH_WHITESPACE = "spa ce";


        [Theory]
        [InlineData(MIN_LENGTH_USERNAME, MiruAppStatus.Idle, true)]
        [InlineData(CORRECT_LENGTH_USERNAME, MiruAppStatus.Idle, true)]
        [InlineData(MAX_LENGTH_USERNAME, MiruAppStatus.Idle, true)]
        [InlineData(CORRECT_LENGTH_USERNAME, MiruAppStatus.InternetConnectionProblems, true)]
        [InlineData(CORRECT_LENGTH_USERNAME, MiruAppStatus.Busy, false)]
        [InlineData(MAX_LENGTH_USERNAME, MiruAppStatus.Busy, false)]
        [InlineData(EMPTY_USERNAME, MiruAppStatus.Busy, false)]
        [InlineData(EMPTY_USERNAME, MiruAppStatus.Idle, false)]
        [InlineData(WHITESPACE_ONLY_USERNAME, MiruAppStatus.Idle, false)]
        [InlineData(USERNAME_WITH_WHITESPACE, MiruAppStatus.InternetConnectionProblems, false)]
        public void CanSyncUserAnimeList_ShouldReturnCorrectValues(string typedInUsername, MiruAppStatus appStatus, bool expected)
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var cls = mock.Create<ShellViewModel>();

                // Act
                bool actual = cls.CanSyncUserAnimeList(typedInUsername, appStatus, true);

                // Assert
                Assert.Equal(expected, actual); 
            }
        }

        [Fact]
        public void UpdateSyncDate_SetsSyncDateProperty()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var cls = mock.Create<ShellViewModel>();
                var testValue = new DateTime();

                // Act
                cls.UpdateSyncDate(null, testValue);

                // Assert
                Assert.Equal(testValue, cls.SyncDate);
            }
        }

        [Fact]
        public void UpdateUsername_WhenTypedInUsernameIsEmpty_SetBothMalUsernameAndTypedInUsername()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var cls = mock.Create<ShellViewModel>();
                var testValue = "name";

                // Act
                cls.UpdateUsername(null, testValue);

                // Assert
                Assert.Equal(testValue, cls.MalUserName);
                Assert.Equal(testValue, cls.TypedInUsername);
            }
        }

        [Fact]
        public void UpdateUsername_WhenTypedInUsernameIsNotEmpty_SetOnlyMalUsername()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var cls = mock.Create<ShellViewModel>();
                var testValue = "name";
                cls.TypedInUsername = "2137";

                // Act
                cls.UpdateUsername(null, testValue);

                // Assert
                Assert.Equal(testValue, cls.MalUserName);
                Assert.NotEqual(testValue, cls.TypedInUsername);
            }
        }

        public static IEnumerable<object[]> GetBrushColorsData()
        {
            yield return new object[] { ApplicationTheme.Dark, Brushes.SeaGreen };
            yield return new object[] { ApplicationTheme.Light, Brushes.Lime };
            yield return new object[] { int.MaxValue, Brushes.Red };
        }

        [Theory]
        [MemberData(nameof(GetBrushColorsData))]
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
        [InlineData(CORRECT_LENGTH_USERNAME, true)]
        [InlineData(EMPTY_USERNAME, false)]
        [InlineData(null, false)]
        public void IsSynced_ShouldReturnCorrectValue(string inputUsername, bool expectedReturnValue)
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var cls = mock.Create<ShellViewModel>();
                cls.MalUserName = inputUsername;

                // Act
                var actual = cls.IsSynced;

                // Assert
                Assert.Equal(expectedReturnValue, actual);
            }
        }

        [Fact]
        public void ClearAppData_ValidCall()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                //mock.Mock<IMiruDbService>().Setup(x => x.ClearDb());
                //mock.Mock<IMiruDbService>().Setup(x => x.ClearLocalImageCache());

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
                mock.Mock<IFileSystemService>().Verify(x => x.ClearImageCache(), Times.Once);
            }
        }

        private const string BUSY_APP_STATUS_TEXT = "Miru -- Busy...";
        private const string IDLE_APP_STATUS_TEXT = "Miru -- Idle";
        private const string InternetConnectionProblemsAppStatusText = "Miru -- Problems with internet connection!";
        private const string CUSTOM_APP_STATUS_DETAILED_DESCRIPTION = "custom";
        private const string APP_STATUS_TEXT_FOR_CUSTOM_DESCRIPTION = "Miru -- custom";


        [Theory]
        [InlineData(MiruAppStatus.Busy, null, MiruAppStatus.Busy, BUSY_APP_STATUS_TEXT)]
        [InlineData(MiruAppStatus.Idle, null, MiruAppStatus.Idle, IDLE_APP_STATUS_TEXT)]
        [InlineData(MiruAppStatus.InternetConnectionProblems, null, MiruAppStatus.InternetConnectionProblems, InternetConnectionProblemsAppStatusText)]
        [InlineData(MiruAppStatus.Busy, CUSTOM_APP_STATUS_DETAILED_DESCRIPTION, MiruAppStatus.Busy, APP_STATUS_TEXT_FOR_CUSTOM_DESCRIPTION)]
        [InlineData(MiruAppStatus.Idle, CUSTOM_APP_STATUS_DETAILED_DESCRIPTION, MiruAppStatus.Idle, APP_STATUS_TEXT_FOR_CUSTOM_DESCRIPTION)]
        [InlineData(MiruAppStatus.InternetConnectionProblems, CUSTOM_APP_STATUS_DETAILED_DESCRIPTION, MiruAppStatus.InternetConnectionProblems, APP_STATUS_TEXT_FOR_CUSTOM_DESCRIPTION)]
        public void UpdateAppStatus_ShouldSetCorrectValues(MiruAppStatus inputAppStatus, string inputDetailedAppStatusDescription, 
            MiruAppStatus expectedAppStatus, string expectedAppStatusText)
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var cls = mock.Create<ShellViewModel>(new NamedParameter("systemService", new SystemService()));

                // Act
                cls.UpdateAppStatus(inputAppStatus, inputDetailedAppStatusDescription);

                // Assert
                Assert.Equal(expectedAppStatus, cls.AppStatus);
                Assert.Equal(expectedAppStatusText, cls.AppStatusText);
            }
        }

        [Fact]
        public void OpenClearLocalDataDialog_ValidCall()
        {
            using (var mock = AutoMock.GetLoose())
            {
                mock.Mock<ISimpleContentDialog>()
                    .Setup(x => x.ShowAsync())
                    .ReturnsAsync(ContentDialogResult.Primary);
                var cls = mock.Create<ShellViewModel>();

                cls.OpenClearLocalDataDialog().Wait();

                Assert.Equal(MiruAppStatus.Idle, cls.AppStatus);
                mock.Mock<ISimpleContentDialog>()
                    .Verify(x => x.Config
                    (
                        "Clear local data?",
                        "Yes",
                        "No",
                        ContentDialogButton.Primary,
                        "Clears local database and image cache.",
                        ""
                    ),
                    Times.Once);
                mock.Mock<ISimpleContentDialog>().Verify(x => x.ShowAsync(), Times.Once);
            }
        }

        [Theory]
        [InlineData(ContentDialogResult.Primary, 1)]
        [InlineData(ContentDialogResult.Secondary, 0)]
        public void UpdateSenpaiData_ShouldUpdateOnlyOnPrimaryButton(ContentDialogResult clickedButton, int expectedTimesCalled)
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                mock.Mock<ISimpleContentDialog>()
                    .Setup(x => x.ShowAsync())
                    .ReturnsAsync(clickedButton);
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
                        ContentDialogButton.Primary,
                        null,
                        ""
                    ),
                    Times.Once);
                Assert.Equal(MiruAppStatus.Idle, cls.AppStatus);
                mock.Mock<ISimpleContentDialog>().Verify(x => x.ShowAsync(), Times.Once);
                mock.Mock<IFileSystemService>().Verify(x => x.UpdateSenpaiData(), Times.Exactly(expectedTimesCalled));
            }
        }

        [Theory]
        [InlineData(false, 0)]
        [InlineData(true, 1)]
        public void SyncUserAnimeList_ValidCall(bool isSeasonSyncOn, int expectedGetCurrentSeasonListTimesCalled)
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var cls = mock.Create<ShellViewModel>();

                mock.Mock<ICurrentUserAnimeListModel>()
                    .Setup(x => x.GetCurrentUserAnimeList(null))
                    .ReturnsAsync((true, It.IsAny<string>()));

                mock.Mock<ICurrentSeasonModel>()
                    .Setup(x => x.GetCurrentSeasonList(It.IsAny<int>()))
                    .ReturnsAsync(true);

                mock.Mock<IMiruDbService>()
                    .Setup(x => x.SaveDetailedAnimeListData(isSeasonSyncOn))
                    .ReturnsAsync(true);

                mock.Mock<IMiruDbService>().Setup(x => x.SaveSyncedUserData(It.IsAny<string>()));

                var currentUserAnimeListMock = mock.Create<ICurrentUserAnimeListModel>();
                var currentSeasonMock = mock.Create<ICurrentSeasonModel>();

                mock.Mock<IMiruDbService>().SetupGet(x => x.CurrentUserAnimeList).Returns(currentUserAnimeListMock);
                mock.Mock<IMiruDbService>().SetupGet(x => x.CurrentSeason).Returns(currentSeasonMock);

                // Act
                cls.SyncUserAnimeList(null, It.IsAny<MiruAppStatus>(), isSeasonSyncOn).Wait();

                // Assert
                mock.Mock<ICurrentSeasonModel>().Verify(x => x.GetCurrentSeasonList(It.IsAny<int>()), Times.Exactly(expectedGetCurrentSeasonListTimesCalled));
            }
        }

        [Fact]
        public void SyncUserAnimeList_GetUserAnimeListIsFalse_AppStatusIdleWithExpectedDescription()
        {
            using (var mock = AutoMock.GetLoose())
            {
                var cls = mock.Create<ShellViewModel>();
                string expectedAppStatusDescription = "2137";
                (bool Success, string ErrorMessage) mockResult = (false, expectedAppStatusDescription);
                mock.Mock<IMiruDbService>()
                    .Setup(x => x.CurrentUserAnimeList.GetCurrentUserAnimeList(It.IsAny<string>()))
                    .ReturnsAsync(mockResult);

                cls.SyncUserAnimeList(It.IsAny<string>(), It.IsAny<MiruAppStatus>(), It.IsAny<bool>()).Wait();

                mock.Mock<IMiruDbService>().Verify(x => x.CurrentUserAnimeList.GetCurrentUserAnimeList(It.IsAny<string>()), Times.Once);
                Assert.Equal($"Miru -- { expectedAppStatusDescription }", cls.AppStatusText);
                Assert.Equal(MiruAppStatus.Idle, cls.AppStatus);
            }
        }           

        [Fact]
        public void SyncUserAnimeList_GetCurrentSeasonIsFalseAndSeasonSyncOn_AppStatusInternetConnectionProblems()
        {
            using (var mock = AutoMock.GetLoose())
            {
                var cls = mock.Create<ShellViewModel>();
                (bool Success, string ErrorMessage) mockResult = (true, null);
                mock.Mock<IMiruDbService>()
                    .Setup(x => x.CurrentUserAnimeList.GetCurrentUserAnimeList(It.IsAny<string>()))
                    .ReturnsAsync(mockResult);
                mock.Mock<IMiruDbService>()
                    .Setup(x => x.CurrentSeason.GetCurrentSeasonList(It.IsAny<int>()))
                    .ReturnsAsync(false);

                cls.SyncUserAnimeList(It.IsAny<string>(), It.IsAny<MiruAppStatus>(), true).Wait();

                mock.Mock<IMiruDbService>().Verify(x => x.CurrentSeason.GetCurrentSeasonList(It.IsAny<int>()), Times.Once);
                Assert.Equal($"Miru -- Problems with internet connection!", cls.AppStatusText);
                Assert.Equal(MiruAppStatus.InternetConnectionProblems, cls.AppStatus);
            }
        }

        [Fact]
        public void SyncUserAnimeList_SaveDetailedAnimeListDataIsFalse_AppStatusInternetConnectionProblems()
        {
            using (var mock = AutoMock.GetLoose())
            {
                var cls = mock.Create<ShellViewModel>();
                (bool Success, string ErrorMessage) mockResult = (true, null);
                mock.Mock<IMiruDbService>()
                    .Setup(x => x.CurrentUserAnimeList.GetCurrentUserAnimeList(It.IsAny<string>()))
                    .ReturnsAsync(mockResult);
                mock.Mock<IMiruDbService>()
                    .Setup(x => x.CurrentSeason.GetCurrentSeasonList(It.IsAny<int>()))
                    .ReturnsAsync(true);
                mock.Mock<IMiruDbService>()
                    .Setup(x => x.SaveDetailedAnimeListData(It.IsAny<bool>()))
                    .ReturnsAsync(false);

                cls.SyncUserAnimeList(It.IsAny<string>(), It.IsAny<MiruAppStatus>(), It.IsAny<bool>()).Wait();

                mock.Mock<IMiruDbService>().Verify(x => x.SaveDetailedAnimeListData(It.IsAny<bool>()), Times.Once);
                Assert.Equal($"Miru -- Problems with internet connection!", cls.AppStatusText);
                Assert.Equal(MiruAppStatus.InternetConnectionProblems, cls.AppStatus);
            }
        }

        [Fact]
        public void UpdateUiAfterDataSync_SetsCorrectProperties()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var cls = mock.Create<ShellViewModel>();
                var testValue = "2137";
                cls.TypedInUsername = testValue;
                mock.Mock<IMiruDbService>()
                    .Setup(x => x.ChangeDisplayedAnimeList(
                        It.IsAny<AnimeListType>(), 
                        It.IsAny<TimeZoneInfo>(), 
                        It.IsAny<AnimeType>(), 
                        null));

                // Act
                cls.UpdateUiAfterDataSync();

                // Assert
                Assert.Equal(AnimeListType.Watching, cls.SelectedDisplayedAnimeList);
                Assert.Equal(TimeZoneInfo.Local, cls.SelectedTimeZone);
                Assert.Equal(testValue, cls.MalUserName);
            }
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, false)]
        public void SaveAnimeListData_ReturnsCorrectValue(bool saveDetailedAnimeListDataResult, bool expectedResult)
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var cls = mock.Create<ShellViewModel>();
                mock.Mock<IMiruDbService>()
                   .Setup(x => x.SaveDetailedAnimeListData(It.IsAny<bool>()))
                   .ReturnsAsync(saveDetailedAnimeListDataResult);

                // Act
                var actualResult = cls.SaveAnimeListData(It.IsAny<bool>()).Result;

                // Assert
                Assert.Equal(expectedResult, actualResult);
            }
        }

        [Fact]
        public void SaveUserInfo_ValidCall()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var cls = mock.Create<ShellViewModel>();
                mock.Mock<IMiruDbService>()
                   .Setup(x => x.SaveSyncedUserData(It.IsAny<string>()));

                // Act
                cls.SaveUserInfo(It.IsAny<string>()).Wait();

                // Assert
                mock.Mock<IMiruDbService>().Verify(x => x.SaveSyncedUserData(It.IsAny<string>()));
            }
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, false)]
        public void GetCurrentSeason_ReturnsCorrectValue(bool getCurrentSeasonListResult, bool expectedResult)
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var cls = mock.Create<ShellViewModel>();

                mock.Mock<ICurrentSeasonModel>()
                    .Setup(x => x.GetCurrentSeasonList(It.IsAny<int>()))
                    .ReturnsAsync(getCurrentSeasonListResult);

                var currentSeasonMock = mock.Create<ICurrentSeasonModel>();

                mock.Mock<IMiruDbService>().SetupGet(x => x.CurrentSeason).Returns(currentSeasonMock);

                // Act
                var actualResult = cls.GetCurrentSeason().Result;

                // Assert
                Assert.Equal(expectedResult, actualResult);
            }
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, false)]
        public void GetUserAnimeList_ReturnsCorrectValue(bool getUserAnimeListResult, bool expectedResult)
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var cls = mock.Create<ShellViewModel>();

                mock.Mock<ICurrentUserAnimeListModel>()
                    .Setup(x => x.GetCurrentUserAnimeList(null))
                    .ReturnsAsync((getUserAnimeListResult, It.IsAny<string>()));

                var currentUserAnimeListMock = mock.Create<ICurrentUserAnimeListModel>();

                mock.Mock<IMiruDbService>().SetupGet(x => x.CurrentUserAnimeList).Returns(currentUserAnimeListMock);

                // Act
                var actualResult = cls.GetUserAnimeList().Result;

                // Assert
                Assert.Equal(expectedResult, actualResult);
            }
        }

        [Fact]
        public void PrepareAnimeTitleCopiedNotification_ReturnsCorrectlyFormattedAnimeTitle()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var cls = mock.Create<ShellViewModel>();
                var testTitle = "2137";

                // Act
                var actualResult = cls.PrepareAnimeTitleCopiedNotification(testTitle);

                // Assert
                Assert.Equal("'2137' copied to the clipboard!", actualResult);
            }
        }

        [Fact]
        public void CopyAnimeTitleToClipboard_ValidCall()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var cls = mock.Create<ShellViewModel>();
                var testValue = "dydnie";
                
                Thread staThread = new Thread(() =>
                {
                    var clipboardContentBeforeTest = System.Windows.Clipboard.GetText();

                    // Act
                    cls.CopyAnimeTitleToClipboard(testValue);

                    // Assert
                    Assert.Equal(testValue, System.Windows.Clipboard.GetText());
                    System.Windows.Clipboard.SetText(clipboardContentBeforeTest);
                });
                staThread.SetApartmentState(ApartmentState.STA);
                staThread.Start();
                staThread.Join();
            }
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("test name")]
        public void CheckSqlLocalDbInstallationPresence_InvalidSubKeyNames_ReturnFalse(string subKeyNames)
        {
            using (var mock = AutoMock.GetLoose())
            {
                var testRegKey = Registry.CurrentUser.CreateSubKey("MiruRegistryTest");
                testRegKey.CreateSubKey(subKeyNames);
                mock.Mock<ISystemService>().Setup(x => x.OpenLocalMachineSubKey(It.IsAny<string>())).Returns(testRegKey);
                var cls = mock.Create<ShellViewModel>();

                var result = cls.CheckSqlLocalDbInstallationPresence();

                Assert.False(result);
                Registry.CurrentUser.DeleteSubKeyTree("MiruRegistryTest");
            }
        }

        [Theory]
        [InlineData("13.0")]
        [InlineData("14.0")]
        public void CheckSqlLocalDbInstallationPresence_SqlLocalDbVer13_0_OrGreater_ReturnTrue(string subKeyNames)
        {
            using (var mock = AutoMock.GetLoose())
            {
                var testRegKey = Registry.CurrentUser.CreateSubKey("MiruRegistryTest");
                testRegKey.CreateSubKey(subKeyNames);
                mock.Mock<ISystemService>().Setup(x => x.OpenLocalMachineSubKey(It.IsAny<string>())).Returns(testRegKey);
                var cls = mock.Create<ShellViewModel>();

                var result = cls.CheckSqlLocalDbInstallationPresence();

                Assert.True(result);
                Registry.CurrentUser.DeleteSubKeyTree("MiruRegistryTest");
            }
        }

        [Theory]
        [InlineData("12.0")]
        [InlineData("10.0")]
        public void CheckSqlLocalDbInstallationPresence_SqlLocalDbVerLowerThan13_0_ReturnFalse(string subKeyNames)
        {
            using (var mock = AutoMock.GetLoose())
            {
                var testRegKey = Registry.CurrentUser.CreateSubKey("MiruRegistryTest");
                testRegKey.CreateSubKey(subKeyNames);
                mock.Mock<ISystemService>().Setup(x => x.OpenLocalMachineSubKey(It.IsAny<string>())).Returns(testRegKey);
                var cls = mock.Create<ShellViewModel>();

                var result = cls.CheckSqlLocalDbInstallationPresence();

                Assert.False(result);
                Registry.CurrentUser.DeleteSubKeyTree("MiruRegistryTest");
            }
        }

        [Fact]
        public void OpenAnimeURL_CallsProcessWithUrlParam()
        {
            using (var mock = AutoMock.GetLoose())
            {
                string testData = "🦂💯";
                mock.Mock<ISystemService>().Setup(x => x.StartProcess(It.IsAny<string>())).Returns<Process>(null);
                var cls = mock.Create<ShellViewModel>();

                var result = cls.OpenAnimeURL(testData);

                mock.Mock<ISystemService>().Verify(x => x.StartProcess(testData), Times.Once);
                Assert.Null(result);
            }
        }

        [Fact]
        public void OpenNoLocalDbInfoDialog_ResultPrimary_ExitEnvironmentCalled()
        {
            using (var mock = AutoMock.GetLoose())
            {
                mock.Mock<ISimpleContentDialog>()
                    .Setup(x => x.ShowAsync())
                    .ReturnsAsync(ContentDialogResult.Primary);
                var cls = mock.Create<ShellViewModel>();

                cls.OpenNoLocalDbInfoDialog().Wait();

                mock.Mock<ISystemService>().Verify(x => x.ExitEnvironment(0), Times.Once);
                mock.Mock<ISystemService>().Verify(x => x.StartProcess(It.IsAny<string>()), Times.Never);
                Assert.Equal(MiruAppStatus.Busy, cls.AppStatus);
            }
        }

        [Fact]
        public void OpenNoLocalDbInfoDialog_ResultSecondary_ExitEnvironmentAndOpenSqlServerDownloadUrlCalled()
        {
            using (var mock = AutoMock.GetLoose())
            {
                
                mock.Mock<ISimpleContentDialog>()
                    .Setup(x => x.ShowAsync())
                    .ReturnsAsync(ContentDialogResult.Secondary);
                var cls = mock.Create<ShellViewModel>();

                cls.OpenNoLocalDbInfoDialog().Wait();

                mock.Mock<ISystemService>().Verify(x => x.ExitEnvironment(0), Times.Once);
                mock.Mock<ISystemService>()
                    .Verify(x => x.StartProcess(@"https://docs.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb?view=sql-server-ver15"), Times.Once);
                Assert.Equal(MiruAppStatus.Busy, cls.AppStatus);
            }
        }

        [Fact]
        public void OpenNoLocalDbInfoDialog_ResultNone_NothingCalled()
        {
            using (var mock = AutoMock.GetLoose())
            {
                mock.Mock<ISimpleContentDialog>()
                    .Setup(x => x.ShowAsync())
                    .ReturnsAsync(ContentDialogResult.None);
                var cls = mock.Create<ShellViewModel>();

                cls.OpenNoLocalDbInfoDialog().Wait();

                mock.Mock<ISystemService>().Verify(x => x.ExitEnvironment(0), Times.Never);
                mock.Mock<ISystemService>().Verify(x => x.StartProcess(It.IsAny<string>()), Times.Never);
                Assert.Equal(MiruAppStatus.Busy, cls.AppStatus);
            }
        }

        [Fact]
        public async Task GetUserDroppedAnimeList_Fails_ReturnFalseAndIdleAppStatusWithErrorMsg()
        {
            using (var mock = AutoMock.GetLoose())
            {
                var expectedErrorMessage = "3939🔥🔥🔥";
                mock.Mock<ICurrentUserAnimeListModel>()
                    .Setup(x => x.GetCurrentUserDroppedAnimeList(It.IsAny<string>()))
                    .ReturnsAsync((false, expectedErrorMessage));
                var currentUserAnimeListMock = mock.Create<ICurrentUserAnimeListModel>();
                mock.Mock<IMiruDbService>().SetupGet(x => x.CurrentUserAnimeList).Returns(currentUserAnimeListMock);
                var sut = mock.Create<ShellViewModel>();

                var actualResult = await sut.GetUserDroppedAnimeList();

                Assert.False(actualResult);
                Assert.Equal(MiruAppStatus.Idle, sut.AppStatus);
                Assert.Equal($"Miru -- {expectedErrorMessage}", sut.AppStatusText);
            }
        }

        [Fact]
        public async Task GetUserDroppedAnimeList_Succeeds_ReturnTrueAndBusyAppStatus()
        {
            using (var autoMock = AutoMock.GetLoose())
            {
                autoMock.Mock<ICurrentUserAnimeListModel>()
                    .Setup(x => x.GetCurrentUserDroppedAnimeList(It.IsAny<string>()))
                    .ReturnsAsync((true, It.IsAny<string>()));
                var currentUserAnimeListMock = autoMock.Create<ICurrentUserAnimeListModel>();
                autoMock.Mock<IMiruDbService>().SetupGet(x => x.CurrentUserAnimeList).Returns(currentUserAnimeListMock);
                var sut = autoMock.Create<ShellViewModel>();

                var actualResult = await sut.GetUserDroppedAnimeList();

                Assert.True(actualResult);
                Assert.Equal(MiruAppStatus.Busy, sut.AppStatus);
            }
        }
        #endregion methods tests

        #region properties tests
        [Theory]
        [InlineData(double.NaN, 134.0)]
        [InlineData(21.37, 21.37)]
        [InlineData(1337, 1337)]
        public void AnimeImageSizeInPixels_ReturnsCorrectValue(double testValue, double expected)
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var cls = mock.Create<ShellViewModel>();

                // Act
                cls.AnimeImageSizeInPixels = testValue;

                // Assert
                Assert.Equal(expected, cls.AnimeImageSizeInPixels);
            }
        }

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
        public void TimeZones_ReturnsSystemTimeZones()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                //var fakeTimeZones = new ReadOnlyCollection<TimeZoneInfo>(new List<TimeZoneInfo>());
                //mock.Mock<IShellViewModel>().SetupGet(x => x.TimeZones).Returns(fakeTimeZones);
                var cls = mock.Create<ShellViewModel>();
                var expected = TimeZoneInfo.GetSystemTimeZones();

                // Act
                var actual = cls.TimeZones;

                // Act & Assert
                //Assert.Equal(fakeTimeZones, mock.Mock<IShellViewModel>().Object.TimeZones);
                Assert.Equal(expected, actual);
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
                var testValue = AnimeListType.AiringAndWatching;
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
                var testValue = AnimeType.ONA;
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
                var testValue = It.IsAny<ISortedAnimeListsViewModel>();

                // Act
                cls.SortedAnimeLists = testValue;

                // Assert
                Assert.Equal(testValue, cls.SortedAnimeLists);
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
