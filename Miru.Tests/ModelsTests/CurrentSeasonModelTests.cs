﻿using Autofac;
using Autofac.Extras.Moq;
using JikanDotNet;
using JikanDotNet.Exceptions;
using MiruLibrary.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        [Fact]
        public async void GetCurrentSeasonList_OnJikanRequestException_TriplesExecutionDelay()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                mock.Mock<IJikan>().SetupSequence(x => x.GetSeason())
                    .ThrowsAsync(new JikanRequestException())
                    .ReturnsAsync(new Season())
                    .ReturnsAsync(new Season());

                var sut = mock.Create<CurrentSeasonModel>();
                int testDelayInMs = 50;
                var timerForTripleDelay = new Stopwatch();
                var timerForSingleDelay = new Stopwatch();

                // Act
                timerForTripleDelay.Start();
                await sut.GetCurrentSeasonList(testDelayInMs);
                timerForTripleDelay.Stop();

                timerForSingleDelay.Start();
                await sut.GetCurrentSeasonList(testDelayInMs);
                timerForSingleDelay.Stop();

                // Assert
                Assert.True(timerForTripleDelay.ElapsedMilliseconds > timerForSingleDelay.ElapsedMilliseconds * 3);
            }
        }

        [Fact]
        public void GetFilteredSeasonList_ReturnsSeasonListWithoutAnimeForKids()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var testSeasonData = new Season
                {
                    SeasonEntries = new List<AnimeSubEntry>()
                    {
                        new AnimeSubEntry() {Kids = true},
                        new AnimeSubEntry() {Kids = false},
                        new AnimeSubEntry() {Kids = null},
                        new AnimeSubEntry() {Kids = true},
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
                Assert.True(result.Count == 4);
                Assert.Equal(expectedResult, result);
            }
        }
    }
}
