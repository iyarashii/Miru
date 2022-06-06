// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using Miru.Views;
using System;
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

        [Fact]
        public void Execute_NullParam_ParamIsMvvmCommand()
        {
            object testResult = null;
            var sut = new MvvmCommand(x => { testResult = x; });

            sut.Execute(null);

            Assert.True(testResult is MvvmCommand);
        }

        [Fact]
        public void Execute_NotNullParam_ParamTypeMatchesGivenParam()
        {
            object testResult = null;
            var sut = new MvvmCommand(x => { testResult = x; });

            sut.Execute(true);

            Assert.True(testResult is bool);
        }

        [Fact]
        public void CanExecute_canExecuteNull_ReturnTrue()
        {
            var sut = new MvvmCommand(x => { });

            var result = sut.CanExecute(null);

            Assert.True(result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CanExecute_canExecuteNotNull_ReturnFuncResult(bool funcResult)
        {
            var sut = new MvvmCommand(x => { }, x => { return funcResult; });

            var result = sut.CanExecute(null);

            Assert.Equal(sut._canExecute(null), result);
        }
    }
}
