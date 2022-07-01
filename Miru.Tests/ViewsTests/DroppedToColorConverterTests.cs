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
    public class DroppedToColorConverterTests
    {
        [Fact]
        public void ConvertBack_ThrowsNotSupportedException()
        {
            var sut = new DroppedToColorConverter();

            Assert.Throws<NotSupportedException>(() => sut.ConvertBack(default, default, default, default));
        }
    }
}
