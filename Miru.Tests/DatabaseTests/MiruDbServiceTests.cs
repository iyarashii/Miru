// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using Autofac;
using Autofac.Extras.Moq;
using JikanDotNet;
using MiruDatabaseLogicLayer;
using MiruLibrary;
using MiruLibrary.Models;
using MiruLibrary.Services;
using Moq;
using MyInternetConnectionLibrary;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using AnimeType = MiruLibrary.AnimeType;

namespace Miru.Tests.DatabaseTests
{
    public class MiruDbServiceTests
    {
        private IMiruDbService SetupMiruDbServiceMock(Mock<IMiruDbContext> mockContext, AutoMock mock,
                                                     [Optional] IQueryable<SyncedMyAnimeListUser> userDbSetData,
                                                     [Optional] List<AnimeListEntry> currentUserAnimeEntryList,
                                                     [Optional] List<AnimeSubEntry> currentSeasonList,
                                                     [Optional] bool fakeConnectionIssues,
                                                     [Optional] IQueryable<MiruAnimeModel> miruAnimeModelDbSetData)
        {
            return SetupMiruDbServiceMock(mockContext, mock, userDbSetData, 
                currentUserAnimeEntryList, currentSeasonList, fakeConnectionIssues, miruAnimeModelDbSetData, out _);
        }
        private IMiruDbService SetupMiruDbServiceMock(Mock<IMiruDbContext> mockContext, AutoMock mock,
                                                     [Optional] IQueryable<SyncedMyAnimeListUser> userDbSetData, 
                                                     [Optional] List<AnimeListEntry> currentUserAnimeEntryList,
                                                     [Optional] List<AnimeSubEntry> currentSeasonList,
                                                     [Optional] bool fakeConnectionIssues,
                                                     [Optional] IQueryable<MiruAnimeModel> miruAnimeModelDbSetData,                                                     
                                                     out IMiruDbContext miruDbContext)
        {
            mockContext.Setup(s => s.ExecuteSqlCommand("TRUNCATE TABLE [MiruAnimeModels]")).Returns(0);
            mockContext.Setup(s => s.ExecuteSqlCommand("TRUNCATE TABLE [SyncedMyAnimeListUsers]")).Returns(0);

            // https://docs.microsoft.com/en-us/ef/ef6/fundamentals/testing/mocking
            // wire up the IQueryable implementation for the DbSet more info in the link above
            if (userDbSetData != null)
            {
                var mockUserSet = new Mock<DbSet<SyncedMyAnimeListUser>>();
                mockUserSet.As<IQueryable<SyncedMyAnimeListUser>>().Setup(m => m.Provider).Returns(userDbSetData.Provider);
                mockUserSet.As<IQueryable<SyncedMyAnimeListUser>>().Setup(m => m.Expression).Returns(userDbSetData.Expression);
                mockUserSet.As<IQueryable<SyncedMyAnimeListUser>>().Setup(m => m.ElementType).Returns(userDbSetData.ElementType);
                mockUserSet.As<IQueryable<SyncedMyAnimeListUser>>().Setup(m => m.GetEnumerator()).Returns(userDbSetData.GetEnumerator());
                mockContext.Setup(m => m.SyncedMyAnimeListUsers).Returns(mockUserSet.Object); 
            }

            if (miruAnimeModelDbSetData != null)
            {
                var mockAnimeModelSet = new Mock<DbSet<MiruAnimeModel>>();
                mockAnimeModelSet.As<IQueryable<MiruAnimeModel>>().Setup(m => m.Provider).Returns(miruAnimeModelDbSetData.Provider);
                mockAnimeModelSet.As<IQueryable<MiruAnimeModel>>().Setup(m => m.Expression).Returns(miruAnimeModelDbSetData.Expression);
                mockAnimeModelSet.As<IQueryable<MiruAnimeModel>>().Setup(m => m.ElementType).Returns(miruAnimeModelDbSetData.ElementType);
                mockAnimeModelSet.As<IQueryable<MiruAnimeModel>>().Setup(m => m.GetEnumerator()).Returns(miruAnimeModelDbSetData.GetEnumerator());
                mockContext.Setup(m => m.MiruAnimeModels).Returns(mockAnimeModelSet.Object);
            }

            var mockEventHandler = new Mock<EventHandler<DateTime>>();
            var mockUsernameEventHandler = new Mock<EventHandler<string>>();
            var mockSyncProgressEventHandler = new Mock<EventHandler<int>>();
            var mockUpdateAppStatusEventHandler = new Mock<MiruDbService.UpdateAppStatusEventHandler>();

            Func<IMiruDbContext> mockFunc = () => { return mockContext.Object; };
            Func<IWebClientWrapper> mockWebClientFunc = () => { return Mock.Of<IWebClientWrapper>(); };

            var syncedMyAnimeListUser = Mock.Of<SyncedMyAnimeListUser>();

            var webServiceMock = new Mock<IWebService>();
            webServiceMock.Setup(x => x.CreateWebClient).Returns(mockWebClientFunc);

            var currentUserAnimeListMock = new Mock<ICurrentUserAnimeListModel>();
            var currentSeasonListMock = new Mock<ICurrentSeasonModel>();
            if (currentUserAnimeEntryList == null && currentSeasonList == null)
            {
                currentUserAnimeListMock.Setup(x => x.UserAnimeListData);
                webServiceMock.Setup(x => x.TryToGetAnimeInfo(It.IsAny<long>(), It.IsAny<int>(), It.IsAny<IJikan>())).Throws(new NoInternetConnectionException());
            }
            else if (currentSeasonList != null)
            {
                foreach (var animeEntry in currentSeasonList)
                {
                    webServiceMock.Setup(x => x.TryToGetAnimeInfo(animeEntry.MalId, It.IsAny<int>(), It.IsAny<IJikan>())).ReturnsAsync(new Anime()
                    { Title = animeEntry.Title, Type = animeEntry.Type, MalId = animeEntry.MalId, Broadcast = "Sundays at 10:00 (JST)", ImageURL = "\\dydo", });
                }

                currentSeasonListMock.Setup(x => x.GetFilteredSeasonList()).Returns(currentSeasonList);
            }
            else
            {
                foreach (var animeEntry in currentUserAnimeEntryList)
                {
                    webServiceMock.Setup(x => x.TryToGetAnimeInfo(animeEntry.MalId, It.IsAny<int>(), It.IsAny<IJikan>())).ReturnsAsync(new Anime()
                    { Title = animeEntry.Title, Type = animeEntry.Type, MalId = animeEntry.MalId, Broadcast = "Sundays at 10:00 (JST)", ImageURL = "\\dydo", });
                }

                currentUserAnimeListMock.Setup(x => x.UserAnimeListData).Returns(new UserAnimeList() { Anime = currentUserAnimeEntryList });
            }

            if (fakeConnectionIssues)
            {
                webServiceMock.Setup(x => x.TryToGetAnimeInfo(39, It.IsAny<int>(), It.IsAny<IJikan>())).Throws(new NoInternetConnectionException());
            }

            var fileServiceMockActual = new Mock<IFileSystemService>();
            fileServiceMockActual.Setup(x => x.DownloadFile(It.IsAny<IWebClientWrapper>(), It.IsAny<string>(), It.IsAny<string>()));
            var iFile = new Mock<IFile>();
            var fileSystemMock = new Mock<IFileSystem>();
            iFile.Setup(z => z.ReadAllText(It.IsAny<string>())).Returns(@"{
                                                                            ""Items"": [
                                                                                        {
                                                                                            ""MALID"": 40507,
                                                                                            ""airdate"": ""13/1/2022 23:30""
                                                                                        }
                                                                            ]
                                                                        }");
            fileSystemMock.Setup(y => y.File).Returns(iFile.Object);
            fileServiceMockActual.Setup(x => x.FileSystem).Returns(fileSystemMock.Object);
            var fileServiceMock = new Lazy<IFileSystemService>(() => fileServiceMockActual.Object);
            var currentUserAnimeList = currentUserAnimeListMock.Object;
            var currentSeasonModel = currentSeasonListMock.Object;
            var webService = webServiceMock.Object;
            var fileSystemService = fileServiceMock;
            var userDataService = mock.Create<UserDataService>(
                                                new NamedParameter("currentUserAnimeListModel", currentUserAnimeList),
                                                new NamedParameter("currentSeasonModel", currentSeasonModel), 
                                                new NamedParameter("syncedMyAnimeListUser", syncedMyAnimeListUser));

            var cls = mock.Create<MiruDbService>(new NamedParameter("userDataService", userDataService), 
                                                new NamedParameter("createMiruDbContext", mockFunc), 
                                                new NamedParameter("fileSystemService", fileSystemService), 
                                                new NamedParameter("webService", webService));

            miruDbContext = mockContext.Object;

            cls.UpdateSyncDate += mockEventHandler.Object;
            cls.UpdateCurrentUsername += mockUsernameEventHandler.Object;
            cls.UpdateSyncProgress += mockSyncProgressEventHandler.Object;
            cls.UpdateAppStatusUI += mockUpdateAppStatusEventHandler.Object;

            return cls;
        }

