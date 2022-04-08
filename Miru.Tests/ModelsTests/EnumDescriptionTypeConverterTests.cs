using Autofac.Extras.Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using MiruLibrary;
using System.Globalization;

namespace Miru.Tests.ModelsTests
{
    public class EnumDescriptionTypeConverterTests
    {
        [Fact]
        public void ConvertTo_GivenNotStringDestinationType_ReturnsBaseCall()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var testData = "Busy";
                var baseEnumConverter = new EnumConverter(typeof(AnimeType));
                var sut = new EnumDescriptionTypeConverter(typeof(AnimeType));

                // Act & Assert
                Assert.Throws<NotSupportedException>(() => baseEnumConverter.ConvertTo(default, default, testData, typeof(int)));
                Assert.Throws<NotSupportedException>(() => sut.ConvertTo(default, default, testData, typeof(int)));
            }
        }

        [Fact]
        public void ConvertTo_GivenNullValue_ReturnsEmptyString()
        {
            // Arrange
            var sut = new EnumDescriptionTypeConverter(typeof(AnimeType));

            // Act
            var result = sut.ConvertTo(default, default, null, typeof(string));

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void ConvertTo_GivenNullFieldInfo_ReturnsEmptyString()
        {
            // Arrange
            var sut = new EnumDescriptionTypeConverter(typeof(AnimeType));
            int testValue = 0;

            // Act
            var result = sut.ConvertTo(default, default, testValue, typeof(string));

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Theory]
        [InlineData(AnimeType.Both, "TV & ONA", typeof(AnimeType))]
        [InlineData(AnimeType.TV, "TV", typeof(AnimeType))]
        [InlineData(AnimeType.ONA, "ONA", typeof(AnimeType))]
        [InlineData(AnimeListType.AiringAndWatching, "Airing & Watching", typeof(AnimeListType))]
        [InlineData(AnimeListType.Watching, "Watching", typeof(AnimeListType))]
        [InlineData(AnimeListType.Season, "Current Season", typeof(AnimeListType))]
        [InlineData(AnimeListType.Senpai, "Senpai - Current Season", typeof(AnimeListType))]
        public void ConvertTo_GivenDescriptionAttribute_ReturnsDescription(
            object testValue, string expectedDescription, Type enumType)
        {
            // Arrange
            var sut = new EnumDescriptionTypeConverter(enumType);

            // Act
            var result = sut.ConvertTo(default, default, testValue, typeof(string));

            // Assert
            Assert.Equal(expectedDescription, result);
        }

        [Theory]
        [InlineData(MiruAppStatus.Busy, nameof(MiruAppStatus.Busy), typeof(MiruAppStatus))]
        [InlineData(MiruAppStatus.Idle, nameof(MiruAppStatus.Idle), typeof(MiruAppStatus))]
        [InlineData(MiruAppStatus.InternetConnectionProblems, nameof(MiruAppStatus.InternetConnectionProblems), typeof(MiruAppStatus))]
        public void ConvertTo_GivenNoDescriptionAttribute_ReturnsEnumName(
            object testValue, string expectedDescription, Type enumType)
        {
            // Arrange
            var sut = new EnumDescriptionTypeConverter(enumType);

            // Act
            var result = sut.ConvertTo(default, default, testValue, typeof(string));

            // Assert
            Assert.Equal(expectedDescription, result);
        }
        enum TestEnum
        {
            [Description("")]
            TestEmptyDescription,

            [Description(null)]
            NullDescription
        }

        [Theory]
        [InlineData(TestEnum.TestEmptyDescription, nameof(TestEnum.TestEmptyDescription), typeof(TestEnum))]
        [InlineData(TestEnum.NullDescription, nameof(TestEnum.NullDescription), typeof(TestEnum))]
        public void ConvertTo_GivenEmptyDescriptionAttribute_ReturnsEnumName(
            object testValue, string expectedDescription, Type enumType)
        {
            // Arrange
            var sut = new EnumDescriptionTypeConverter(enumType);

            // Act
            var result = sut.ConvertTo(default, default, testValue, typeof(string));

            // Assert
            Assert.Equal(expectedDescription, result);
        }
    }
}
