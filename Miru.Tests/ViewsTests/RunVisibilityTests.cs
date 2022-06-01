// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using Miru.Views;
using System;
using System.Windows.Documents;
using Xunit;

namespace Miru.Tests.ViewsTests
{
    public class RunVisibilityTests
    {
        [Fact]
        public void GetVisible_DefaultRun_ReturnVisiblePropertyValue()
        {
            Run testData = new Run();

            var result = RunVisibility.GetVisible(testData);
        
            Assert.True(result);
        }

        [Fact]
        public void GetVisible_NullRun_ThrowException()
        {
            Assert.Throws<NullReferenceException>(() => RunVisibility.GetVisible(null));
        }
    }
}
