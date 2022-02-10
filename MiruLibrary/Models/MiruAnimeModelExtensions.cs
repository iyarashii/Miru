using JikanDotNet;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MiruLibrary.Models
{
    public static class MiruAnimeModelExtensions
    {
        public static void FilterByTitle(this List<MiruAnimeModel> animeList, string title)
        {
            if (!string.IsNullOrWhiteSpace(title))
            {
                animeList.RemoveAll(x => !(x.Title.IndexOf(title, StringComparison.OrdinalIgnoreCase) >= 0));
            }
        }

        // filter list of the animes depending on the broadcast type
        public static void FilterByBroadcastType(this List<MiruAnimeModel> animeList, AnimeType broadcastType)
        {
            switch (broadcastType)
            {
                case AnimeType.Both:
                    animeList.RemoveAll(x => x.Type != "TV" && x.Type != "ONA");
                    break;

                case AnimeType.TV:
                    animeList.RemoveAll(x => x.Type != "TV");
                    break;

                case AnimeType.ONA:
                    animeList.RemoveAll(x => x.Type != "ONA");
                    break;
            }
        }

        public static void ConvertJstBroadcastTimeToSelectedTimeZone(this MiruAnimeModel anime, TimeZoneInfo selectedTimeZone)
        {
            if (!anime.JSTBroadcastTime.HasValue)
            {
                anime.LocalBroadcastTime = DateTime.Today;
                return;
            }
            // covert JST to UTC
            var broadcastTimeInSelectedTimeZone = TimeZoneInfo.ConvertTimeToUtc(anime.JSTBroadcastTime.Value, TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time"));

            // get the UTC offset for the selected time zone
            var utcOffset = selectedTimeZone.GetUtcOffset(DateTime.UtcNow);

            // return Japanese broadcast time converted to selected time zone
            anime.LocalBroadcastTime = broadcastTimeInSelectedTimeZone.Add(utcOffset);
        }

        /// <summary>
        /// Parses day and time from the broadcast string, converts it to the local time zone and saves to the LocalBroadcastTime property of the MiruAnimeModel.
        /// </summary>
        /// <param name="detailedAnimeList">List of airing animes with broadcast strings but without parsed day and time data.</param>
        /// <returns>List of airing animes with parsed data saved in LocalBroadcastTime properties.</returns>
        public static void ParseTimeFromBroadcast(this List<MiruAnimeModel> detailedAnimeList, IFileSystemService fileSystemService)
        {
            if (fileSystemService == null || detailedAnimeList.Count == 0)
                return;
            // local variables
            string dayOfTheWeek;
            string[] broadcastWords;
            DateTime broadcastTime = default;
            DateTime time;

            var jp = CultureInfo.GetCultureInfo("ja-JP");
            string[] formats = { "dd/MM/yyyy HH:mm", "d/MM/yyyy HH:mm" };

            // deserialize data from senpai as a backup source of anime broadcast time
            var senpaiEntries = JsonConvert.DeserializeObject<SenpaiEntryModel>(fileSystemService?.FileSystem?.File?.ReadAllText(Constants.SenpaiFilePath));
            var senpaiIDs = senpaiEntries.Items.Select(x => x.MALID).ToArray();

            // for each airingAnime parse time and day of the week from the broadcast string
            foreach (var airingAnime in detailedAnimeList)
            {
                bool parsed = false;
                if (senpaiIDs.Contains(airingAnime.MalId))
                {
                    airingAnime.IsOnSenpai = true;
                    var airDateAndTime = senpaiEntries.Items.First(x => x.MALID == airingAnime.MalId).airdate;
                    parsed = DateTime.TryParseExact(airDateAndTime, formats, jp, DateTimeStyles.None, out DateTime parsedSenpaiBroadcast);
                    if (parsed)
                    {
                        airingAnime.Broadcast = airDateAndTime;
                        broadcastTime = parsedSenpaiBroadcast;
                    }
                }
                else
                {
                    airingAnime.IsOnSenpai = false;
                }

                if (!parsed && DateTime.TryParseExact(airingAnime.Broadcast, formats, jp, DateTimeStyles.None, out DateTime parsedBroadcast))
                {
                    broadcastTime = parsedBroadcast;
                }
                else if (!parsed)
                {
                    // split the broadcast string into words
                    broadcastWords = airingAnime.Broadcast.Split(' ');

                    if (broadcastWords.Length <= 2)
                    {
                        continue;
                    }

                    // set the first word of the broadcast string as a day of the week
                    dayOfTheWeek = broadcastWords[0];

                    // parse time from the 2nd broadcast string word
                    if (!DateTime.TryParse(broadcastWords[2], out time))
                    {
                        continue;
                    }
                    
                    // depending on the 1st word set the correct day
                    switch (dayOfTheWeek)
                    {
                        case "Mondays":
                            broadcastTime = new DateTime(2020, 01, 20, time.Hour, time.Minute, 0);
                            break;

                        case "Tuesdays":
                            broadcastTime = new DateTime(2020, 01, 21, time.Hour, time.Minute, 0);
                            break;

                        case "Wednesdays":
                            broadcastTime = new DateTime(2020, 01, 22, time.Hour, time.Minute, 0);
                            break;

                        case "Thursdays":
                            broadcastTime = new DateTime(2020, 01, 23, time.Hour, time.Minute, 0);
                            break;

                        case "Fridays":
                            broadcastTime = new DateTime(2020, 01, 24, time.Hour, time.Minute, 0);
                            break;

                        case "Saturdays":
                            broadcastTime = new DateTime(2020, 01, 25, time.Hour, time.Minute, 0);
                            break;

                        case "Sundays":
                            broadcastTime = new DateTime(2020, 01, 26, time.Hour, time.Minute, 0);
                            break;

                        default:
                            broadcastTime = new DateTime();
                            break;
                    }
                    airingAnime.IsOnSenpai = false;
                }

                // save JST broadcast time parsed from the Broadcast string
                airingAnime.JSTBroadcastTime = broadcastTime;

                // save JST broadcast time converted to your computer's local time to the model's property
                airingAnime.ConvertJstBroadcastTimeToSelectedTimeZone(TimeZoneInfo.Local);
            }
        }

        public static void SetAiringAnimeModelData(this MiruAnimeModel animeModel, Anime animeInfo, AnimeListEntry animeListEntry)
        {
            animeModel.MalId = animeInfo.MalId;
            animeModel.Broadcast = animeInfo.Broadcast ?? animeInfo.Aired.From.ToString();
            animeModel.Title = animeInfo.Title;
            animeModel.ImageURL = animeInfo.ImageURL;
            animeModel.LocalImagePath = animeInfo.MalId.ToString();
            animeModel.TotalEpisodes = animeListEntry.TotalEpisodes;
            animeModel.URL = animeListEntry.URL;
            animeModel.WatchedEpisodes = animeListEntry.WatchedEpisodes;
            animeModel.IsOnWatchingList = true;
            animeModel.CurrentlyAiring = animeListEntry.AiringStatus == AiringStatus.Airing;
            animeModel.Type = animeInfo.Type;
        }

        public static void SetSeasonalAnimeModelData(this MiruAnimeModel animeModel, Anime animeInfo, AnimeSubEntry seasonEntry)
        {
            animeModel.MalId = animeInfo.MalId;
            animeModel.Broadcast = animeInfo.Broadcast ?? animeInfo.Aired.From.ToString();
            animeModel.Title = animeInfo.Title;
            animeModel.ImageURL = animeInfo.ImageURL;
            animeModel.LocalImagePath = animeInfo.MalId.ToString();
            animeModel.URL = seasonEntry.URL;
            animeModel.IsOnWatchingList = false;
            animeModel.CurrentlyAiring = true;
            animeModel.Type = animeInfo.Type;
        }
    }
}
