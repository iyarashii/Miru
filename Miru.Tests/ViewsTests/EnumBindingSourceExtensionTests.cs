// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using System;
using Xunit;
using Miru.Views;
using MiruLibrary;

namespace Miru.Tests.ViewsTests
{
    public class EnumBindingSourceExtensionTests
    {
        [Fact]
        public void EnumType_SameValue_DoNothing()
        {
            var sut = new EnumBindingSourceExtension();
            var initialValue = sut.EnumType;
            
            sut.EnumType = null;

            Assert.Equal(initialValue, sut.EnumType);
        }

        [Fact]
        public void EnumType_EnumValue_SetValue()
        {
            var sut = new EnumBindingSourceExtension(typeof(MiruAppStatus));

            Assert.Equal(typeof(MiruAppStatus), sut.EnumType);
        }

        [Theory]
        [InlineData(typeof(int?))]
        [InlineData(typeof(float))]
        public void EnumType_NotEnumValue_ThrowArgumentException(Type type)
        {
            Assert.Throws<ArgumentException>(() => new EnumBindingSourceExtension(type));
        }

        [Fact]
        public void EnumType_NullValue_SetNull()
        {
            var sut = new EnumBindingSourceExtension(typeof(MiruAppStatus))
            {
                EnumType = null
            };

            Assert.Null(sut.EnumType);
        }

        [Fact]
        public void ProvideValue_EnumTypeNull_ThrowInvalidOperationException()
        {
            var sut = new EnumBindingSourceExtension();

            Assert.Throws<InvalidOperationException>(() => sut.ProvideValue(null));
        }

        [Fact]
        public void ProvideValue_EnumType_ReturnEnumTypeValues()
        {
            var sut = new EnumBindingSourceExtension(typeof(MiruAppStatus));
            Array expectedResult = new[]
            {
                MiruAppStatus.Busy,
                MiruAppStatus.Idle,
                MiruAppStatus.InternetConnectionProblems
            };

            var result = sut.ProvideValue(default);

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void ProvideValue_NullableEnumType_ReturnActualEnumTypeValues()
        {
            var sut = new EnumBindingSourceExtension(typeof(MiruAppStatus?));
            Array expectedResult = new[]
            {
                MiruAppStatus.Busy,
                MiruAppStatus.Busy,
                MiruAppStatus.Idle,
                MiruAppStatus.InternetConnectionProblems
            };

            var result = sut.ProvideValue(default);

            Assert.Equal(expectedResult, result);
        }
    }
}
