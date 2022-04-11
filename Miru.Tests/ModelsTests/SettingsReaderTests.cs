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
                var result = (UserSettings)sut.Load(typeof(UserSettings));

                // Assert
                Assert.Equal(expectedData.AnimeImageSize, result.AnimeImageSize);
                Assert.Equal(expectedData.DisplayedAnimeListType, result.DisplayedAnimeListType);
                Assert.Equal(expectedData.DisplayedAnimeType, result.DisplayedAnimeType);
            }
        }
    }
}
