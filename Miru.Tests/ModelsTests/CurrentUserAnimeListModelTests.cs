// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using Autofac.Extras.Moq;
using JikanDotNet;
using JikanDotNet.Exceptions;
using MiruLibrary.Models;
using Moq;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Miru.Tests.ModelsTests
{
    public class CurrentUserAnimeListModelTests
    {
        [Fact]
        public async Task GetCurrentUserAnimeList_OnHttpRequestException_ReturnsFalseAndExpectedErrorMessage()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                mock.Mock<IJikan>()
                    .Setup(x => x.GetUserAnimeList(It.IsAny<string>(), UserAnimeListExtension.Watching))
                    .ThrowsAsync(new HttpRequestException());
                var expectedErrorMessage = "Problems with internet connection!";
                var sut = mock.Create<CurrentUserAnimeListModel>();

                // Act
                var (result, errorMessage) = await sut.GetCurrentUserAnimeList(It.IsAny<string>());

                // Assert
                Assert.False(result);
                Assert.Equal(expectedErrorMessage, errorMessage);
            }
        }

        [Fact]
        public async Task GetCurrentUserAnimeList_OnJikanRequestException_ReturnsFalseAndExpectedErrorMessage()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                mock.Mock<IJikan>()
                    .Setup(x => x.GetUserAnimeList(It.IsAny<string>(), UserAnimeListExtension.Watching))
                    .ThrowsAsync(new JikanRequestException());
                var expectedErrorMessage = $"Could not find the user \"{ It.IsAny<string>() }\". Please make sure you typed in the name correctly.";
                var sut = mock.Create<CurrentUserAnimeListModel>();

                // Act
                var (result, errorMessage) = await sut.GetCurrentUserAnimeList(It.IsAny<string>());

                // Assert
                Assert.False(result);
                Assert.Equal(expectedErrorMessage, errorMessage);
            }
        }

        [Fact]
        public async Task GetCurrentUserAnimeList_OnNoExceptions_ReturnsTrueAndEmptyErrorMessage()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                mock.Mock<IJikan>()
                    .Setup(x => x.GetUserAnimeList(It.IsAny<string>(), UserAnimeListExtension.Watching))
                    .ReturnsAsync(new UserAnimeList());
                var expectedErrorMessage = string.Empty;
                var sut = mock.Create<CurrentUserAnimeListModel>();

                // Act
                var (result, errorMessage) = await sut.GetCurrentUserAnimeList(It.IsAny<string>());

                // Assert
                Assert.True(result);
                Assert.Equal(expectedErrorMessage, errorMessage);
            }
        }

        [Theory]
        [InlineData(50)]
        [InlineData(299)]
        [InlineData(0)]
        [InlineData(1)]
        public async Task GetCurrentUserDroppedAnimeList_LessThan300DroppedAnimes_ReturnTrueAndCorrectCount(int expectedDroppedAnimeCount)
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var testData = new UserAnimeList
                {
                    Anime = new AnimeListEntry[expectedDroppedAnimeCount]
                };
                mock.Mock<IJikan>()
                    .Setup(x => x.GetUserAnimeList(It.IsAny<string>(), UserAnimeListExtension.Dropped, It.IsAny<int>()))
                    .ReturnsAsync(testData);
                var sut = mock.Create<CurrentUserAnimeListModel>();

                // Act
                var (result, errorMessage) = await sut.GetCurrentUserDroppedAnimeList(It.IsAny<string>());

                // Assert
                Assert.True(result);
                Assert.Equal(string.Empty, errorMessage);
                Assert.Equal(expectedDroppedAnimeCount, sut.UserDroppedAnimeListData.Anime.Count);
            }
        }

        [Fact]
        public async Task GetCurrentUserDroppedAnimeList_Exception_ReturnFalseAndErrorMessage()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                mock.Mock<IJikan>()
                    .Setup(x => x.GetUserAnimeList(It.IsAny<string>(), UserAnimeListExtension.Dropped, It.IsAny<int>()))
                    .ThrowsAsync(new Exception());
                var sut = mock.Create<CurrentUserAnimeListModel>();

                // Act
                var (result, errorMessage) = await sut.GetCurrentUserDroppedAnimeList(It.IsAny<string>());

                // Assert
                Assert.False(result);
                Assert.Equal("Problem with getting user's dropped anime list!", errorMessage);
            }
        }

        [Theory]
        [InlineData(300)]
        [InlineData(600)]
        [InlineData(2137)]
        [InlineData(3939)]
        public async Task GetCurrentUserDroppedAnimeList_300OrMoreDroppedAnimes_ReturnTrueAndCorrectCount(int expectedDroppedAnimeCount)
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                int pages = 0, restOfEntries = 0;
                if (expectedDroppedAnimeCount % 300 == 0)
                    pages = expectedDroppedAnimeCount / 300;
                else if (expectedDroppedAnimeCount > 300)
                {
                    pages = (expectedDroppedAnimeCount - expectedDroppedAnimeCount % 300) / 300;
                    restOfEntries = expectedDroppedAnimeCount % 300;
                }
                else
                    restOfEntries = expectedDroppedAnimeCount % 300;
                mock.Mock<IJikan>()
                    .Setup(x => x.GetUserAnimeList(It.IsAny<string>(), UserAnimeListExtension.Dropped, It.IsAny<int>()))
                    .ReturnsAsync(() =>
                    {
                        if(pages > 0)
                        {
                            pages--;
                            return new UserAnimeList
                            {
                                Anime = new AnimeListEntry[300]
                            };
                        }
                        return new UserAnimeList
                        {
                            Anime = new AnimeListEntry[restOfEntries]
                        };
                    });
                var sut = mock.Create<CurrentUserAnimeListModel>();

                // Act
                var (result, errorMessage) = await sut.GetCurrentUserDroppedAnimeList(It.IsAny<string>());

                // Assert
                Assert.True(result);
                Assert.Equal(string.Empty, errorMessage);
                Assert.Equal(expectedDroppedAnimeCount, sut.UserDroppedAnimeListData.Anime.Count);
            }
        }
    }
}
