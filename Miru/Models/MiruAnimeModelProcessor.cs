using System;
using System.Collections.Generic;
using System.Linq;

namespace Miru.Models
{
    public class MiruAnimeModelProcessor
    {
        // returns a list of anime models which belong to the specified anime list type
        public static IEnumerable<MiruAnimeModel> FilterAnimeModelsByAnimeListType(IEnumerable<MiruAnimeModel> animeModels, AnimeListType animeListType)
        {
            switch (animeListType)
            {
                case AnimeListType.AiringAndWatching:
                    animeModels = animeModels.Where(a => a.IsOnWatchingList && a.CurrentlyAiring).ToList();
                    break;

                case AnimeListType.Watching:
                    animeModels = animeModels.Where(a => a.IsOnWatchingList).ToList();
                    break;

                case AnimeListType.Season:
                    animeModels = animeModels.Where(a => a.CurrentlyAiring).ToList();
                    break;
            }
            return animeModels;
        }

        /// <summary>
        /// Returns list of anime models filtered by the specific day of week and ordered by air time.
        /// </summary>
        /// <param name="animeModels">Anime models that will be filtered</param>
        /// <param name="dayOfWeek">Specific day of the week on which filtered animes air</param>
        /// <returns>
        /// List of anime models filtered by the specific day of week and ordered by air time.
        /// </returns>
        public static IEnumerable<MiruAnimeModel> FilterAnimeModelsByAirDayOfWeekAndOrderByAirTime(IEnumerable<MiruAnimeModel> animeModels, DayOfWeek dayOfWeek)
        {
            return animeModels.Where(a => a.LocalBroadcastTime.Value.DayOfWeek == dayOfWeek).OrderBy(s => s.LocalBroadcastTime.Value.TimeOfDay).ToList();
        }
    }
}