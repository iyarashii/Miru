// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using Miru.Views;
using System.Windows;
using System.Windows.Controls;
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

        [StaFact]
        public void SetFocusGesture_ObjIsNotUiElement_DoNotAddWindowInputBinding()
        {
            var testData = new KeyGesture(Key.D, ModifierKeys.Control);
            var dependencyObject = new DependencyObject();
            var testWindow = new Window
            {
                Content = dependencyObject
            };

            TextBoxHelper.SetFocusGesture(dependencyObject, testData);

            Assert.Equal(testData, dependencyObject.GetValue(TextBoxHelper.FocusGestureProperty));
            Assert.Empty(testWindow.InputBindings);
        }

        [StaFact]
        public void SetFocusGesture_ObjIsUiElement_AddWindowInputBinding()
        {
            var testData = new KeyGesture(Key.D, ModifierKeys.Control);
            var testObj = new FrameworkElement();
            var testWindow = new Window
            {
                Content = testObj
            };
            var inputBindingsBeforeSetFocus = testWindow.InputBindings.Count;

            TextBoxHelper.SetFocusGesture(testObj, testData);

            Assert.Equal(testData, testObj.GetValue(TextBoxHelper.FocusGestureProperty));
            Assert.True(testWindow.InputBindings.Count == ++inputBindingsBeforeSetFocus);
        }

        [StaFact]
        public void FocusCommand_GivenMvvmCommand_FocusTagAndSelectAllText()
        {
            var testText = "3939";
            var testData = new MvvmCommand(x => { })
            {
                Tag = new TextBox() { Text = testText}
            };
            (testData.Tag as TextBox).Focusable = true;
            (testData.Tag as TextBox).IsEnabled = true;

            TextBoxHelper.FocusCommand(testData);

            Assert.True((testData.Tag as TextBox).IsFocused);
            Assert.Equal(testText, (testData.Tag as TextBox).SelectedText);
        }

        [StaFact]
        public void FocusCommand_ParamNotMvvmCommand_DoNotFocusTag()
        {
            var testData = new FrameworkElement()
            {
                Tag = new UIElement()
            };
            (testData.Tag as UIElement).Focusable = true;
            (testData.Tag as UIElement).IsEnabled = true;

            TextBoxHelper.FocusCommand(testData);

            Assert.False((testData.Tag as UIElement).IsFocused);
        }
    }
}
