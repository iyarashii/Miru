// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using Autofac;
using Autofac.Extras.Moq;
using JikanDotNet;
using JikanDotNet.Exceptions;
using MiruLibrary.Models;
using MiruLibrary.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Miru.Tests.ModelsTests
{
    public class CurrentSeasonModelTests
    {
        [Fact]
        public async Task GetCurrentSeasonList_OnHttpRequestException_ReturnsFalse()
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
        public async Task GetCurrentSeasonList_OnSeasonDataGet_ReturnsTrue()
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

        [Fact]
        public void GetCurrentSeasonList_OnJikanRequestException_TriplesExecutionDelay()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                mock.Mock<IJikan>().SetupSequence(x => x.GetSeason())
                    .ThrowsAsync(new JikanRequestException())
                    .ReturnsAsync(new Season());
                mock.Mock<ITimerService>().Setup(x => x.DelayTask(It.IsAny<int>())).Returns(Task.CompletedTask);

                var sut = mock.Create<CurrentSeasonModel>();
                int testDelayInMs = 1;

                // Act
                sut.GetCurrentSeasonList(testDelayInMs).Wait();

                // Assert
                mock.Mock<ITimerService>().Verify(x => x.DelayTask(testDelayInMs), Times.Exactly(3));
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        [InlineData(null)]
        public void GetFilteredSeasonList_ReturnsSeasonListWithoutAnimeForKids(bool? forKids)
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var testSeasonData = new Season
                {
                    SeasonEntries = new List<AnimeSubEntry>()
                    {
                        new AnimeSubEntry() {Kids = forKids},
                        new AnimeSubEntry() {Kids = false},
                        new AnimeSubEntry() {Kids = null},
                        new AnimeSubEntry() {Kids = forKids},
                        new AnimeSubEntry() {Kids = false},
                        new AnimeSubEntry() {Kids = null},
                    }
                };

                mock.Mock<IJikan>().Setup(x => x.GetSeason()).ReturnsAsync(testSeasonData);
                var sut = mock.Create<CurrentSeasonModel>();
                sut.GetCurrentSeasonList(0).Wait();

                var expectedResult = testSeasonData.SeasonEntries.ToList();
                expectedResult.RemoveAll(x => x.Kids == true);

                // Act
                var result = sut.GetFilteredSeasonList();

                // Assert
                Assert.True(result.Count == expectedResult.Count);
                Assert.Equal(expectedResult, result);
            }
        }
    }
}
