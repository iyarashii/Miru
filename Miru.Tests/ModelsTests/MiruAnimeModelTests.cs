// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using Autofac.Extras.Moq;
using MiruLibrary;
using MiruLibrary.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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

        [Fact]
        public void FormatThemesOutput_GivenIncorrectJsonString_ReturnsJsonString()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var sut = mock.Create<MiruAnimeModel>();

                // Act
                var result = sut.FormatThemesOutput(null, "test");

                // Assert
                Assert.Equal("test", result);
            }
        }

        [Fact]
        public void FormatThemesOutput_GivenCorrectJsonString_ReturnsListOfThemes()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var sut = mock.Create<MiruAnimeModel>();
                var themesList = new List<string> { "theme1", "theme2", "theme3" };
                var json = JsonConvert.SerializeObject(themesList);

                // Act
                var result = sut.FormatThemesOutput("OP", json);

                // Assert
                Assert.Equal("OP\ntheme1\ntheme2\ntheme3\n", result);
            }
        }

        [Fact]
        public void OpeningThemes_GetValue_ReturnsFormattedSongs()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var sut = mock.Create<MiruAnimeModel>();
                var themesList = new List<string> { "song1", "song2", "song3" };
                sut.OpeningThemes = JsonConvert.SerializeObject(themesList);

                // Act
                var result = sut.OpeningThemes;

                // Assert
                Assert.Equal("OP\nsong1\nsong2\nsong3\n", result);
            }
        }
    }
}