        [Fact]
        public void LoadLastSyncedData_SetsCorrectValuesFromDb()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var mockContext = new Mock<IMiruDbContext>();
                var testSyncDate = It.IsAny<DateTime>();
                var testUsername = "some text to make property see change";
                var data = new List<SyncedMyAnimeListUser>
                {
                    new SyncedMyAnimeListUser { Username = testUsername, SyncTime = testSyncDate },
                }.AsQueryable();

                var cls = SetupMiruDbServiceMock(mockContext, mock, data);

                // Act
                cls.LoadLastSyncedData();

                // Assert
                Assert.Equal(testSyncDate, cls.SyncDateData);
                Assert.Equal(testUsername, cls.CurrentUsername);
            }
        }

        [Fact]
        public void LoadLastSyncedData_WhenSyncedMyAnimeListUsersIsEmpty_DoesNotSetProperties()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var mockContext = new Mock<IMiruDbContext>();
                DateTime testSyncDate = default;
                var data = new List<SyncedMyAnimeListUser>().AsQueryable();

                var cls = SetupMiruDbServiceMock(mockContext, mock, data);

                // Act
                cls.LoadLastSyncedData();

                // Assert
                Assert.Equal(testSyncDate, cls.SyncDateData);
                Assert.Null(cls.CurrentUsername);
            }
        }

        // this test is just an example how you could test private properties set by constructor
        [Fact]
        public void MiruDbService_ConstructorSetsPropertiesCorrectly()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var userDataService = new Mock<IUserDataService>();
                var jikanWrapper = new Mock<IJikan>();
                var createMiruDbContext = new Mock<Func<IMiruDbContext>>();
                var webService = new Mock<IWebService>();
                var fileSystemService = new Mock<Lazy<IFileSystemService>>();
                var createMiruAnimeModel = new Mock<Func<MiruAnimeModel>>();

                Type clsType = typeof(MiruDbService);
                var privateProperties = clsType.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance).ToList();

                // Act
                var cls = new MiruDbService(userDataService.Object, jikanWrapper.Object, createMiruDbContext.Object, webService.Object,
                    fileSystemService.Object, createMiruAnimeModel.Object);

                // Assert
                Assert.Equal(userDataService.Object.CurrentSeasonModel, cls.CurrentSeason);
                Assert.Equal(userDataService.Object.CurrentUserAnimeListModel, cls.CurrentUserAnimeList);
                Assert.Equal(webService.Object, cls.WebService);
                Assert.Equal(jikanWrapper.Object, privateProperties.Where(x => x.Name == "JikanWrapper").First().GetValue(cls));
                Assert.Equal(userDataService.Object.SyncedMyAnimeListUser, privateProperties.Where(x => x.Name == "SyncedMyAnimeListUser").First().GetValue(cls));
                Assert.Equal(createMiruDbContext.Object, privateProperties.Where(x => x.Name == "CreateMiruDbContext").First().GetValue(cls));
                Assert.Equal(fileSystemService.Object, privateProperties.Where(x => x.Name == "FileSystemService").First().GetValue(cls));
                Assert.Equal(createMiruAnimeModel.Object, privateProperties.Where(x => x.Name == "CreateMiruAnimeModel").First().GetValue(cls));
            }
        }

        [Fact]
        public void ClearDb_TruncatesTables()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var mockContext = new Mock<IMiruDbContext>();
                var cls = SetupMiruDbServiceMock(mockContext, mock);

                // Act
                cls.ClearDb();

                // Assert
                mockContext.Verify(x => x.ExecuteSqlCommand("TRUNCATE TABLE [MiruAnimeModels]"), Times.Once);
                mockContext.Verify(x => x.ExecuteSqlCommand("TRUNCATE TABLE [SyncedMyAnimeListUsers]"), Times.Once);
            }
        }

        [Fact]
        public void ChangeDisplayedAnimeList_ShouldFire_UpdateAnimeListEntriesUI_Event()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var eventExecuted = false;
                var mockContext = new Mock<IMiruDbContext>();
                var data = new List<MiruAnimeModel>
                {
                    new MiruAnimeModel {Title = "nnn" },
                }.AsQueryable();
                var cls = SetupMiruDbServiceMock(mockContext, mock, miruAnimeModelDbSetData: data);
                cls.UpdateAnimeListEntriesUI += (x, y) => eventExecuted = true;

                // Act
                cls.ChangeDisplayedAnimeList(It.IsAny<AnimeListType>(), It.IsAny<TimeZoneInfo>(), It.IsAny<MiruLibrary.AnimeType>(), It.IsAny<string>());

                // Assert
                Assert.True(eventExecuted);
            }
        }

        [Theory]
        [InlineData("tako", 2, AnimeType.TV)]
        [InlineData("tako", 3, AnimeType.Both)]
        [InlineData("tako", 1, AnimeType.ONA)]
        [InlineData("gura", 4, AnimeType.Both)]
        [InlineData("gura", 2, AnimeType.TV)]
        [InlineData("gura", 2, AnimeType.ONA)]
        [InlineData("YMD", 0, AnimeType.Both)]
        public void GetFilteredUserAnimeList_AllFilters_WorkCorrectly(string title, 
            int expectedFilteredListSize, AnimeType broadcastType)
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var mockContext = new Mock<IMiruDbContext>();
                var data = new List<MiruAnimeModel>
                {
                    new MiruAnimeModel { JSTBroadcastTime = null, Title = "tako", Type = "TV"},
                    new MiruAnimeModel { JSTBroadcastTime = null, Title = "takodachi", Type = "TV"},
                    new MiruAnimeModel { JSTBroadcastTime = null, Title = "gura", Type = "ONA"},
                    new MiruAnimeModel { JSTBroadcastTime = null, Title = "gura", Type = "TV"},
                    new MiruAnimeModel { JSTBroadcastTime = null, Title = "guraxxx", Type = "TV"},
                    new MiruAnimeModel { JSTBroadcastTime = null, Title = "gura123", Type = "ONA"},
                    new MiruAnimeModel { JSTBroadcastTime = null, Title = "tako", Type = "ONA"},
                }.AsQueryable();
                var cls = SetupMiruDbServiceMock(mockContext, mock, miruAnimeModelDbSetData: data, miruDbContext: out IMiruDbContext db);
                var converter = new EnumDescriptionTypeConverter(typeof(AnimeType));
                var animeBroadcastTypeDescription = converter.ConvertToString(broadcastType);

                // Act
                var result = cls.GetFilteredUserAnimeList(db, broadcastType, title, It.IsAny<TimeZoneInfo>());

                // Assert
                Assert.All(result, x => Assert.Contains(title, x.Title));
                Assert.All(result, x => Assert.Contains(x.Type, animeBroadcastTypeDescription));
                Assert.All(result, x => Assert.Equal(DateTime.Today, x.LocalBroadcastTime));
                Assert.Equal(expectedFilteredListSize, result.Count);
            }
        }

        public static IEnumerable<object[]> TruncateCalledTimesData => new List<object[]>
        {
            new object[] { Times.Once(), new List<SyncedMyAnimeListUser> // not empty list => truncate called 1 time
                                         {
                                             new SyncedMyAnimeListUser { Username = It.IsAny<string>(), SyncTime = It.IsAny<DateTime>()},
                                         }.AsQueryable() },
            new object[] { Times.Never(), new List<SyncedMyAnimeListUser>().AsQueryable() } // empty list => truncate never called
        };

        [Theory]
        [MemberData(nameof(TruncateCalledTimesData))]
        public void SaveSyncedUserData_GivenSyncedUsersListNotEmpty_ShouldTruncateTable(Times truncateTimesExecuted, IQueryable<SyncedMyAnimeListUser> usersData)
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var mockContext = new Mock<IMiruDbContext>();
                var cls = SetupMiruDbServiceMock(mockContext, mock, userDbSetData: usersData);

                // Act
                cls.SaveSyncedUserData(It.IsAny<string>()).Wait();

                // Assert
                mockContext.Verify(x => x.ExecuteSqlCommand("TRUNCATE TABLE [SyncedMyAnimeListUsers]"), truncateTimesExecuted);
            }
        }

        [Fact]
        public void SaveSyncedUserData_ShouldAddSyncedUser()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var mockContext = new Mock<IMiruDbContext>();
                var usersData = new List<SyncedMyAnimeListUser>().AsQueryable();
                var cls = SetupMiruDbServiceMock(mockContext, mock, userDbSetData: usersData);
                var testUsername = "UwU";

                // Act
                cls.SaveSyncedUserData(testUsername).Wait();

                // Assert
                mockContext.Verify(x => x.SyncedMyAnimeListUsers.Add(It.Is<SyncedMyAnimeListUser>(y => y.Username == testUsername && y.SyncTime == cls.SyncDateData)), Times.Once());
                mockContext.Verify(x => x.SaveChangesAsync(), Times.Once());
            }
        }

        [Theory]
        [InlineData(true, 1)]
        [InlineData(false, 2)]
        public void SaveDetailedAnimeListData_GivenConnectionIssues_ReturnFalse(bool connectionIssues, int expectedTimesEventFired)
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var mockContext = new Mock<IMiruDbContext>();
                var data = new List<MiruAnimeModel>
                {
                }.AsQueryable();
                var entryList = new List<AnimeListEntry>() { new AnimeListEntry() { MalId = 39, AiringStatus = AiringStatus.Airing, WatchedEpisodes = 5, TotalEpisodes = 10, Type = "TV", Title = "10", } };
                var cls = SetupMiruDbServiceMock(mockContext, mock, miruAnimeModelDbSetData: data, currentUserAnimeEntryList: entryList, fakeConnectionIssues: connectionIssues);
                int eventExecutedTimes = 0;
                cls.UpdateAppStatusUI += (x, y) => ++eventExecutedTimes;
                var timesCalled = connectionIssues ? Times.Never() : Times.Once();

                // Act
                var result = cls.SaveDetailedAnimeListData(It.IsAny<bool>()).Result;

                // Assert
                Assert.Equal(expectedTimesEventFired, eventExecutedTimes);
                mockContext.Verify(x => x.ExecuteSqlCommand("TRUNCATE TABLE [MiruAnimeModels]"), timesCalled);
                mockContext.Verify(x => x.MiruAnimeModels.AddRange(It.IsAny<List<MiruAnimeModel>>()), timesCalled);
                mockContext.Verify(x => x.SaveChangesAsync(), timesCalled);
                Assert.Equal(!connectionIssues, result);
            }
        }

        [Fact]
        public void SaveDetailedAnimeListData_ParseTimeFromBroadcast_SetsCorrectBroadcastTimes()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var mockContext = new Mock<IMiruDbContext>();
                var data = new List<MiruAnimeModel>
                {
                    new MiruAnimeModel {Title = "10", Type = "TV", MalId = 1, Broadcast = "Sundays at 10:00 (JST)",},
                }.AsQueryable();

                var animeEntryList = new List<AnimeListEntry>()
                {
                    new AnimeListEntry() { MalId = 1,  AiringStatus = AiringStatus.Airing, WatchedEpisodes = 5, TotalEpisodes = 10, Type = "TV", Title = "10", }
                };

                var cls = SetupMiruDbServiceMock(mockContext, mock, miruAnimeModelDbSetData: data, currentUserAnimeEntryList: animeEntryList);
                var date = new DateTime(2020, 01, 26, 10, 0, 0);
                cls.UpdateAppStatusUI += (x, y) => x.ToString();
                var testModel = new MiruAnimeModel { Title = "10", Type = "TV", MalId = 1, Broadcast = "Sundays at 10:00 (JST)", JSTBroadcastTime = date };

                // Act
                cls.SaveDetailedAnimeListData(It.IsAny<bool>()).Wait();
                testModel.ConvertJstBroadcastTimeToSelectedTimeZone(TimeZoneInfo.Local);

                // Assert
                mockContext.Verify(x => x.MiruAnimeModels
                .AddRange(It.Is<List<MiruAnimeModel>>(
                    y => y.Select(z => z.JSTBroadcastTime == date 
                    && z.LocalBroadcastTime == testModel.LocalBroadcastTime)
                    .ToList().Any())), Times.Once());
            }
        }

        [Theory]
        [InlineData(5, 10, AiringStatus.Airing, 1337L, AiringStatus.Completed, "TAKOTIME", 69, 1337, "ONA", "WAH")]
        [InlineData(15, 210, AiringStatus.Upcoming, 1337L, AiringStatus.Airing, "GOOMBAH", 69, 1337, "TV", "url Uwu")]
        public void GetDetailedUserAnimeList_GivenNotNullUserAnimeListEntries_ReturnsDetailedList(
            // 1st entry updated exisiting
            int expectedUpdatedWatchedEpisodesValue, 
            int expectedUpdatedTotalEpisodesValue, 
            AiringStatus updatedAiringStatus, 
            // 2nd entry -- new one
            long expectedMalIdOfNewEntry, 
            AiringStatus expectedAiringStatusOfNewEntry, 
            string  titleOfNewEntry,
            int watchedEpisodesOfNewEntry, 
            int totalEpisodesOfNewEntry, 
            string typeOfNewEntry,
            string urlOfNewEntry)
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var mockContext = new Mock<IMiruDbContext>();
                var date = new DateTime(2020, 01, 26, 10, 0, 0);
                var testModel = new MiruAnimeModel { Title = "initial title", Type = "TV", MalId = 1L, Broadcast = "Sundays at 10:00 (JST)", JSTBroadcastTime = date, 
                    IsOnWatchingList = false, WatchedEpisodes = 0, TotalEpisodes = 20, CurrentlyAiring = false, ImageURL = "stays the same", URL = "same", LocalImagePath = "\\same" };
                var data = new List<MiruAnimeModel>
                {
                    testModel,
                }.AsQueryable();

                var animeEntryList = new List<AnimeListEntry>()
                {
                    new AnimeListEntry() { MalId = 1L,  AiringStatus = updatedAiringStatus, WatchedEpisodes = expectedUpdatedWatchedEpisodesValue, 
                        TotalEpisodes = expectedUpdatedTotalEpisodesValue, Type = "TV", Title = "changed" },

                    new AnimeListEntry() { MalId = expectedMalIdOfNewEntry,  AiringStatus = expectedAiringStatusOfNewEntry, 
                        WatchedEpisodes = watchedEpisodesOfNewEntry, TotalEpisodes = totalEpisodesOfNewEntry, Type = typeOfNewEntry, Title = titleOfNewEntry, URL = urlOfNewEntry }
                };

                var cls = SetupMiruDbServiceMock(mockContext, mock, miruDbContext: out IMiruDbContext db, currentUserAnimeEntryList: animeEntryList, miruAnimeModelDbSetData: data);

                // Act
                var result = cls.GetDetailedUserAnimeList(db, cls.CurrentUserAnimeList.UserAnimeListData.Anime, It.IsAny<bool>()).Result;
                var resultUpdatedEntry = result.First(x => x == testModel);
                var resultNewEntry = result.First(x => x != testModel);

                // Assert
                Assert.Equal(2, result.Count());
                // check updated anime entry - only 4 values should change
                Assert.True(resultUpdatedEntry.IsOnWatchingList); // should change to true
                Assert.Equal(updatedAiringStatus == AiringStatus.Airing, resultUpdatedEntry.CurrentlyAiring); // should change depending on airing status
                Assert.Equal(expectedUpdatedWatchedEpisodesValue, resultUpdatedEntry.WatchedEpisodes); // should change to new value
                Assert.Equal(expectedUpdatedTotalEpisodesValue, resultUpdatedEntry.TotalEpisodes); // should change to new value
                // check if rest of the values for updated entry stay the same
                Assert.Equal(1L, resultUpdatedEntry.MalId);
                Assert.Equal("Sundays at 10:00 (JST)", resultUpdatedEntry.Broadcast);
                Assert.Equal("initial title", resultUpdatedEntry.Title);
                Assert.Equal("stays the same", resultUpdatedEntry.ImageURL);
                Assert.Equal("\\same", resultUpdatedEntry.LocalImagePath);
                Assert.Equal("same", resultUpdatedEntry.URL);
                Assert.Equal("TV", resultUpdatedEntry.Type);

                // check new entry
                Assert.Equal(expectedMalIdOfNewEntry, resultNewEntry.MalId);
                Assert.Equal("Sundays at 10:00 (JST)", resultNewEntry.Broadcast); // it is set in webServiceMock
                Assert.Equal(titleOfNewEntry, resultNewEntry.Title);
                Assert.Equal("\\dydo", resultNewEntry.ImageURL); // it is set in webServiceMock
                Assert.Equal(Path.Combine(Constants.ImageCacheFolderPath, $"{ expectedMalIdOfNewEntry }.jpg"), resultNewEntry.LocalImagePath);
                Assert.Equal(totalEpisodesOfNewEntry, resultNewEntry.TotalEpisodes);
                Assert.Equal(urlOfNewEntry, resultNewEntry.URL);
                Assert.Equal(watchedEpisodesOfNewEntry, resultNewEntry.WatchedEpisodes);
                Assert.True(resultNewEntry.IsOnWatchingList);
                Assert.Equal(expectedAiringStatusOfNewEntry == AiringStatus.Airing, resultNewEntry.CurrentlyAiring);
                Assert.Equal(typeOfNewEntry, resultNewEntry.Type);
            }
        }

        [Fact]
        public void GetDetailedSeasonAnimeListInfo_GivenAnimeList_ReturnsUpdatedList()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var mockContext = new Mock<IMiruDbContext>();
                var date = new DateTime(2020, 01, 26, 10, 0, 0);
                var testModel = new MiruAnimeModel
                {
                    Title = "initial title",
                    Type = "TV",
                    MalId = 1L,
                    Broadcast = "Sundays at 10:00 (JST)",
                    JSTBroadcastTime = date,
                    IsOnWatchingList = false,
                    WatchedEpisodes = 0,
                    TotalEpisodes = 20,
                    CurrentlyAiring = false,
                    ImageURL = "stays the same",
                    URL = "same",
                    LocalImagePath = "\\same"
                };
                var data = new List<MiruAnimeModel>
                {
                    testModel,
                }.AsQueryable();

                var seasonAnimeEntryList = new List<AnimeSubEntry>()
                {
                    new AnimeSubEntry() { MalId = 1, Title = "old entry", Type = "ONA", URL = "only value from seasonEntry" },
                    new AnimeSubEntry() { MalId = 39, Title = "new season entry", Type = "TV", URL = "only value from seasonEntry" }
                };

                var animeEntryList = new List<AnimeListEntry>()
                {
                    
                };

                var cls = SetupMiruDbServiceMock(mockContext, mock, miruDbContext: out IMiruDbContext db, currentUserAnimeEntryList: animeEntryList, 
                    currentSeasonList: seasonAnimeEntryList, miruAnimeModelDbSetData: data);

                // Act
                var result = cls.GetDetailedUserAnimeList(db, animeEntryList, true).Result;

                var resultNewEntry = result.First(x => x != testModel);
                var resultUpdatedEntry = result.First(x => x == testModel);

                // Assert
                Assert.True(resultUpdatedEntry.CurrentlyAiring);

                // check if rest of the values for updated entry stay the same
                Assert.False(resultUpdatedEntry.IsOnWatchingList);
                Assert.Equal(1L, resultUpdatedEntry.MalId);
                Assert.Equal("Sundays at 10:00 (JST)", resultUpdatedEntry.Broadcast);
                Assert.Equal("initial title", resultUpdatedEntry.Title);
                Assert.Equal("stays the same", resultUpdatedEntry.ImageURL);
                Assert.Equal("\\same", resultUpdatedEntry.LocalImagePath);
                Assert.Equal("same", resultUpdatedEntry.URL);
                Assert.Equal("TV", resultUpdatedEntry.Type);

                // check new entry
                Assert.Equal(39, resultNewEntry.MalId);
                Assert.Equal("Sundays at 10:00 (JST)", resultNewEntry.Broadcast); // it is set in webServiceMock
                Assert.Equal("new season entry", resultNewEntry.Title);
                Assert.Equal("\\dydo", resultNewEntry.ImageURL); // it is set in webServiceMock
                Assert.Equal(Path.Combine(Constants.ImageCacheFolderPath, $"{ 39 }.jpg"), resultNewEntry.LocalImagePath);
                Assert.Equal("only value from seasonEntry", resultNewEntry.URL);
                Assert.False(resultNewEntry.IsOnWatchingList);
                Assert.True(resultNewEntry.CurrentlyAiring);
                Assert.Equal("TV", resultNewEntry.Type);
            }
        }

        [Fact]
        public void GetDetailedSeasonAnimeListInfo_GivenConnectionIssuesWithGettingAnimeInfo_ReturnsNull()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var mockContext = new Mock<IMiruDbContext>();
                var date = new DateTime(2020, 01, 26, 10, 0, 0);
                var testModel = new MiruAnimeModel
                {
                    Title = "initial title",
                    Type = "TV",
                    MalId = 1L,
                    Broadcast = "Sundays at 10:00 (JST)",
                    JSTBroadcastTime = date,
                    IsOnWatchingList = false,
                    WatchedEpisodes = 0,
                    TotalEpisodes = 20,
                    CurrentlyAiring = false,
                    ImageURL = "stays the same",
                    URL = "same",
                    LocalImagePath = "\\same"
                };
                var data = new List<MiruAnimeModel>
                {
                    testModel,
                }.AsQueryable();

                var seasonAnimeEntryList = new List<AnimeSubEntry>()
                {
                    new AnimeSubEntry() { MalId = 39, Title = "new season entry", Type = "TV", URL = "only value from seasonEntry" }
                };

                var animeEntryList = new List<AnimeListEntry>()
                {

                };

                var cls = SetupMiruDbServiceMock(mockContext, mock, miruDbContext: out IMiruDbContext db, currentUserAnimeEntryList: animeEntryList,
                    currentSeasonList: seasonAnimeEntryList, miruAnimeModelDbSetData: data, fakeConnectionIssues: true);

                // Act
                var result = cls.GetDetailedUserAnimeList(db, animeEntryList, true).Result;

                // Assert
                Assert.Null(result);
            }
        }

   }
}
