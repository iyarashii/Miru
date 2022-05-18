// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using Xunit;
using MiruLibrary;
using MiruLibrary.Settings;
using MiruLibrary.Models;
using Autofac;

namespace Miru.Tests.ModelsTests
{
    public class SettingsReaderTests
    {
        [Fact]
        public void Load_GivenNoConfigFilePresent_ReturnsDefaultUserSettings()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                mock.Mock<IFileSystemService>()
                    .Setup(x => x.FileSystem.File.Exists(Constants.SettingsPath))
                    .Returns(false);
                var sut = mock.Create<SettingsReader>(
                    new NamedParameter("fileSystemService", mock.Create<IFileSystemService>()), 
                    new NamedParameter("configurationFilePath", Constants.SettingsPath), 
                    new NamedParameter("sectionNameSuffix", "Settings"));

                var expectedData = new UserSettings();

                // Act
                var result = sut.Load<UserSettings>();

                // Assert
                Assert.Equal(expectedData.AnimeImageSize, result.AnimeImageSize);
                Assert.Equal(expectedData.DisplayedAnimeListType, result.DisplayedAnimeListType);
                Assert.Equal(expectedData.DisplayedAnimeType, result.DisplayedAnimeType);
            }
        }

        [Theory]
        [InlineData(21.37, AnimeListType.Senpai, AnimeType.Both)]
        [InlineData(13.37, AnimeListType.Watching, AnimeType.TV)]
        [InlineData(420.69, AnimeListType.Season, AnimeType.ONA)]
        public void Load_GivenConfigFilePresent_ReturnsUserSettings(double imageSize, AnimeListType listType, AnimeType animeType)
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                string testJson = $@"
                                {{
                                  ""animeImageSize"": {imageSize},
                                  ""displayedAnimeListType"": {(int)listType},
                                  ""displayedAnimeType"": {(int)animeType}
                                }}";

                mock.Mock<IFileSystemService>()
                    .Setup(x => x.FileSystem.File.Exists(Constants.SettingsPath))
                    .Returns(true);
                mock.Mock<IFileSystemService>()
                    .Setup(x => x.FileSystem.File.ReadAllText(Constants.SettingsPath))
                    .Returns(testJson);
                var sut = mock.Create<SettingsReader>(
                    new NamedParameter("fileSystemService", mock.Create<IFileSystemService>()),
                    new NamedParameter("configurationFilePath", Constants.SettingsPath),
                    new NamedParameter("sectionNameSuffix", "Settings"));

                var expectedData = new UserSettings() 
                { 
                    AnimeImageSize = imageSize, 
                    DisplayedAnimeListType = listType, 
                    DisplayedAnimeType = animeType
                };

                // Act
                var result = sut.Load<UserSettings>();

                // Assert
                Assert.Equal(expectedData.AnimeImageSize, result.AnimeImageSize);
                Assert.Equal(expectedData.DisplayedAnimeListType, result.DisplayedAnimeListType);
                Assert.Equal(expectedData.DisplayedAnimeType, result.DisplayedAnimeType);
            }
        }
    }
}
