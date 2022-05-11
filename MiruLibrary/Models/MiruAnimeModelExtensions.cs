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
                animeList.RemoveAll(x => x.Title.IndexOf(title, StringComparison.OrdinalIgnoreCase) < 0);
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

        public static void ConvertJstBroadcastTimeToSelectedTimeZone(this MiruAnimeModel anime, 
            TimeZoneInfo selectedTimeZone)
        {
            if (!anime.JSTBroadcastTime.HasValue)
            {
                anime.LocalBroadcastTime = DateTime.Today;
                return;
            }
            // covert JST to UTC
            var broadcastTimeInSelectedTimeZone = TimeZoneInfo.ConvertTimeToUtc(anime.JSTBroadcastTime.Value, 
                TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time"));

            // get the UTC offset for the selected time zone
            var utcOffset = selectedTimeZone.GetUtcOffset(DateTime.UtcNow);

            // return Japanese broadcast time converted to selected time zone
            anime.LocalBroadcastTime = broadcastTimeInSelectedTimeZone.Add(utcOffset);
        }

        /// <summary>
        /// Parses day and time from the broadcast string, converts it to the local time zone and saves to the LocalBroadcastTime property of the MiruAnimeModel.
        /// </summary>
        /// <param name="detailedAnimeList">List of animes with broadcast strings but without parsed day and time data.</param>
        /// <returns>List of animes with parsed data saved in LocalBroadcastTime properties.</returns>
        public static void ParseTimeFromBroadcast(this List<MiruAnimeModel> detailedAnimeList, IFileSystemService fileSystemService)
        {
            var jpCultureInfo = CultureInfo.GetCultureInfo("ja-JP");
            string[] formats = { "dd/MM/yyyy HH:mm", "d/MM/yyyy HH:mm", "dd/M/yyyy HH:mm", "d/M/yyyy HH:mm"};

            // deserialize data from senpai as a backup source of anime broadcast time
            var senpaiEntries = JsonConvert.DeserializeObject<SenpaiEntryModel>(fileSystemService?.FileSystem?.File?.ReadAllText(Constants.SenpaiFilePath));
            var senpaiIDs = senpaiEntries.Items.Select(x => x.MalId).ToArray();

            // for each anime parse time and day of the week from the broadcast string
            foreach (var anime in detailedAnimeList)
            {
                anime.IsOnSenpai = senpaiIDs.Contains(anime.MalId);
                bool parsed = false;

                if (anime.IsOnSenpai)
                {
                    var airDateAndTime = senpaiEntries.Items.First(x => x.MalId == anime.MalId).Airdate;
                    parsed = TryParseAndSetAirTime(airDateAndTime, anime, formats, jpCultureInfo);
                }

                if (!parsed && !TryParseAndSetAirTime(anime.Broadcast, anime, formats, jpCultureInfo))
                {
                    TryParseAndSetAirTimeFromMyAnimeList(anime);
                }

                // save JST broadcast time converted to your computer's local time to the model's property
                anime.ConvertJstBroadcastTimeToSelectedTimeZone(TimeZoneInfo.Local);
            }
        }

        public static bool TryParseAndSetAirTime(string airTimeText, MiruAnimeModel animeToUpdate, string[] formats, CultureInfo cultureInfo)
        {
            var parsed = DateTime.TryParseExact(airTimeText, formats, cultureInfo, DateTimeStyles.None, out DateTime parsedSenpaiBroadcast);
            if (parsed)
            {
                animeToUpdate.Broadcast = airTimeText;
                animeToUpdate.JSTBroadcastTime = parsedSenpaiBroadcast;
            }
            return parsed;
        }

        /// <summary>
        /// This method parses DateTime from MAL strings that look like this: "Saturdays at 02:25 (JST)".
        /// </summary>
        /// <param name="animeToUpdate">Anime that will be updated with parsed data.</param>
        /// <returns></returns>
        public static bool TryParseAndSetAirTimeFromMyAnimeList(MiruAnimeModel animeToUpdate)
        {
            // split the broadcast string into words
            var broadcastWords = animeToUpdate.Broadcast.Split(' ');

            if (broadcastWords.Length <= 2)
            {
                return false;
            }
            // parse time from the 2nd broadcast string word
            if (!DateTime.TryParse(broadcastWords[2], out DateTime time))
            {
                return false;
            }

            // set the first word of the broadcast string as a day of the week
            var dayOfTheWeek = broadcastWords[0];
            DateTime jstAirTime;

            // depending on the 1st word set the correct day
            switch (dayOfTheWeek)
            {
                case "Mondays":
                    jstAirTime = new DateTime(2020, 01, 20, time.Hour, time.Minute, 0);
                    break;

                case "Tuesdays":
                    jstAirTime = new DateTime(2020, 01, 21, time.Hour, time.Minute, 0);
                    break;

                case "Wednesdays":
                    jstAirTime = new DateTime(2020, 01, 22, time.Hour, time.Minute, 0);
                    break;

                case "Thursdays":
                    jstAirTime = new DateTime(2020, 01, 23, time.Hour, time.Minute, 0);
                    break;

                case "Fridays":
                    jstAirTime = new DateTime(2020, 01, 24, time.Hour, time.Minute, 0);
                    break;

                case "Saturdays":
                    jstAirTime = new DateTime(2020, 01, 25, time.Hour, time.Minute, 0);
                    break;

                case "Sundays":
                    jstAirTime = new DateTime(2020, 01, 26, time.Hour, time.Minute, 0);
                    break;

                default:
                    jstAirTime = new DateTime();
                    break;
            }
            animeToUpdate.JSTBroadcastTime = jstAirTime;
            return true;
        }

        public static void SetAiringAnimeModelData(this MiruAnimeModel animeModel, Anime animeInfo, AnimeListEntry animeListEntry)
        {
            animeModel.MalId = animeInfo.MalId;
            animeModel.Broadcast = animeInfo.Broadcast ?? animeInfo?.Aired?.From.ToString();
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
