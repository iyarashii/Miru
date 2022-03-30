﻿using System;
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
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                mock.Mock<IDirectoryInfo>()
                    .Setup(x => x.Exists)
                    .Returns(cacheFolderPresent);

                var fakeCacheDirectoryInfo = mock.Create<IDirectoryInfo>();

                mock.Mock<IFileSystem>()
                    .Setup(x => x.DirectoryInfo.FromDirectoryName(It.IsAny<string>()))
                    .Returns(fakeCacheDirectoryInfo);

                mock.Mock<IFileSystem>()
                    .Setup(x => x.File.Exists(It.IsAny<string>()))
                    .Returns(true);

                // Act
                var sut = mock.Create<FileSystemService>();

                // Assert
                mock.Mock<IDirectoryInfo>().Verify(x => x.Create(), Times.Exactly(timesCreateIsCalled));
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
    }
}
