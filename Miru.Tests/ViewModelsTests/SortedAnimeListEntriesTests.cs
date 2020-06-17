using Autofac;
using Autofac.Extras.Moq;
using Miru.ViewModels;
using MiruLibrary;
using MiruLibrary.Models;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace Miru.Tests
{
    public class SortedAnimeListEntriesTests
    {
        #region method tests

        // TODO: move this to the MiruAnimeModelProcessor tests
        public static IEnumerable<object[]> PrepareDataForFilterAnimeModelsByAnimeListType()
        {
            yield return new object[] { AnimeListType.AiringAndWatching, new Predicate<MiruAnimeModel>(x => x.IsOnWatchingList && x.CurrentlyAiring) };
            yield return new object[] { AnimeListType.Watching, new Predicate<MiruAnimeModel>(x => x.IsOnWatchingList) };
            yield return new object[] { AnimeListType.Season, new Predicate<MiruAnimeModel>(x => x.CurrentlyAiring) };
        }

        // TODO: move this test to the MiruAnimeModelProcessor tests
        //[Theory]
        //[MemberData(nameof(PrepareDataForFilterAnimeModelsByAnimeListType))]
        //public void FilterAnimeModelsByAnimeListType_ShouldFilterCorrectly(AnimeListType filterBy, Predicate<MiruAnimeModel> expectedFilterPredicate)
        //{
        //    using (var mock = AutoMock.GetLoose())
        //    {
        //        // Arrange
        //        List<MiruAnimeModel> animeTestModels = new List<MiruAnimeModel>
        //        {
        //             new MiruAnimeModel { IsOnWatchingList = true, CurrentlyAiring = true },
        //             new MiruAnimeModel { IsOnWatchingList = true, CurrentlyAiring = false },
        //             new MiruAnimeModel { IsOnWatchingList = false, CurrentlyAiring = true }
        //        };
        //        var sut = mock.Create<SortedAnimeListEntries>();

        //        // Act
        //        var filteredList = sut.FilterAnimeModelsByAnimeListType(animeTestModels, filterBy);

        //        // Assert
        //        Assert.True(filteredList.TrueForAll(expectedFilterPredicate));
        //    }
        //}
        [Fact]
        public void SetAnimeSortedByAirDayOfWeekAndFilteredByGivenAnimeListType_CallsCorrectMethods()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var sut = mock.Create<SortedAnimeListEntries>();

                mock.Mock<IMiruAnimeModelProcessor>()
                   .Setup(x => x.FilterAnimeModelsByAnimeListType(null, It.IsAny<AnimeListType>()));

                mock.Mock<IMiruAnimeModelProcessor>()
                    .Setup(x => x.FilterAnimeModelsByAirDayOfWeekAndOrderByAirTime(It.IsAny<IEnumerable<MiruAnimeModel>>(), It.IsAny<DayOfWeek>()));

                // Act
                sut.SetAnimeSortedByAirDayOfWeekAndFilteredByGivenAnimeListType(null, It.IsAny<AnimeListType>());

                // Assert
                mock.Mock<IMiruAnimeModelProcessor>()
                    .Verify(x => x.FilterAnimeModelsByAnimeListType(null, It.IsAny<AnimeListType>()), Times.Once);
                mock.Mock<IMiruAnimeModelProcessor>()
                    .Verify(x => x.FilterAnimeModelsByAirDayOfWeekAndOrderByAirTime(It.IsAny<IEnumerable<MiruAnimeModel>>(), It.IsAny<DayOfWeek>()), Times.Exactly(7));
            }
        }

        #endregion method tests

        [Fact]
        public void MiruAnimeModelProcessor_ReturnsCorrectValue()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var fakeMiruAnimeModelProcessor = mock.Create<MiruAnimeModelProcessor>();

                // Act
                var sut = mock.Create<SortedAnimeListEntries>(new TypedParameter(typeof(IMiruAnimeModelProcessor), fakeMiruAnimeModelProcessor));

                // Assert
                Assert.Equal(fakeMiruAnimeModelProcessor, sut.MiruAnimeModelProcessor);
            }
        }

        [Fact]
        public void MondayAiringAnimeList_StoresCorrectly()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var sut = mock.Create<SortedAnimeListEntries>();
                var testValue = It.IsAny<IEnumerable<MiruAnimeModel>>();

                // Act
                sut.MondayAiringAnimeList = testValue;

                // Assert
                Assert.Equal(testValue, sut.MondayAiringAnimeList);
            }
        }

        [Fact]
        public void TuesdayAiringAnimeList_StoresCorrectly()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var sut = mock.Create<SortedAnimeListEntries>();
                var testValue = It.IsAny<IEnumerable<MiruAnimeModel>>();

                // Act
                sut.TuesdayAiringAnimeList = testValue;

                // Assert
                Assert.Equal(testValue, sut.TuesdayAiringAnimeList);
            }
        }
    }
}