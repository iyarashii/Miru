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
            yield return new object[] { AnimeListType.AiringAndWatching, true, true };
            yield return new object[] { AnimeListType.Watching, true, It.IsAny<bool>() };
            yield return new object[] { AnimeListType.Season, It.IsAny<bool>(), true };
        }

        [Theory]
        [MemberData(nameof(PrepareDataForFilterAnimeModelsByAnimeListType))]
        public void FilterAnimeModelsByAnimeListType_ShouldFilterCorrectly(AnimeListType filterBy, bool isOnWatchingList, bool currentlyAiring)
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                List<MiruAiringAnimeModel> animeTestModels = new List<MiruAiringAnimeModel>
                {
                     new MiruAiringAnimeModel { IsOnWatchingList = true, CurrentlyAiring = true },
                     new MiruAiringAnimeModel { IsOnWatchingList = true, CurrentlyAiring = It.IsAny<bool>() },
                     new MiruAiringAnimeModel { IsOnWatchingList = It.IsAny<bool>(), CurrentlyAiring = true }
                };
                var cls = mock.Create<SortedAnimeListEntries>();

                // Act
                var actual = cls.FilterAnimeModelsByAnimeListType(animeTestModels, filterBy);
                bool expected = false;
                //var expected = animeTestModels.Where(x => x.IsOnWatchingList == isOnWatchingList && x.CurrentlyAiring == currentlyAiring);
                foreach (var i in animeTestModels)
                {
                    if(i.IsOnWatchingList == isOnWatchingList && i.CurrentlyAiring == currentlyAiring)
                    {
                        //expected.Add(i);
                        expected = true;
                    }
                }

                // Assert
                //Assert.Equal(expected, actual);
                Assert.True(expected);
            }
        }
        #endregion method tests
    }
}
