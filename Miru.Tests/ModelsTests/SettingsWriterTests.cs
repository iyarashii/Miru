// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using Autofac;
using Autofac.Extras.Moq;
using MiruLibrary;
using MiruLibrary.Models;
using MiruLibrary.Settings;
using Moq;
using System.IO;
using Xunit;

namespace Miru.Tests.ModelsTests
{
    public class SettingsWriterTests
    {
        [Fact]
        public void Write_GivenUserSettingsData_WritesExpectedJsonString()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var testData = new UserSettings();
                string expectedJsonString = File.ReadAllText("config.json");

                mock.Mock<IFileSystemService>()
                    .Setup(x => x.FileSystem.File.WriteAllText(Constants.SettingsPath, It.IsAny<string>()));
                var sut = mock.Create<SettingsWriter>(
                    new NamedParameter("fileSystemService", mock.Create<IFileSystemService>()),
                    new NamedParameter("configurationFilePath", Constants.SettingsPath));

                // Act
                sut.Write(testData);

                // Assert
                mock.Mock<IFileSystemService>().Verify(x => x.FileSystem.File.WriteAllText(Constants.SettingsPath, expectedJsonString), Times.Once);
            }
        }
    }
}
