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
        public void ConvertTo_GivenDestinationTypeNotString_ReturnsBaseCall()
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
    }
}
