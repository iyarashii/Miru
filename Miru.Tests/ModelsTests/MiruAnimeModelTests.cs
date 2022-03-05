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
                var source = new BitmapImage();
                source.BeginInit();
                source.UriSource = new Uri(testFilePath, UriKind.RelativeOrAbsolute);
                source.CacheOption = BitmapCacheOption.OnLoad;
                source.EndInit();

                // Act
                sut.LocalImagePath = testFilePath;

                // Assert
                Assert.Equal(source.UriSource, sut.LocalImageSource.UriSource);
                Assert.Equal(source.CacheOption, sut.LocalImageSource.CacheOption);
            }
        }
    }
}
