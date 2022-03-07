using Autofac;
using Autofac.Extras.Moq;
using JikanDotNet;
using JikanDotNet.Exceptions;
using MiruLibrary;
using MiruLibrary.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Xunit;

namespace Miru.Tests.ModelsTests
{
    public class MiruAnimeModelTests
    {
        public static IEnumerable<object[]> GetLocalImagePathTestData()
        {
            yield return new object[] { "string without a slash", Path.Combine(Constants.ImageCacheFolderPath, $"string without a slash.jpg") };
        }

        [Theory]
        [InlineData(@"string with a slash - \", @"string with a slash - \")]
        [MemberData(nameof(GetLocalImagePathTestData))]
        public void LocalImagePath_GivenDifferentStrings_SetsExpectedValue(string valueToSet, string expectedResult)
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var sut = mock.Create<MiruAnimeModel>();

                // Act
                sut.LocalImagePath = valueToSet;

                // Assert
                Assert.Equal(expectedResult, sut.LocalImagePath);
            }
        }

        [Fact]
        public void LocalImageSource_GivenIncorrectLocalImagePath_ReturnsNull()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var sut = mock.Create<MiruAnimeModel>();

                // Act
                sut.LocalImagePath = "\\";

                // Assert
                Assert.Null(sut.LocalImageSource);
            }
        }

        [Fact]
        public void LocalImageSource_GivenValidLocalImagePath_ReturnsBitmapImage()
        {
            string testFilePath = Path.Combine(Constants.ImageCacheFolderPath, "test.jpg");
            if (!File.Exists(testFilePath))
            {
                var testImage = new Bitmap(1, 1);
                testImage.Save(testFilePath, ImageFormat.Jpeg);
            }

            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var sut = mock.Create<MiruAnimeModel>();
                var expectedResult = new BitmapImage();
                expectedResult.BeginInit();
                expectedResult.UriSource = new Uri(testFilePath, UriKind.RelativeOrAbsolute);
                expectedResult.CacheOption = BitmapCacheOption.OnLoad;
                expectedResult.EndInit();

                // Act
                sut.LocalImagePath = testFilePath;

                // Assert
                Assert.Equal(expectedResult.UriSource, sut.LocalImageSource.UriSource);
                Assert.Equal(expectedResult.CacheOption, sut.LocalImageSource.CacheOption);
            }
        }

        [Fact]
        public void TotalEpisodes_SetToZero_ReturnsNull()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var sut = mock.Create<MiruAnimeModel>();

                // Act
                sut.TotalEpisodes = 0;

                // Assert
                Assert.Null(sut.TotalEpisodes);
            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(1337)]
        [InlineData(-777)]
        public void TotalEpisodes_SetToNonZeroNumber_ReturnsThatNumber(int? nonZeroNumber)
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var sut = mock.Create<MiruAnimeModel>();

                // Act
                sut.TotalEpisodes = nonZeroNumber;

                // Assert
                Assert.Equal(nonZeroNumber, sut.TotalEpisodes);
            }
        }
    }
}
