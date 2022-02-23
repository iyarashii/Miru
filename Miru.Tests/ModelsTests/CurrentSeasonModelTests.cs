using Autofac.Extras.Moq;
using JikanDotNet;
using MiruLibrary.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Miru.Tests.ModelsTests
{
    public class CurrentSeasonModelTests
    {
        [Fact]
        public async void GetCurrentSeasonList_OnHttpRequestException_ReturnsFalse()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                mock.Mock<IJikan>().Setup(x => x.GetSeason()).ThrowsAsync(new HttpRequestException());
                var sut = mock.Create<CurrentSeasonModel>();

                // Act
                var result = await sut.GetCurrentSeasonList(It.IsAny<int>());

                // Assert
                Assert.False(result);
            }
        }

        [Fact]
        public async void GetCurrentSeasonList_OnSeasonDataGet_ReturnsTrue()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                mock.Mock<IJikan>().Setup(x => x.GetSeason()).ReturnsAsync(new Season());
                var sut = mock.Create<CurrentSeasonModel>();

                // Act
                var result = await sut.GetCurrentSeasonList(It.IsAny<int>());

                // Assert
                Assert.True(result);
            }
        }
    }
}
