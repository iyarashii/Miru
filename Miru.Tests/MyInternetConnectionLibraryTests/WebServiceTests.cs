﻿// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using Autofac.Extras.Moq;
using JikanDotNet;
using JikanDotNet.Exceptions;
using Moq;
using MyInternetConnectionLibrary;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Miru.Tests.MyInternetConnectionLibraryTests
{
    public class WebServiceTests
    {
        [Fact]
        public async Task TryToGetAnimeInfo_GivenValidMalId_ReturnsAnime()
        {
            using (var mock = AutoMock.GetLoose())
            {
                Func<IWebClientWrapper> mockWebClientFunc = () => { return Mock.Of<IWebClientWrapper>(); };
                mock.Mock<IWebService>().Setup(x => x.CreateWebClient).Returns(mockWebClientFunc);
                var testData = new Anime();
                mock.Mock<IJikan>().Setup(x => x.GetAnime(It.IsAny<long>())).ReturnsAsync(testData);
                var sut = mock.Create<WebService>();

                var result = await sut.TryToGetAnimeInfo(default, default, mock.Mock<IJikan>().Object);

                Assert.Equal(testData, result);
            }
        }

        [Fact]
        public async Task TryToGetAnimeInfo_GivenJikanRequestException_RetryAndReturnAnime()
        {
            using (var mock = AutoMock.GetLoose())
            {
                Func<IWebClientWrapper> mockWebClientFunc = () => { return Mock.Of<IWebClientWrapper>(); };
                mock.Mock<IWebService>()
                    .Setup(x => x.CreateWebClient)
                    .Returns(mockWebClientFunc);
                var testData = new Anime();
                mock.Mock<IJikan>()
                    .SetupSequence(x => x.GetAnime(It.IsAny<long>()))
                    .ThrowsAsync(new JikanRequestException())
                    .ReturnsAsync(testData);
                var sut = mock.Create<WebService>();

                var result = await sut.TryToGetAnimeInfo(default, default, mock.Mock<IJikan>().Object);

                mock.Mock<IJikan>().Verify(x => x.GetAnime(It.IsAny<long>()), Times.Exactly(2));
                Assert.Equal(testData, result);
            }
        }

        [Fact]
        public async Task TryToGetAnimeInfo_GivenHttpRequestException_ThrowsNoInternetConnectionException()
        {
            using (var mock = AutoMock.GetLoose())
            {
                Func<IWebClientWrapper> mockWebClientFunc = () => { return Mock.Of<IWebClientWrapper>(); };
                mock.Mock<IWebService>()
                    .Setup(x => x.CreateWebClient)
                    .Returns(mockWebClientFunc);
                mock.Mock<IJikan>()
                    .Setup(x => x.GetAnime(It.IsAny<long>()))
                    .ThrowsAsync(new HttpRequestException());
                var sut = mock.Create<WebService>();

                var result = await Assert.ThrowsAsync<NoInternetConnectionException>(
                    () => sut.TryToGetAnimeInfo(default, default, mock.Mock<IJikan>().Object)
                    );
                Assert.Equal("No internet connection", result.Message);
            }
        }
    }
}
