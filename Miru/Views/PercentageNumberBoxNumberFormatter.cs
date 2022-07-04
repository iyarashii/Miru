// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using ModernWpf.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miru.Views
{
    public class PercentageNumberBoxNumberFormatter : INumberBoxNumberFormatter
    {
        public string FormatDouble(double value)
        {
            return $"{value*100}%";
        }

        public double? ParseDouble(string text)
        {
            if (double.TryParse(text, out double result))
            {
                return result/100;
            }
            return null;
        }
    }
}
