// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extras.Moq;
using MiruLibrary;
using MiruLibrary.Models;
using Moq;
using MyInternetConnectionLibrary;
using Xunit;

namespace Miru.Tests.ModelsTests
{
    public class FileSystemServiceTests
    {
        [Theory]
        [InlineData(5)]
        [InlineData(1)]
        [InlineData(12)]
        public void ClearImageCache_CallsFileDeleteForEachFileInMiruCacheFolder(int cacheFileCount)
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var fakeFiles = mock.Create<IList<IFileInfo>>();
                
                for (int i = 0; i < cacheFileCount - 1; i++) // -1 because list starts with 1 element from mock.Create
                {
                    fakeFiles.Add(mock.Create<IFileInfo>());
                }

                mock.Mock<IDirectoryInfo>()
                    .Setup(x => x.EnumerateFiles())
                    .Returns(fakeFiles);
                
                var fakeCacheDirectoryInfo = mock.Create<IDirectoryInfo>();

                mock.Mock<IFileSystem>()
                    .Setup(x => x.DirectoryInfo.FromDirectoryName(It.IsAny<string>()))
                    .Returns(fakeCacheDirectoryInfo);

                mock.Mock<IFileSystem>()
                    .Setup(x => x.File.Exists(It.IsAny<string>()))
                    .Returns(true);

                var sut = mock.Create<FileSystemService>();

                // Act
                sut.ClearImageCache();

