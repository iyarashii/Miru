using Autofac;
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
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Miru.Tests.ModelsTests
{
    public class CurrentUserAnimeListModelTests
    {
        [Fact]
        public async void GetCurrentUserAnimeList_OnHttpRequestException_ReturnsFalseAndExpectedErrorMessage()
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
    }
}
