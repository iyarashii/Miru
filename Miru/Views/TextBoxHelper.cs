﻿// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Miru.Views
{
    public class TextBoxHelper : DependencyObject
    {
        public static KeyGesture GetFocusGesture(DependencyObject obj)
        {
            return (KeyGesture)obj.GetValue(FocusGestureProperty);
        }

        public static void SetFocusGesture(DependencyObject obj, KeyGesture value)
        {
            obj.SetValue(FocusGestureProperty, value);
        }

        [TypeConverter(typeof(KeyGestureConverter))]
        public static readonly DependencyProperty FocusGestureProperty =
            DependencyProperty.RegisterAttached("FocusGesture", typeof(KeyGesture), typeof(TextBoxHelper), 
            new PropertyMetadata(
                new KeyGesture(Key.F, ModifierKeys.Control), 
                new PropertyChangedCallback((s, e) =>
                {
                    if (s is UIElement targetElement)
                    {
                        MvvmCommand command = new MvvmCommand(parameter => FocusCommand(parameter))
                        {
                            Tag = targetElement,
                        };
                        InputGesture inputg = (KeyGesture)e.NewValue;
                        Window.GetWindow(targetElement).InputBindings.Add(new InputBinding(command, inputg));
                    }
                })));

        public static void FocusCommand(object parameter)
        {
            if (parameter is MvvmCommand targetCommand && targetCommand.Tag is UIElement targetElement)
            {
                targetElement.Focus();
                (targetElement as TextBox).SelectAll();
            }
        }
    }
}
