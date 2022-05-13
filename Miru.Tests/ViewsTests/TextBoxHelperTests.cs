using Miru.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
