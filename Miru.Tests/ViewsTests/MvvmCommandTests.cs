using Miru.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Miru.Tests.ViewsTests
{
    public class MvvmCommandTests
    {
        [Fact]
        public void Constructor_GivenNullExecute_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>("execute", () => new MvvmCommand(null));
        }

        [Fact]
        public void Constructor_GivenNullCanExecute_SetCanExecuteTrue()
        {
            var sut = new MvvmCommand(x => { }, null);
            
            Assert.True(sut._canExecute(null));
        }
    }
}
