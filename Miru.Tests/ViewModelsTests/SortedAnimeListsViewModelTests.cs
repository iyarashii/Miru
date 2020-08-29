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
    public class SortedAnimeListsViewModelTests
    {
        #region method tests

        [Fact]
        public void SetAnimeSortedByAirDayOfWeekAndFilteredByGivenAnimeListType_CallsCorrectMethods()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var sut = mock.Create<SortedAnimeListsViewModel>();

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
                var sut = mock.Create<SortedAnimeListsViewModel>(new TypedParameter(typeof(IMiruAnimeModelProcessor), fakeMiruAnimeModelProcessor));

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
                var sut = mock.Create<SortedAnimeListsViewModel>();
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
                var sut = mock.Create<SortedAnimeListsViewModel>();
                var testValue = It.IsAny<IEnumerable<MiruAnimeModel>>();

                // Act
                sut.TuesdayAiringAnimeList = testValue;

                // Assert
                Assert.Equal(testValue, sut.TuesdayAiringAnimeList);
            }
        }

        [Fact]
        public void WednesdayAiringAnimeList_StoresCorrectly()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var sut = mock.Create<SortedAnimeListsViewModel>();
                var testValue = It.IsAny<IEnumerable<MiruAnimeModel>>();

                // Act
                sut.WednesdayAiringAnimeList = testValue;

                // Assert
                Assert.Equal(testValue, sut.WednesdayAiringAnimeList);
            }
        }

        [Fact]
        public void ThursdayAiringAnimeList_StoresCorrectly()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var sut = mock.Create<SortedAnimeListsViewModel>();
                var testValue = It.IsAny<IEnumerable<MiruAnimeModel>>();

                // Act
                sut.ThursdayAiringAnimeList = testValue;

                // Assert
                Assert.Equal(testValue, sut.ThursdayAiringAnimeList);
            }
        }

        [Fact]
        public void FridayAiringAnimeList_StoresCorrectly()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var sut = mock.Create<SortedAnimeListsViewModel>();
                var testValue = It.IsAny<IEnumerable<MiruAnimeModel>>();

                // Act
                sut.FridayAiringAnimeList = testValue;

                // Assert
                Assert.Equal(testValue, sut.FridayAiringAnimeList);
            }
        }

        [Fact]
        public void SaturdayAiringAnimeList_StoresCorrectly()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var sut = mock.Create<SortedAnimeListsViewModel>();
                var testValue = It.IsAny<IEnumerable<MiruAnimeModel>>();

                // Act
                sut.SaturdayAiringAnimeList = testValue;

                // Assert
                Assert.Equal(testValue, sut.SaturdayAiringAnimeList);
            }
        }

        [Fact]
        public void SundayAiringAnimeList_StoresCorrectly()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var sut = mock.Create<SortedAnimeListsViewModel>();
                var testValue = It.IsAny<IEnumerable<MiruAnimeModel>>();

                // Act
                sut.SundayAiringAnimeList = testValue;

                // Assert
                Assert.Equal(testValue, sut.SundayAiringAnimeList);
            }
        }

    }
}