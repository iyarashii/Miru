// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using MiruLibrary;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Miru.Views
{
    [ValueConversion(typeof(bool), typeof(Color))]
    public class DroppedToColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            switch (values[1])
            {
                case AnimeListType.Season:
                    if ((bool)values[0])
                        return Brushes.Red.Color;
                    if ((bool)values[2])
                        return Brushes.Green.Color;
                    return DependencyProperty.UnsetValue;
                default:
                    return DependencyProperty.UnsetValue;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
