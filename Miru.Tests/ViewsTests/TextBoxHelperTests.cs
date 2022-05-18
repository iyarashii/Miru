// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using Miru.Views;
using System.Windows;
using System.Windows.Input;
using Xunit;

namespace Miru.Tests.ViewsTests
{
    public class TextBoxHelperTests
    {
        [Fact]
        public void GetFocusGesture_GivenDependencyObject_ReturnsKeyGesture()
        {
            var testData = new DependencyObject();

            var result = TextBoxHelper.GetFocusGesture(testData);

            Assert.Equal(Key.F, result.Key);
            Assert.Equal(string.Empty, result.DisplayString);
            Assert.Equal(ModifierKeys.Control, result.Modifiers);
        }

        [Fact]
        public void SetFocusGesture_GivenKeyGesture_SetsFocusGesturePropertyToKeyGesture()
        {
            var testData = new KeyGesture(Key.D, ModifierKeys.Control);
            var dependencyObject = new DependencyObject();

            TextBoxHelper.SetFocusGesture(dependencyObject, testData);

            Assert.True(dependencyObject.GetValue(TextBoxHelper.FocusGestureProperty) == testData);
        }

        [StaFact]
        public void FocusCommand_GivenMvvmCommand_FocusTag()
        {
            var testData = new MvvmCommand(x => { })
            {
                Tag = new UIElement()
            };
            (testData.Tag as UIElement).Focusable = true;
            (testData.Tag as UIElement).IsEnabled = true;

            TextBoxHelper.FocusCommand(testData);

            Assert.True((testData.Tag as UIElement).IsFocused);
        }
    }
}
