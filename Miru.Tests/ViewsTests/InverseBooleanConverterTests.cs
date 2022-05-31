// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using Miru.Views;
using System;
using Xunit;

namespace Miru.Tests.ViewsTests
{
    public class InverseBooleanConverterTests
    {
        [Fact]
        public void Convert_TargetTypeNotBool_ThrowInvalidOperationException()
        {
            var sut = new InverseBooleanConverter();

            Assert.Throws<InvalidOperationException>(() => sut.Convert(default, typeof(int), default, default));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Convert_TargetTypeBool_ReturnInverseBool(bool testData)
        {
            var sut = new InverseBooleanConverter();

            var result = sut.Convert(testData, typeof(bool), default, default);

            Assert.Equal(!testData, result);
        }

        [Fact]
        public void ConvertBack_ThrowsNotSupportedException()
        {
            var sut = new InverseBooleanConverter();

            Assert.Throws<NotSupportedException>(() => sut.ConvertBack(default, default, default, default));
        }
    }
}
