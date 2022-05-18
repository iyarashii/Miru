// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using System.Windows;
using System.Windows.Documents;

namespace Miru.Views
{
    // adds the 'visible' property to the Run visual element
    public static class RunVisibility
    {
        public static readonly DependencyProperty VisibleProperty =
            DependencyProperty.RegisterAttached("Visible", typeof(bool), typeof(RunVisibility),
                new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnVisibilityChanged)));

        public static bool GetVisible(Run d)
        {
            return (bool)d.GetValue(VisibleProperty);
        }

        public static void SetVisible(Run d, bool value)
        {
            d.SetValue(VisibleProperty, value);
        }

        private static void OnVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Run)
            {
                var run = (d as Run);
                if ((bool)e.NewValue)
                {
                    if (run.Tag != null)
                    {
                        run.FontSize = (double)run.Tag;
                    }
                }
                else
                {
                    run.Tag = run.FontSize;
                    run.FontSize = 0.004;
                }
            }
        }
    }
}