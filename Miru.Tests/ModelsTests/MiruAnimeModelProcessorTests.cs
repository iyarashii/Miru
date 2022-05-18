// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using Autofac;
using Autofac.Extras.Moq;
using Miru.ViewModels;
using MiruLibrary;
using MiruLibrary.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Miru.Tests.ModelsTests
{
    public class MiruAnimeModelProcessorTests
    {
        public static IEnumerable<object[]> PrepareDataForFilterAnimeModelsByAnimeListType()
        {
            yield return new object[] { AnimeListType.AiringAndWatching, new Predicate<MiruAnimeModel>(x => x.IsOnWatchingList && x.CurrentlyAiring) };
            yield return new object[] { AnimeListType.Watching, new Predicate<MiruAnimeModel>(x => x.IsOnWatchingList) };
            yield return new object[] { AnimeListType.Season, new Predicate<MiruAnimeModel>(x => x.CurrentlyAiring) };
        }

        [Theory]
        [MemberData(nameof(PrepareDataForFilterAnimeModelsByAnimeListType))]
        public void FilterAnimeModelsByAnimeListType_ShouldFilterCorrectly(AnimeListType filterBy, Predicate<MiruAnimeModel> expectedFilterPredicate)
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                List<MiruAnimeModel> animeTestModels = new List<MiruAnimeModel>
                {
                     new MiruAnimeModel { IsOnWatchingList = true, CurrentlyAiring = true },
                     new MiruAnimeModel { IsOnWatchingList = true, CurrentlyAiring = false },
                     new MiruAnimeModel { IsOnWatchingList = false, CurrentlyAiring = true }
                };
                var sut = mock.Create<MiruAnimeModelProcessor>();

                // Act
                var filteredList = sut.FilterAnimeModelsByAnimeListType(animeTestModels, filterBy).ToList();

                // Assert
                Assert.True(filteredList.TrueForAll(expectedFilterPredicate));
            }
        }

        [Fact]
        public void FilterAnimeModelsByAirDayOfWeekAndOrderByAirTime_FiltersAndOrdersCorrectly()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                List<MiruAnimeModel> animeTestModels = new List<MiruAnimeModel>
                {
                    // 20/07/2020 was Monday
                     new MiruAnimeModel { LocalBroadcastTime = DateTime.Parse("11:00 20/07/2020")},
                     new MiruAnimeModel { LocalBroadcastTime = DateTime.Parse("15:00 20/07/2020")},
                     new MiruAnimeModel { LocalBroadcastTime = DateTime.Parse("20:00 20/07/2020")},
                     new MiruAnimeModel { LocalBroadcastTime = DateTime.Parse("20:00 21/07/2020")},
                     new MiruAnimeModel { LocalBroadcastTime = DateTime.Parse("20:00 22/07/2020")},
                     new MiruAnimeModel { LocalBroadcastTime = DateTime.Parse("20:00 23/07/2020")},
                     new MiruAnimeModel { LocalBroadcastTime = DateTime.Parse("20:00 24/07/2020")},
                     new MiruAnimeModel { LocalBroadcastTime = DateTime.Parse("20:00 25/07/2020")},
                     new MiruAnimeModel { LocalBroadcastTime = DateTime.Parse("20:00 26/07/2020")}
                };
                var sut = mock.Create<MiruAnimeModelProcessor>();

                // Act
                var filteredList = sut.FilterAnimeModelsByAirDayOfWeekAndOrderByAirTime(animeTestModels, DayOfWeek.Monday).ToList();

                // Assert
                Assert.True(filteredList.TrueForAll(x => x.LocalBroadcastTime.Value.DayOfWeek == DayOfWeek.Monday));
                Assert.Collection(filteredList,
                                  x => Assert.True(x.LocalBroadcastTime.Value.TimeOfDay == new TimeSpan(11, 00, 00)),
                                  x => Assert.True(x.LocalBroadcastTime.Value.TimeOfDay == new TimeSpan(15, 00, 00)),
                                  x => Assert.True(x.LocalBroadcastTime.Value.TimeOfDay == new TimeSpan(20, 00, 00)));
            }
        }
    }
}
