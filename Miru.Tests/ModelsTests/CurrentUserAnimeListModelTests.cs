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
    }
}
