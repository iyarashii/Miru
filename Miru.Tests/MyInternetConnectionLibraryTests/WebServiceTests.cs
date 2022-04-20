using Autofac.Extras.Moq;
using JikanDotNet;
using Moq;
using MyInternetConnectionLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Miru.Tests.MyInternetConnectionLibraryTests
{
    public class WebServiceTests
    {
        [Fact]
        public async void TryToGetAnimeInfo_GivenValidMalId_ReturnsAnime()
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
    }
}
