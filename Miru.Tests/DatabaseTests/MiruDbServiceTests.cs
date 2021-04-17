using Autofac;
using Autofac.Extras.Moq;
using MiruDatabaseLogicLayer;
using MiruLibrary.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Miru.Tests.DatabaseTests
{
    public class MiruDbServiceTests
    {
        [Fact]
        public void LoadLastSyncedData_SetsCorrectValuesFromDb()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var testSyncDate = It.IsAny<DateTime>();
                var testUsername = "some text to make property see change";
                var data = new List<SyncedMyAnimeListUser>
                {
                    new SyncedMyAnimeListUser { Username = testUsername, SyncTime = testSyncDate },
                }.AsQueryable();

                // https://docs.microsoft.com/en-us/ef/ef6/fundamentals/testing/mocking
                // wire up the IQueryable implementation for the DbSet more info in the link above
                var mockUserSet = new Mock<DbSet<SyncedMyAnimeListUser>>();
                mockUserSet.As<IQueryable<SyncedMyAnimeListUser>>().Setup(m => m.Provider).Returns(data.Provider);
                mockUserSet.As<IQueryable<SyncedMyAnimeListUser>>().Setup(m => m.Expression).Returns(data.Expression);
                mockUserSet.As<IQueryable<SyncedMyAnimeListUser>>().Setup(m => m.ElementType).Returns(data.ElementType);
                mockUserSet.As<IQueryable<SyncedMyAnimeListUser>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

                var mockContext = new Mock<MiruDbContext>();
                mockContext.Setup(m => m.SyncedMyAnimeListUsers).Returns(mockUserSet.Object);
                var mockEventHandler = new Mock<EventHandler<DateTime>>();
                var mockUsernameEventHandler = new Mock<EventHandler<string>>();

                Func<IMiruDbContext> mockFunc = () => { return mockContext.Object; };

                var cls = mock.Create<MiruDbService>(new NamedParameter("createMiruDbContext", mockFunc));
                cls.UpdateSyncDate += mockEventHandler.Object;
                cls.UpdateCurrentUsername += mockUsernameEventHandler.Object;
                //cls.UpdateSyncDate += new EventHandler<DateTime>((x, y) => { });

                // Act
                cls.LoadLastSyncedData();

                // Assert
                Assert.Equal(testSyncDate, cls.SyncDateData);
                Assert.Equal(testUsername, cls.CurrentUsername);
            }
        }
    }
}
