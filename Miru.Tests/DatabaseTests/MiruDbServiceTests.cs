using Autofac;
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
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Miru.Tests.DatabaseTests
{
    public class MiruDbServiceTests
    {
        private IMiruDbService SetupMiruDbServiceMock(Mock<IMiruDbContext> mockContext, AutoMock mock,
                                                     [Optional] IQueryable<SyncedMyAnimeListUser> userDbSetData,
                                                     [Optional] IQueryable<MiruAnimeModel> miruAnimeModelDbSetData,
                                                     [Optional] IMiruAnimeModelExtensionsWrapper mockWrapper)
        {
            return SetupMiruDbServiceMock(mockContext, mock, userDbSetData, miruAnimeModelDbSetData, mockWrapper, out _);
        }
        private IMiruDbService SetupMiruDbServiceMock(Mock<IMiruDbContext> mockContext, AutoMock mock,
                                                     [Optional] IQueryable<SyncedMyAnimeListUser> userDbSetData, 
                                                     [Optional] IQueryable<MiruAnimeModel> miruAnimeModelDbSetData,
                                                     [Optional] IMiruAnimeModelExtensionsWrapper mockWrapper,
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

            var cls = mock.Create<MiruDbService>(new NamedParameter("createMiruDbContext", mockFunc), new NamedParameter("miruAnimeModelExtensionsWrapper", mockWrapper));

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
                var miruAnimeModelExtensionsWrapper = new Mock<MiruAnimeModelExtensionsWrapper>();

                Type clsType = typeof(MiruDbService);
                var privateProperties = clsType.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance).ToList();

                // Act
                var cls = new MiruDbService(currentSeasonModel.Object, currentUserAnimeListModel.Object, 
                    jikanWrapper.Object, createMiruDbContext.Object, syncedMyAnimeListUser.Object, webService.Object,
                    fileSystemService.Object, createMiruAnimeModel.Object, miruAnimeModelExtensionsWrapper.Object);

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
        public void ChangeDisplayedAnimeList_Fires_UpdateAnimeListEntriesUI_Event()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var eventExecuted = false;
                var mockContext = new Mock<IMiruDbContext>();
                var mockWrapper = Mock.Of<IMiruAnimeModelExtensionsWrapper>();
                var data = new List<MiruAnimeModel>
                {
                    new MiruAnimeModel {Title = "nnn" },
                }.AsQueryable();
                var cls = SetupMiruDbServiceMock(mockContext, mock, miruAnimeModelDbSetData: data, mockWrapper: mockWrapper);
                cls.UpdateAnimeListEntriesUI += (x, y) => eventExecuted = true;

                // Act
                cls.ChangeDisplayedAnimeList(It.IsAny<AnimeListType>(), It.IsAny<TimeZoneInfo>(), It.IsAny<MiruLibrary.AnimeType>(), It.IsAny<string>());

                // Assert
                Assert.True(eventExecuted);
            }
        }

        [Fact]
        public void GetFilteredUserAnimeList_CallsCorrectFilters()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var mockContext = new Mock<IMiruDbContext>();
                var mockWrapper = new Mock<IMiruAnimeModelExtensionsWrapper>();
                var data = new List<MiruAnimeModel>
                {
                    new MiruAnimeModel {Title = "nnn" },
                }.AsQueryable();
                var cls = SetupMiruDbServiceMock(mockContext, mock, miruAnimeModelDbSetData: data, miruDbContext: out IMiruDbContext db, mockWrapper: mockWrapper.Object);

                // Act
                cls.GetFilteredUserAnimeList(db, It.IsAny<MiruLibrary.AnimeType>(), It.IsAny<string>(), It.IsAny<TimeZoneInfo>());

                // Assert
                mockWrapper.Verify(x => x.FilterByTitle(It.IsAny<List<MiruAnimeModel>>(), It.IsAny<string>()), Times.Once);
            }
        }
    }
}
