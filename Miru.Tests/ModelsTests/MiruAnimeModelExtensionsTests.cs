using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using MiruLibrary.Models;
using MiruLibrary;
using Autofac.Extras.Moq;

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
    }
}