                // Assert
                mock.Mock<IFileInfo>().Verify(x => x.Delete(), Times.Exactly(cacheFileCount));
            }
        }

        [Theory]
        [InlineData(false, 1)]
        [InlineData(true, 0)]
        public void FileSystemService_Constructor_WhenNoCacheFolder_CreatesImageCacheFolder(bool cacheFolderPresent, int timesCreateIsCalled)
        {
            using (var autoMock = AutoMock.GetLoose())
            {
                // Arrange
                autoMock.Mock<IDirectoryInfo>()
                    .Setup(x => x.Exists)
                    .Returns(cacheFolderPresent);

                var fakeCacheDirectoryInfo = autoMock.Create<IDirectoryInfo>();

                autoMock.Mock<IFileSystem>()
                    .Setup(x => x.DirectoryInfo.FromDirectoryName(It.IsAny<string>()))
                    .Returns(fakeCacheDirectoryInfo);

                autoMock.Mock<IFileSystem>()
                    .Setup(x => x.File.Exists(It.IsAny<string>()))
                    .Returns(true);

                Func<IWebClientWrapper> mockWebClientFunc = () => { return Mock.Of<IWebClientWrapper>(); };
                autoMock.Mock<IWebService>().Setup(x => x.CreateWebClient).Returns(mockWebClientFunc);

                // Act
                var sut = new FileSystemService(autoMock.Create<IFileSystem>(), autoMock.Create<IWebService>());

                // Assert
                autoMock.Mock<IDirectoryInfo>().Verify(x => x.Create(), Times.Exactly(timesCreateIsCalled));
            }
        }

        [Theory]
        [InlineData(1, true)]
        [InlineData(0, false)]
        public void UpdateSenpaiData_GivenSenpaiDataPresent_DeleteCurrentData(int timesSenpaiDataDeleteCalled, bool senpaiDataPresent)
        {
            using (var autoMock = AutoMock.GetLoose())
            {
                // Arrange
                autoMock.Mock<IDirectoryInfo>()
                    .Setup(x => x.Exists)
                    .Returns(true);

                var fakeCacheDirectoryInfo = autoMock.Create<IDirectoryInfo>();

                autoMock.Mock<IFileSystem>()
                    .Setup(x => x.DirectoryInfo.FromDirectoryName(It.IsAny<string>()))
                    .Returns(fakeCacheDirectoryInfo);

                autoMock.Mock<IFileSystem>()
                    .SetupSequence(x => x.File.Exists(It.IsAny<string>()))
                    .Returns(true)
                    .Returns(senpaiDataPresent)
                    .Returns(true);

                var sut = autoMock.Create<FileSystemService>();

                // Act
                sut.UpdateSenpaiData();

                // Assert
                autoMock.Mock<IFileSystem>().Verify(x => x.File.Delete(Constants.SenpaiFilePath), Times.Exactly(timesSenpaiDataDeleteCalled));
            }
        }

        [Fact]
        public void GetSenpaiData_GivenSenpaiDataPresent_DoNotGetNewSenpaiData()
        {
            using (var autoMock = AutoMock.GetLoose())
            {
                // Arrange
                autoMock.Mock<IDirectoryInfo>()
                    .Setup(x => x.Exists)
                    .Returns(true);

                var fakeCacheDirectoryInfo = autoMock.Create<IDirectoryInfo>();

                autoMock.Mock<IFileSystem>()
                    .Setup(x => x.DirectoryInfo.FromDirectoryName(It.IsAny<string>()))
                    .Returns(fakeCacheDirectoryInfo);

                autoMock.Mock<IFileSystem>()
                    .Setup(x => x.File.Exists(It.IsAny<string>()))
                    .Returns(true);

                var sut = autoMock.Create<FileSystemService>();

                // Act
                sut.GetSenpaiData();

                // Assert
                autoMock.Mock<IFileSystem>().Verify(x => x.File.CreateText(Constants.SenpaiFilePath), Times.Never);
            }
        }

        [Fact]
        public void GetSenpaiData_GivenNoSenpaiData_GetNewSenpaiData()
        {
            using (var autoMock = AutoMock.GetLoose())
            {
                // Arrange
                autoMock.Mock<IDirectoryInfo>()
                    .Setup(x => x.Exists)
                    .Returns(true);

                var fakeCacheDirectoryInfo = autoMock.Create<IDirectoryInfo>();

                autoMock.Mock<IFileSystem>()
                    .Setup(x => x.DirectoryInfo.FromDirectoryName(It.IsAny<string>()))
                    .Returns(fakeCacheDirectoryInfo);

                autoMock.Mock<IFileSystem>()
                    .Setup(x => x.File.CreateText(It.IsAny<string>()))
                    .Returns(new StreamWriter(new MemoryStream()));

                autoMock.Mock<IFileSystem>()
                    .SetupSequence(x => x.File.Exists(It.IsAny<string>()))
                    .Returns(true)
                    .Returns(false);

                autoMock.Mock<IWebService>().Setup(x => x.Client).Returns(new System.Net.Http.HttpClient());

                var sut = autoMock.Create<FileSystemService>();

                // Act
                sut.GetSenpaiData();

                // Assert
                autoMock.Mock<IFileSystem>().Verify(x => x.File.CreateText(Constants.SenpaiFilePath), Times.Once);
            }
        }

        [Theory]
        [InlineData(1, false)]
        [InlineData(0, true)]
        public void DownloadFile_DependingIfFileIsPresent_Download(int downloadFileTimesCalled, bool filePresent)
        {
            using (var autoMock = AutoMock.GetLoose())
            {
                // Arrange
                autoMock.Mock<IDirectoryInfo>()
                    .Setup(x => x.Exists)
                    .Returns(true);

                var fakeCacheDirectoryInfo = autoMock.Create<IDirectoryInfo>();

                autoMock.Mock<IFileSystem>()
                    .Setup(x => x.DirectoryInfo.FromDirectoryName(It.IsAny<string>()))
                    .Returns(fakeCacheDirectoryInfo);

                autoMock.Mock<IFileSystem>()
                    .SetupSequence(x => x.File.Exists(It.IsAny<string>()))
                    .Returns(true)
                    .Returns(filePresent);

                var sut = autoMock.Create<FileSystemService>();

                autoMock.Mock<IWebClientWrapper>();
                var fakeWebClient = autoMock.Create<IWebClientWrapper>();

                // Act
                sut.DownloadFile(fakeWebClient, It.IsAny<string>(), It.IsAny<string>());

                // Assert
                autoMock.Mock<IWebClientWrapper>()
                    .Verify(x => x.DownloadFile(It.IsAny<string>(), It.IsAny<string>()), 
                    Times.Exactly(downloadFileTimesCalled));
            }
        }
    }
}
