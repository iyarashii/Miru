// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using Miru.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Miru.Tests.ViewsTests
{
    public class PercentageNumberBoxNumberFormatterTests
    {
        [Theory]
        [InlineData(0.1, "10%")]
        [InlineData(0.66, "66%")]
        [InlineData(0.97, "97%")]
        [InlineData(21.37, "2137%")]
        [InlineData(.39, "39%")]
        [InlineData(0, "0%")]
        [InlineData(0.01, "1%")]
        public void FormatDouble_ValidDoubleValue_ReturnsValueAsPercentage(double testValue, string expectedResult)
        {
            var sut = new PercentageNumberBoxNumberFormatter();

            var result = sut.FormatDouble(testValue);

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void ParseDouble_NotParsableText_ReturnNull()
        {
            var sut = new PercentageNumberBoxNumberFormatter();

            var result = sut.ParseDouble("🧅");

            Assert.Null(result);
        }

        [Theory]
        [InlineData("10", 0.1)]
        [InlineData("66", 0.66)]
        [InlineData("97", 0.97)]
        [InlineData("2137", 21.37)]
        [InlineData("39", .39)]
        [InlineData("0", 0)]
        [InlineData("1", 0.01)]
        public void ParseDouble_ParsableText_ReturnDoubleValue(string testValue, double expectedResult)
        {
            var sut = new PercentageNumberBoxNumberFormatter();

            var result = sut.ParseDouble(testValue);

            Assert.Equal(expectedResult, result);
        }
    }
}
