// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using Miru.Views;
using MiruLibrary;
using System;
using System.Windows;
using System.Windows.Media;
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

        [Theory]
        [InlineData(AnimeListType.Senpai)]
        [InlineData(AnimeListType.Season)]
        public void Convert_DroppedTrueAndSeasonListType_ReturnRedColor(AnimeListType animeListType)
        {
            var testValues = new object[] { true, animeListType};
            var sut = new DroppedToColorConverter();

            var result = sut.Convert(testValues, typeof(Color), default, default);

            Assert.Equal(Brushes.Red.Color, result);
        }

        [Theory]
        [InlineData(AnimeListType.Senpai)]
        [InlineData(AnimeListType.Season)]
        public void Convert_WatchingTrueAndSeasonListType_ReturnGreenColor(AnimeListType animeListType)
        {
            var testValues = new object[] { false, animeListType, true };
            var sut = new DroppedToColorConverter();

            var result = sut.Convert(testValues, typeof(Color), default, default);

            Assert.Equal(Brushes.Green.Color, result);
        }

        [Theory]
        [InlineData(AnimeListType.Senpai)]
        [InlineData(AnimeListType.Season)]
        [InlineData(AnimeListType.AiringAndWatching)]
        [InlineData(AnimeListType.Watching)]
        public void Convert_NotSeasonListTypeOrBothDroppedAndWatchingFalse_ReturnUnsetValue(AnimeListType animeListType)
        {
            var testValues = new object[] { false, animeListType, false };
            var sut = new DroppedToColorConverter();

            var result = sut.Convert(testValues, typeof(Color), default, default);

            Assert.Equal(DependencyProperty.UnsetValue, result);
        }
    }
}
