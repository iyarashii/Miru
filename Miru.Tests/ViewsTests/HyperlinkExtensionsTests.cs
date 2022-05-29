// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using Miru.Views;
using System;
using System.Windows;
using System.Windows.Documents;
using Xunit;

namespace Miru.Tests.ViewsTests
{
    public class HyperlinkExtensionsTests
    {
        [Fact]
        public void GetIsExternal_NotNullObj_ReturnIsExternalPropertyValue()
        {
            var testData = new DependencyObject();

            bool result = HyperlinkExtensions.GetIsExternal(testData);

            Assert.False(result);
        }

        [Fact]
        public void GetIsExternal_NullObj_ThrowNullReferenceException()
        {
            Assert.Throws<NullReferenceException> (() => HyperlinkExtensions.GetIsExternal(null));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void SetIsExternal_BoolValue_SetIsExternalProperty(bool testData)
        {
            var testDependencyObject = new Hyperlink();

            HyperlinkExtensions.SetIsExternal(testDependencyObject, testData);
            
            Assert.Equal(testData, HyperlinkExtensions.GetIsExternal(testDependencyObject));
        }

        [Fact]
        public void SetIsExternal_NullObj_ThrowNullReferenceException()
        {
            Assert.Throws<NullReferenceException>(() => HyperlinkExtensions.SetIsExternal(null, default));
        }
    }
}
