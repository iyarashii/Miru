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
using JikanDotNet;
using System.IO;
using AnimeType = MiruLibrary.AnimeType;
using Newtonsoft.Json;
using System.Globalization;

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
                Assert.All(animeList, x => Assert.Contains(titleToFilterBy, x.Title, StringComparison.OrdinalIgnoreCase));
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

                var converter = new EnumDescriptionTypeConverter(typeof(AnimeType));
                var animeBroadcastTypeDescription = converter.ConvertToString(broadcastType);

                // Act
                animeList.FilterByBroadcastType(broadcastType);

                // Assert
                Assert.All(animeList, x => Assert.Contains(x.Type, animeBroadcastTypeDescription));
                Assert.Equal(expectedListCount, animeList.Count());
            }
        }

        public static IEnumerable<object[]> GetTimeZoneData() // Tokyo Standard Time = UTC+9
        {
            yield return new object[] { TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time"),
                TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time").GetUtcOffset(DateTime.UtcNow) }; // UTC+1

            yield return new object[] { TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"),
                TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time").GetUtcOffset(DateTime.UtcNow) }; // UTC-5/-4 depending on daylight saving time

            yield return new object[] { TimeZoneInfo.FindSystemTimeZoneById("AUS Eastern Standard Time"),
                TimeZoneInfo.FindSystemTimeZoneById("AUS Eastern Standard Time").GetUtcOffset(DateTime.UtcNow) }; // UTC+10

            yield return new object[] { TimeZoneInfo.FindSystemTimeZoneById("Alaskan Standard Time"),
                TimeZoneInfo.FindSystemTimeZoneById("Alaskan Standard Time").GetUtcOffset(DateTime.UtcNow) }; // UTC-9
        }

        [Theory]
        [MemberData(nameof(GetTimeZoneData))]
        public void ConvertJstBroadcastTimeToSelectedTimeZone_GivenAnimeModelsWithJstBroadcastTimePresent_WorksCorrectly(TimeZoneInfo timeZone, TimeSpan utcOffset)
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

        const string TITLE = "takodachis adventure";
        const int MAL_ID = 1337;
        const string IMAGE_URL = "KEKW";
        const string TYPE = "TV";
        const int TOTAL_EPISODES = 777;
        const string URL = "🐙";
        const int WATCHED_EPISODES = 39;

        [Theory]
        [InlineData(AiringStatus.Airing, "not null")]
        [InlineData(AiringStatus.Completed, null)]
        public void SetAiringAnimeModelData_GivenValidInput_SetsDataCorrectly(AiringStatus airingStatus, string broadcast)
        {
            // Arrange
            var sut = new MiruAnimeModel();
            DateTime airedFromDate = new DateTime(2022, 3, 14);
            var animeInfo = new Anime()
            {
                MalId = MAL_ID,
                Broadcast = broadcast,
                Title = TITLE,
                ImageURL = IMAGE_URL,
                Type = TYPE,
                Aired = new TimePeriod() { From = airedFromDate }
            };
            var animeListEntry = new AnimeListEntry()
            {
                TotalEpisodes = TOTAL_EPISODES,
                URL = URL,
                WatchedEpisodes = WATCHED_EPISODES,
                AiringStatus = airingStatus
            };

            // Act
            sut.SetAiringAnimeModelData(animeInfo, animeListEntry);

            // Assert
            Assert.Equal(MAL_ID, sut.MalId);
            Assert.Equal(TITLE, sut.Title);
            Assert.Equal(IMAGE_URL, sut.ImageURL);
            Assert.Equal(TOTAL_EPISODES, sut.TotalEpisodes);
            Assert.Equal(URL, sut.URL);
            Assert.Equal(WATCHED_EPISODES, sut.WatchedEpisodes);
            Assert.Equal(TYPE, sut.Type);
            Assert.True(sut.IsOnWatchingList);
            Assert.Equal(Path.Combine(Constants.ImageCacheFolderPath, $"{ MAL_ID }.jpg"), sut.LocalImagePath);
            Assert.Equal(airingStatus == AiringStatus.Airing, sut.CurrentlyAiring);
            Assert.Equal(broadcast ?? airedFromDate.ToString(), sut.Broadcast);
        }

        [Theory]
        [InlineData("not null")]
        [InlineData(null)]
        public void SetSeasonalAnimeModelData_ValidInput_SetsDataCorrectly(string broadcast)
        {
            // Arrange
            var sut = new MiruAnimeModel();
            DateTime airedFromDate = new DateTime(2022, 3, 14);
            var animeInfo = new Anime()
            {
                MalId = MAL_ID,
                Broadcast = broadcast,
                Title = TITLE,
                ImageURL = IMAGE_URL,
                Type = TYPE,
                Aired = new TimePeriod() { From = airedFromDate }
            };
            var animeSubEntry = new AnimeSubEntry()
            {
                URL = URL,
            };

            // Act
            sut.SetSeasonalAnimeModelData(animeInfo, animeSubEntry);

            // Assert
            Assert.Equal(MAL_ID, sut.MalId);
            Assert.Equal(TITLE, sut.Title);
            Assert.Equal(IMAGE_URL, sut.ImageURL);
            Assert.Equal(URL, sut.URL);
            Assert.Equal(TYPE, sut.Type);
            Assert.False(sut.IsOnWatchingList);
            Assert.Equal(Path.Combine(Constants.ImageCacheFolderPath, $"{ MAL_ID }.jpg"), sut.LocalImagePath);
            Assert.True(sut.CurrentlyAiring);
            Assert.Equal(broadcast ?? airedFromDate.ToString(), sut.Broadcast);
        }

        [Fact]
        public void ParseTimeFromBroadcast_AnimeIsInSenpaiEntriesAndAirdateParseIsSuccessful_SetsCorrectBroadcastTimes()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var senpaiEntry = @"{
                                    ""Items"": [
                                                {
                                                    ""MALID"": 40507,
                                                    ""airdate"": ""13/1/2022 23:30""
                                                }
                                               ]
                                    }";
                var jpCultureInfo = CultureInfo.GetCultureInfo("ja-JP");
                string[] formats = { "dd/MM/yyyy HH:mm", "d/MM/yyyy HH:mm", "dd/M/yyyy HH:mm", "d/M/yyyy HH:mm" };
                var deserializedSenpaiEntry = JsonConvert.DeserializeObject<SenpaiEntryModel>(senpaiEntry);
                mock.Mock<IFileSystemService>().Setup(x => x.FileSystem.File.ReadAllText(It.IsAny<string>())).Returns(senpaiEntry);

                var mockFileSystemService = mock.Create<IFileSystemService>();
                var sut = new List<MiruAnimeModel>
                {
                    new MiruAnimeModel {Title = "10", Type = "TV", MalId = 40507, Broadcast = "",},
                };

                // Act
                sut.ParseTimeFromBroadcast(mockFileSystemService);
                var parsed = DateTime.TryParseExact(deserializedSenpaiEntry.Items.First().airdate, formats, jpCultureInfo, DateTimeStyles.None, out DateTime parsedSenpaiBroadcast);
                var compareLocalBroadcastAnime = new MiruAnimeModel() { JSTBroadcastTime = parsedSenpaiBroadcast };
                compareLocalBroadcastAnime.ConvertJstBroadcastTimeToSelectedTimeZone(TimeZoneInfo.Local);

                // Assert
                Assert.True(sut.First().IsOnSenpai);
                Assert.Equal(deserializedSenpaiEntry.Items.First().airdate, sut.First().Broadcast);
                Assert.Equal(parsedSenpaiBroadcast, sut.First().JSTBroadcastTime);
                Assert.Equal(compareLocalBroadcastAnime.LocalBroadcastTime, sut.First().LocalBroadcastTime);
            }
        }

        [Fact]
        public void ParseTimeFromBroadcast_AnimeIsNotInSenpaiEntriesAndAirdateParseIsSuccessful_SetsCorrectBroadcastTimes()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var senpaiEntry = @"{
                                    ""Items"": [
                                                {
                                                    ""MALID"": 40507,
                                                    ""airdate"": ""13/1/2022 23:30""
                                                }
                                               ]
                                    }";
                var jpCultureInfo = CultureInfo.GetCultureInfo("ja-JP");
                string[] formats = { "dd/MM/yyyy HH:mm", "d/MM/yyyy HH:mm", "dd/M/yyyy HH:mm", "d/M/yyyy HH:mm" };
                mock.Mock<IFileSystemService>().Setup(x => x.FileSystem.File.ReadAllText(It.IsAny<string>())).Returns(senpaiEntry);

                var mockFileSystemService = mock.Create<IFileSystemService>();
                var sut = new List<MiruAnimeModel>
                {
                    new MiruAnimeModel {Title = "10", Type = "TV", MalId = 1, Broadcast = "21/10/2022 23:30",},
                };

                // Act
                sut.ParseTimeFromBroadcast(mockFileSystemService);
                var parsed = DateTime.TryParseExact("21/10/2022 23:30", formats, jpCultureInfo, DateTimeStyles.None, out DateTime parsedBroadcast);
                var compareLocalBroadcastAnime = new MiruAnimeModel() { JSTBroadcastTime = parsedBroadcast };
                compareLocalBroadcastAnime.ConvertJstBroadcastTimeToSelectedTimeZone(TimeZoneInfo.Local);

                // Assert
                Assert.False(sut.First().IsOnSenpai);
                Assert.Equal("21/10/2022 23:30", sut.First().Broadcast);
                Assert.Equal(parsedBroadcast, sut.First().JSTBroadcastTime);
                Assert.Equal(compareLocalBroadcastAnime.LocalBroadcastTime, sut.First().LocalBroadcastTime);
            }
        }
    }
}
