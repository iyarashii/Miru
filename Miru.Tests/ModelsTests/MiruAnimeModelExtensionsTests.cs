using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using MiruLibrary.Models;
using MiruLibrary;
using Autofac.Extras.Moq;
using Moq;

namespace Miru.Tests.ModelsTests
{
    public class MiruAnimeModelExtensionsTests
    {
        [Fact]
        public void FilterByTitle_GivenEmptyString_DoesNothing()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var animeList = new List<MiruAnimeModel> { new MiruAnimeModel() };
                var animeListExpected = animeList.ToList(); // create new list instance instead of copying reference

                // Act
                animeList.FilterByTitle(string.Empty);

                // Assert
                Assert.NotSame(animeListExpected, animeList);
                Assert.Equal(animeListExpected, animeList);
            }
        }

        [Theory]
        [InlineData("dydo", 2)]
        [InlineData("d", 4)]
        [InlineData("pog", 3)]
        [InlineData("a", 2)]
        [InlineData("X", 0)]
        public void FilterByTitle_GivenNotEmptyString_FiltersAnimeList(string titleToFilterBy, int expectedCountOfTitlesMatching)
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var animeList = new List<MiruAnimeModel> 
                { 
                    new MiruAnimeModel() { Title = "DYDO" }, 
                    new MiruAnimeModel() { Title = "dydo" },
                    new MiruAnimeModel() { Title = "dd" },
                    new MiruAnimeModel() { Title = "dank" },
                    new MiruAnimeModel() { Title = "poggers" },
                    new MiruAnimeModel() { Title = "pog" },
                    new MiruAnimeModel() { Title = "pogchamp" },
                    
                };
                var animeListExpected = animeList
                    .Where(x => x.Title.IndexOf(titleToFilterBy, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();

                // Act
                animeList.FilterByTitle(titleToFilterBy);

                // Assert
                Assert.Equal(expectedCountOfTitlesMatching, animeList.Count);
                Assert.Equal(animeListExpected, animeList);
            }
        }

        [Theory]
        [InlineData(AnimeType.Both, 3)]
        [InlineData(AnimeType.ONA, 2)]
        [InlineData(AnimeType.TV, 1)]
        public void FilterByBroadcastType_GivenBroadcastType_ShouldFilterAnimeList(AnimeType broadcastType, int expectedListCount)
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var animeList = new List<MiruAnimeModel>
                {
                    new MiruAnimeModel() { Type = "TV"},
                    new MiruAnimeModel() { Type = "ONA"},
                    new MiruAnimeModel() { Type = "ONA"},
                    new MiruAnimeModel() { Type = string.Empty},
                };

                var animeListExpected = animeList
                    .Where(x => 
                    broadcastType == AnimeType.Both ? 
                    x.Type == "TV" || x.Type == "ONA" :
                    x.Type == broadcastType.ToString())
                    .ToList();

                // Act
                animeList.FilterByBroadcastType(broadcastType);

                // Assert
                Assert.Equal(expectedListCount, animeList.Count);
                Assert.Equal(animeListExpected, animeList);
            }
        }

        public static IEnumerable<object[]> GetTimeZoneData() // Tokyo Standard Time = UTC+9
        {
            yield return new object[] { TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time"),
                TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time").BaseUtcOffset }; // UTC+1

            yield return new object[] { TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"),
                TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time").BaseUtcOffset }; // UTC-5

            yield return new object[] { TimeZoneInfo.FindSystemTimeZoneById("AUS Eastern Standard Time"),
                TimeZoneInfo.FindSystemTimeZoneById("AUS Eastern Standard Time").GetUtcOffset(DateTime.UtcNow) }; // UTC+10

            yield return new object[] { TimeZoneInfo.FindSystemTimeZoneById("Alaskan Standard Time"),
                TimeZoneInfo.FindSystemTimeZoneById("Alaskan Standard Time").BaseUtcOffset }; // UTC-9
        }

        [Theory]
        [MemberData(nameof(GetTimeZoneData))]
        public void ConvertJstBroadcastTimeToSelectedTimeZone_GivenAnimeModelsWithJstBroadcastTimePresent_WorksCorrectly(TimeZoneInfo timeZone, TimeSpan utcOffset)
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var data = new List<MiruAnimeModel>
                {
                    new MiruAnimeModel { JSTBroadcastTime = new DateTime(2022, 2, 1, 10, 0, 0)},
                    new MiruAnimeModel { JSTBroadcastTime = new DateTime(2022, 2, 1, 20, 0, 0)},
                    new MiruAnimeModel { JSTBroadcastTime = new DateTime(2022, 2, 1, 15, 0, 0)},
                    new MiruAnimeModel { JSTBroadcastTime = new DateTime(2022, 2, 1, 0, 0, 0)},
                };

                // Act
                data.ForEach(x => x.ConvertJstBroadcastTimeToSelectedTimeZone(timeZone));

                // Assert
                Assert.All(data, x => Assert.Equal(x.JSTBroadcastTime.Value.AddHours(-9.0).Add(utcOffset).Hour, x.LocalBroadcastTime.Value.Hour));
            }
        }

        [Fact]
        public void ConvertJstBroadcastTimeToSelectedTimeZone_GivenEmptyJstBroadcastTime_SetLocalBroadcastTimeForToday()        
        {
            // Arrange
            var sut = new MiruAnimeModel { JSTBroadcastTime = null };

            // Act
            sut.ConvertJstBroadcastTimeToSelectedTimeZone(It.IsAny<TimeZoneInfo>());

            // Assert
            Assert.True(sut.LocalBroadcastTime == DateTime.Today);
        }
    }
}
