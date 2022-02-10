﻿using Autofac;
using Autofac.Extras.Moq;
using JikanDotNet;
using MiruDatabaseLogicLayer;
using MiruLibrary;
using MiruLibrary.Models;
using Moq;
using MyInternetConnectionLibrary;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
                                                     [Optional] bool currentUserAnimeListEmpty,
                                                     [Optional] IQueryable<MiruAnimeModel> miruAnimeModelDbSetData)
        {
            return SetupMiruDbServiceMock(mockContext, mock, userDbSetData, currentUserAnimeListEmpty, miruAnimeModelDbSetData, out _);
        }
        private IMiruDbService SetupMiruDbServiceMock(Mock<IMiruDbContext> mockContext, AutoMock mock,
                                                     [Optional] IQueryable<SyncedMyAnimeListUser> userDbSetData, 
                                                     [Optional] bool currentUserAnimeListEmpty, 
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

            Func<IMiruDbContext> mockFunc = () => { return mockContext.Object; };
            Func<IWebClientWrapper> mockWebClientFunc = () => { return Mock.Of<IWebClientWrapper>(); };

            var syncedMyAnimeListUser = Mock.Of<SyncedMyAnimeListUser>();

            var webServiceMock = new Mock<IWebService>();
            webServiceMock.Setup(x => x.CreateWebClient).Returns(mockWebClientFunc);

            var currentUserAnimeListMock = new Mock<ICurrentUserAnimeListModel>();
            if (currentUserAnimeListEmpty)
            {
                currentUserAnimeListMock.Setup(x => x.UserAnimeListData);
                webServiceMock.Setup(x => x.TryToGetAnimeInfo(It.IsAny<long>(), It.IsAny<int>(), It.IsAny<IJikan>())).Throws(new NoInternetConnectionException());
            }
            else
            {
                webServiceMock.Setup(x => x.TryToGetAnimeInfo(It.IsAny<long>(), It.IsAny<int>(), It.IsAny<IJikan>())).ReturnsAsync(new Anime() 
                { Title = "10", Type = "TV", MalId = 1, Broadcast = "Sundays at 10:00 (JST)", ImageURL = "dydo",});

                currentUserAnimeListMock.Setup(x => x.UserAnimeListData).Returns(new UserAnimeList() { Anime = new List<AnimeListEntry>()
                { 
                    new AnimeListEntry() { MalId = 1,  AiringStatus = AiringStatus.Airing, WatchedEpisodes = 5, TotalEpisodes = 10} 
                } });
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
            var webService = webServiceMock.Object;
            var fileSystemService = fileServiceMock;
            var cls = mock.Create<MiruDbService>(new NamedParameter("currentUserAnimeListModel", currentUserAnimeList), 
                                                new NamedParameter("createMiruDbContext", mockFunc), 
                                                new NamedParameter("syncedMyAnimeListUser", syncedMyAnimeListUser), 
                                                new NamedParameter("fileSystemService", fileSystemService), 
                                                new NamedParameter("webService", webService));

            miruDbContext = mockContext.Object;

            cls.UpdateSyncDate += mockEventHandler.Object;
            cls.UpdateCurrentUsername += mockUsernameEventHandler.Object;

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
                var currentSeasonModel = new Mock<ICurrentSeasonModel>();
                var currentUserAnimeListModel = new Mock<ICurrentUserAnimeListModel>();
                var jikanWrapper = new Mock<IJikan>();
                var createMiruDbContext = new Mock<Func<IMiruDbContext>>();
                var syncedMyAnimeListUser = new Mock<ISyncedMyAnimeListUser>();
                var webService = new Mock<IWebService>();
                var fileSystemService = new Mock<Lazy<IFileSystemService>>();
                var createMiruAnimeModel = new Mock<Func<MiruAnimeModel>>();

                Type clsType = typeof(MiruDbService);
                var privateProperties = clsType.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance).ToList();

                // Act
                var cls = new MiruDbService(currentSeasonModel.Object, currentUserAnimeListModel.Object, 
                    jikanWrapper.Object, createMiruDbContext.Object, syncedMyAnimeListUser.Object, webService.Object,
                    fileSystemService.Object, createMiruAnimeModel.Object);

                // Assert
                Assert.Equal(currentSeasonModel.Object, cls.CurrentSeason);
                Assert.Equal(currentUserAnimeListModel.Object, cls.CurrentUserAnimeList);
                Assert.Equal(webService.Object, cls.WebService);
                Assert.Equal(jikanWrapper.Object, privateProperties.Where(x => x.Name == "JikanWrapper").First().GetValue(cls));
                Assert.Equal(syncedMyAnimeListUser.Object, privateProperties.Where(x => x.Name == "SyncedMyAnimeListUser").First().GetValue(cls));
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


        public static IEnumerable<object[]> GetTimeZoneData() // Tokyo Standard Time = UTC+9
        {
            yield return new object[] { TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time"), TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time").BaseUtcOffset }; // UTC+1
            yield return new object[] { TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time").BaseUtcOffset }; // UTC-5
            yield return new object[] { TimeZoneInfo.FindSystemTimeZoneById("AUS Eastern Standard Time"), TimeZoneInfo.FindSystemTimeZoneById("AUS Eastern Standard Time").GetUtcOffset(DateTime.UtcNow) }; // UTC+10
            yield return new object[] { TimeZoneInfo.FindSystemTimeZoneById("Alaskan Standard Time"), TimeZoneInfo.FindSystemTimeZoneById("Alaskan Standard Time").BaseUtcOffset }; // UTC-9
        }

        [Theory]
        [MemberData(nameof(GetTimeZoneData))]
        public void GetFilteredUserAnimeList_ConvertJstBroadcastTimeToSelectedTimeZone_WorksCorrectly(TimeZoneInfo timeZone, TimeSpan utcOffset)
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var mockContext = new Mock<IMiruDbContext>();
                var data = new List<MiruAnimeModel>
                {
                    new MiruAnimeModel {Title = "10", Type = "TV",  JSTBroadcastTime = new DateTime(2022, 2, 1, 10, 0, 0)},
                    new MiruAnimeModel {Title = "20", Type = "TV",  JSTBroadcastTime = new DateTime(2022, 2, 1, 20, 0, 0)},
                    new MiruAnimeModel {Title = "15", Type = "TV",  JSTBroadcastTime = new DateTime(2022, 2, 1, 15, 0, 0)},
                    new MiruAnimeModel {Title = "0", Type = "TV",  JSTBroadcastTime = new DateTime(2022, 2, 1, 0, 0, 0)},
                }.AsQueryable();
                var cls = SetupMiruDbServiceMock(mockContext, mock, miruAnimeModelDbSetData: data, miruDbContext: out IMiruDbContext db);

                // Act
                var result = cls.GetFilteredUserAnimeList(db, It.IsAny<AnimeType>(), It.IsAny<string>(), timeZone);

                // Assert
                Assert.All(result, x => Assert.Equal(x.JSTBroadcastTime.Value.AddHours(-9.0).Add(utcOffset).Hour, x.LocalBroadcastTime.Value.Hour));
            }
        }

        [Theory]
        [InlineData("tako", 3)]
        [InlineData("gura", 4)]
        [InlineData("YMD", 0)]
        public void GetFilteredUserAnimeList_FilterByTitle_WorksCorrectly(string title, int expectedFilteredListSize)
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var mockContext = new Mock<IMiruDbContext>();
                var data = new List<MiruAnimeModel>
                {
                    new MiruAnimeModel {Title = "tako", Type = "TV"},
                    new MiruAnimeModel {Title = "tako", Type = "TV"},
                    new MiruAnimeModel {Title = "takodachi", Type = "TV"},
                    new MiruAnimeModel {Title = "gura", Type = "TV"},
                    new MiruAnimeModel {Title = "gura", Type = "TV"},
                    new MiruAnimeModel {Title = "guraxxx", Type = "TV"},
                    new MiruAnimeModel {Title = "gura123", Type = "TV"},
                }.AsQueryable();
                var cls = SetupMiruDbServiceMock(mockContext, mock, miruAnimeModelDbSetData: data, miruDbContext: out IMiruDbContext db);

                // Act
                var result = cls.GetFilteredUserAnimeList(db, AnimeType.TV, title, It.IsAny<TimeZoneInfo>());

                // Assert
                Assert.All(result, x => Assert.Contains(title, x.Title));
                Assert.Equal(expectedFilteredListSize, result.Count());
            }
        }

        [Theory]
        [InlineData(AnimeType.TV, 3)]
        [InlineData(AnimeType.ONA, 4)]
        [InlineData(AnimeType.Both, 7)]
        public void GetFilteredUserAnimeList_FilterByBroadcastType_WorksCorrectly(AnimeType broadcastType, int expectedFilteredListSize)
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var mockContext = new Mock<IMiruDbContext>();
                var data = new List<MiruAnimeModel>
                {
                    new MiruAnimeModel {Title = "tako", Type = "TV"},
                    new MiruAnimeModel {Title = "tako", Type = "TV"},
                    new MiruAnimeModel {Title = "tako", Type = "TV"},
                    new MiruAnimeModel {Title = "tako", Type = "ONA"},
                    new MiruAnimeModel {Title = "tako", Type = "ONA"},
                    new MiruAnimeModel {Title = "tako", Type = "ONA"},
                    new MiruAnimeModel {Title = "tako", Type = "ONA"},
                }.AsQueryable();
                var cls = SetupMiruDbServiceMock(mockContext, mock, miruAnimeModelDbSetData: data, miruDbContext: out IMiruDbContext db);

                var converter = new EnumDescriptionTypeConverter(typeof(AnimeType));
                var animeBroadcastTypeDescription = converter.ConvertToString(broadcastType);

                // Act
                var result = cls.GetFilteredUserAnimeList(db, broadcastType, "tako", It.IsAny<TimeZoneInfo>());

                // Assert
                Assert.All(result, x => Assert.Contains(x.Type, animeBroadcastTypeDescription));
                Assert.Equal(expectedFilteredListSize, result.Count());
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
        [InlineData(false, 1)]
        [InlineData(true, 2)]
        public void SaveDetailedAnimeListData_GivenConnectionIssues_ReturnFalse(bool expectedResult, int expectedTimesEventFired)
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var mockContext = new Mock<IMiruDbContext>();
                var data = new List<MiruAnimeModel>
                {
                }.AsQueryable();
                var cls = SetupMiruDbServiceMock(mockContext, mock, miruAnimeModelDbSetData: data, currentUserAnimeListEmpty: !expectedResult);
                int eventExecutedTimes = 0;
                cls.UpdateAppStatusUI += (x, y) => ++eventExecutedTimes;
                var timesCalled = expectedResult ? Times.Once() : Times.Never();

                // Act
                var result = cls.SaveDetailedAnimeListData(It.IsAny<bool>()).Result;

                // Assert
                Assert.Equal(expectedTimesEventFired, eventExecutedTimes);
                mockContext.Verify(x => x.ExecuteSqlCommand("TRUNCATE TABLE [MiruAnimeModels]"), timesCalled);
                mockContext.Verify(x => x.MiruAnimeModels.AddRange(It.IsAny<List<MiruAnimeModel>>()), timesCalled);
                mockContext.Verify(x => x.SaveChangesAsync(), timesCalled);
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
                var cls = SetupMiruDbServiceMock(mockContext, mock, miruAnimeModelDbSetData: data, currentUserAnimeListEmpty: false);
                var date = new DateTime(2020, 01, 26, 10, 0, 0);
                cls.UpdateAppStatusUI += (x, y) => x.ToString();
                var testModel = new MiruAnimeModel { Title = "10", Type = "TV", MalId = 1, Broadcast = "Sundays at 10:00 (JST)", JSTBroadcastTime = date };

                // Act
                cls.SaveDetailedAnimeListData(It.IsAny<bool>()).Wait();
                testModel.ConvertJstBroadcastTimeToSelectedTimeZone(TimeZoneInfo.Local);

                // Assert
                mockContext.Verify(x => x.MiruAnimeModels.AddRange(It.Is<List<MiruAnimeModel>>(y => y.Select(z => z.JSTBroadcastTime == date && z.LocalBroadcastTime == testModel.LocalBroadcastTime).ToList().Any())), Times.Once());
            }
        }
    }
}
