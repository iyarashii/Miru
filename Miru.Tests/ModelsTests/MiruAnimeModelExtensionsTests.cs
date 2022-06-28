// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using Autofac;
using Autofac.Extras.Moq;
using JikanDotNet;
using MiruLibrary;
using MiruLibrary.Models;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Xunit;
using AnimeType = MiruLibrary.AnimeType;

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

        public static IEnumerable<object[]> PrepareDataForSetAiringAnimeModelData()
        {
            yield return new object[] { AiringStatus.Airing, "not null", null };
            yield return new object[] { AiringStatus.Completed, null, null };
            yield return new object[] { AiringStatus.Completed, null, new TimePeriod() { From = new DateTime(2022, 3, 14) } };
        }

        [Theory]
        [MemberData(nameof(PrepareDataForSetAiringAnimeModelData))]
        public void SetAiringAnimeModelData_GivenValidInput_SetsDataCorrectly(AiringStatus airingStatus, string broadcast, TimePeriod timePeriod)
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
                Aired = timePeriod
            };
            var animeListEntry = new AnimeListEntry()
            {
                TotalEpisodes = TOTAL_EPISODES,
                URL = URL,
                WatchedEpisodes = WATCHED_EPISODES,
                AiringStatus = airingStatus
            };

            // Act
            sut.SetAiringAnimeModelData(animeInfo, animeListEntry, null);

            // Assert
            Assert.Equal(MAL_ID, sut.MalId);
            Assert.Equal(TITLE, sut.Title);
            Assert.Equal(IMAGE_URL, sut.ImageURL);
            Assert.Equal(TOTAL_EPISODES, sut.TotalEpisodes);
            Assert.Equal(URL, sut.URL);
            Assert.Equal(WATCHED_EPISODES, sut.WatchedEpisodes);
            Assert.Equal(TYPE, sut.Type);
            Assert.True(sut.IsOnWatchingList);
            Assert.Equal(Path.Combine(Constants.ImageCacheFolderPath, $"{MAL_ID}.jpg"), sut.LocalImagePath);
            Assert.Equal(airingStatus == AiringStatus.Airing, sut.CurrentlyAiring);
            Assert.Equal(broadcast ?? animeInfo.Aired?.From.ToString(), sut.Broadcast);
            Assert.False(sut.Dropped);
        }

        [Fact]
        public void SetAiringAnimeModelData_MatchingMalIdOnDroppedList_DroppedIsTrue()
        {
            using (var autoMock = AutoMock.GetLoose())
            {
                // Arrange
                var sut = new MiruAnimeModel();
                var animeInfo = new Anime()
                {
                    MalId = MAL_ID,
                };
                var animeListEntry = new AnimeListEntry()
                {
                    MalId = MAL_ID,
                };
                var currentUserAnimeList = new UserAnimeList
                {
                    Anime = new List<AnimeListEntry> { animeListEntry }
                };
                autoMock.Mock<ICurrentUserAnimeListModel>()
                    .SetupGet(x => x.UserDroppedAnimeListData)
                    .Returns(currentUserAnimeList);

                // Act
                sut.SetAiringAnimeModelData(animeInfo, animeListEntry, autoMock.Create<ICurrentUserAnimeListModel>());

                // Assert
                Assert.True(sut.Dropped);
            }
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
            sut.SetSeasonalAnimeModelData(animeInfo, animeSubEntry, null);

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
            Assert.False(sut.Dropped);
        }

        [Fact]
        public void SetSeasonalAnimeModelData_MatchingMalIdOnDroppedList_DroppedIsTrue()
        {
            using (var autoMock = AutoMock.GetLoose())
            {
                // Arrange
                var sut = new MiruAnimeModel();
                var animeInfo = new Anime()
                {
                    MalId = MAL_ID,
                };
                var animeListEntry = new AnimeSubEntry()
                {
                    MalId = MAL_ID,
                };
                var currentUserAnimeList = new UserAnimeList
                {
                    Anime = new List<AnimeListEntry> 
                    {
                        new AnimeListEntry { MalId = MAL_ID }
                    }
                };
                autoMock.Mock<ICurrentUserAnimeListModel>()
                    .SetupGet(x => x.UserDroppedAnimeListData)
                    .Returns(currentUserAnimeList);

                // Act
                sut.SetSeasonalAnimeModelData(animeInfo, animeListEntry, autoMock.Create<ICurrentUserAnimeListModel>());

                // Assert
                Assert.True(sut.Dropped);
            }
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
                var parsed = DateTime.TryParseExact(deserializedSenpaiEntry.Items.First().Airdate, formats, jpCultureInfo, DateTimeStyles.None, out DateTime parsedSenpaiBroadcast);
                var compareLocalBroadcastAnime = new MiruAnimeModel() { JSTBroadcastTime = parsedSenpaiBroadcast };
                compareLocalBroadcastAnime.ConvertJstBroadcastTimeToSelectedTimeZone(TimeZoneInfo.Local);

                // Assert
                Assert.True(sut.First().IsOnSenpai);
                Assert.Equal(deserializedSenpaiEntry.Items.First().Airdate, sut.First().Broadcast);
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

        [Fact]
        public void ParseTimeFromBroadcast_AnimeIsInSenpaiEntriesAndAirdateParseFailsOnce_SetsCorrectBroadcastTimes()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var senpaiEntry = @"{
                                    ""Items"": [
                                                {
                                                    ""MALID"": 40507,
                                                    ""airdate"": ""1337/1/2022 23:30""
                                                }
                                               ]
                                    }";
                var jpCultureInfo = CultureInfo.GetCultureInfo("ja-JP");
                string[] formats = { "dd/MM/yyyy HH:mm", "d/MM/yyyy HH:mm", "dd/M/yyyy HH:mm", "d/M/yyyy HH:mm" };
                var deserializedSenpaiEntry = JsonConvert.DeserializeObject<SenpaiEntryModel>(senpaiEntry);
                mock.Mock<IFileSystemService>().Setup(x => x.FileSystem.File.ReadAllText(It.IsAny<string>())).Returns(senpaiEntry);
                string airingAnimeBroadcastInCorrectFormat = "09/04/2000 18:00";
                var mockFileSystemService = mock.Create<IFileSystemService>();
                var sut = new List<MiruAnimeModel>
                {
                    new MiruAnimeModel {Title = "10", Type = "TV", MalId = 40507, Broadcast = airingAnimeBroadcastInCorrectFormat,},
                };

                // Act
                sut.ParseTimeFromBroadcast(mockFileSystemService);
                var parsed = DateTime.TryParseExact(airingAnimeBroadcastInCorrectFormat, formats, jpCultureInfo, DateTimeStyles.None, out DateTime parsedSenpaiBroadcast);
                var expectedLocalBroadcastAnime = new MiruAnimeModel() { JSTBroadcastTime = parsedSenpaiBroadcast };
                expectedLocalBroadcastAnime.ConvertJstBroadcastTimeToSelectedTimeZone(TimeZoneInfo.Local);

                // Assert
                Assert.True(sut.First().IsOnSenpai);
                Assert.Equal(airingAnimeBroadcastInCorrectFormat, sut.First().Broadcast);
                Assert.Equal(parsedSenpaiBroadcast, sut.First().JSTBroadcastTime);
                Assert.Equal(expectedLocalBroadcastAnime.LocalBroadcastTime, sut.First().LocalBroadcastTime);
            }
        }

        [Fact]
        public void ParseTimeFromBroadcast_AnimeIsInSenpaiEntriesAndAirdateParseFailsTwice_SetsCorrectBroadcastTimes()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange
                var senpaiEntry = @"{
                                    ""Items"": [
                                                {
                                                    ""MALID"": 40507,
                                                    ""airdate"": ""1337/1/2022 23:30""
                                                }
                                               ]
                                    }";
                var jpCultureInfo = CultureInfo.GetCultureInfo("ja-JP");
                string[] formats = { "dd/MM/yyyy HH:mm", "d/MM/yyyy HH:mm", "dd/M/yyyy HH:mm", "d/M/yyyy HH:mm" };
                var deserializedSenpaiEntry = JsonConvert.DeserializeObject<SenpaiEntryModel>(senpaiEntry);
                mock.Mock<IFileSystemService>().Setup(x => x.FileSystem.File.ReadAllText(It.IsAny<string>())).Returns(senpaiEntry);
                string airingAnimeBroadcastInMalFormat = "Mondays at 18:00";
                var mockFileSystemService = mock.Create<IFileSystemService>();
                var sut = new List<MiruAnimeModel>
                {
                    new MiruAnimeModel {Title = "10", Type = "TV", MalId = 40507, Broadcast = airingAnimeBroadcastInMalFormat,},
                };

                // Act
                sut.ParseTimeFromBroadcast(mockFileSystemService);
                var parsedBroadcast = new DateTime(2020, 01, 20, 18, 0, 0);
                var expectedLocalBroadcastAnime = new MiruAnimeModel() { JSTBroadcastTime = parsedBroadcast };
                expectedLocalBroadcastAnime.ConvertJstBroadcastTimeToSelectedTimeZone(TimeZoneInfo.Local);

                // Assert
                Assert.True(sut.First().IsOnSenpai);
                Assert.Equal(airingAnimeBroadcastInMalFormat, sut.First().Broadcast);
                Assert.Equal(parsedBroadcast, sut.First().JSTBroadcastTime);
                Assert.Equal(expectedLocalBroadcastAnime.LocalBroadcastTime, sut.First().LocalBroadcastTime);
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("one_word")]
        public void TryParseAndSetAirTimeFromMyAnimeList_LessThanThreeBroadcastWords_ReturnFalse(string broadcast)
        {
            var sut = new MiruAnimeModel() { Broadcast = broadcast };

            var result = sut.TryParseAndSetAirTimeFromMyAnimeList();

            Assert.False(result);
        }

        [Fact]
        public void TryParseAndSetAirTimeFromMyAnimeList_BroadcastDateParseFail_ReturnFalse()
        {
            var sut = new MiruAnimeModel() { Broadcast = "test test 25:00" };

            var result = sut.TryParseAndSetAirTimeFromMyAnimeList();

            Assert.False(result);
        }

        [Theory]
        [InlineData("Mondays at 01:11", DayOfWeek.Monday, 1, 11)]
        [InlineData("Tuesdays at 2:22", DayOfWeek.Tuesday, 2, 22)]
        [InlineData("Wednesdays at 21:37", DayOfWeek.Wednesday, 21, 37)]
        [InlineData("Thursdays at 01:01", DayOfWeek.Thursday, 1, 1)]
        [InlineData("Fridays at 22:05", DayOfWeek.Friday, 22, 5)]
        [InlineData("Saturdays at 03:5", DayOfWeek.Saturday, 3, 5)]
        [InlineData("Sundays at 9:9", DayOfWeek.Sunday, 9, 9)]
        [InlineData("Unknown at 00:00", DayOfWeek.Monday, 0, 0)]
        public void TryParseAndSetAirTimeFromMyAnimeList_BroadcastDateParseSuccess_ReturnTrueAndSetJstBroadcastTime(
            string broadcast, DayOfWeek expectedDay, int expectedHour, int expectedMinute)
        {
            var sut = new MiruAnimeModel() { Broadcast = broadcast };

            var result = sut.TryParseAndSetAirTimeFromMyAnimeList();

            Assert.True(result);
            Assert.Equal(expectedDay, sut.JSTBroadcastTime.Value.DayOfWeek);
            Assert.Equal(expectedHour, sut.JSTBroadcastTime.Value.Hour);
            Assert.Equal(expectedMinute, sut.JSTBroadcastTime.Value.Minute);
        }
    }
}
