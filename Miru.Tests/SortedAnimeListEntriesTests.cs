using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Miru;
using Miru.ViewModels;
using Xunit;
using Miru.Data;
using Miru.Models;
using Autofac.Extras.Moq;
using ModernWpf;
using ModernWpf.Controls;
using System.Windows.Media;
using Moq;
using ToastNotifications.Core;
using System.Collections.ObjectModel;
using Caliburn.Micro;
using System.Threading;

namespace Miru.Tests
{
    public class SortedAnimeListEntriesTests
    {
        #region method tests
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
                var sut = mock.Create<SortedAnimeListEntries>();

                // Act
                var filteredList = sut.FilterAnimeModelsByAnimeListType(animeTestModels, filterBy);

                // Assert
                Assert.True(filteredList.TrueForAll(expectedFilterPredicate));
            }
        }
        #endregion method tests
    }
}
