using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
