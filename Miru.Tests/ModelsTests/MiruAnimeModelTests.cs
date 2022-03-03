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
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
    }
}
