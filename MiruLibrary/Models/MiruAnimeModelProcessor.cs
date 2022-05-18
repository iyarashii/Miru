// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using System;
using System.Collections.Generic;
using System.Linq;

namespace MiruLibrary.Models
{
    public class MiruAnimeModelProcessor : IMiruAnimeModelProcessor
    {
        // returns a list of anime models which belong to the specified anime list type
        public IEnumerable<MiruAnimeModel> FilterAnimeModelsByAnimeListType(IEnumerable<MiruAnimeModel> animeModels, AnimeListType animeListType)
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

                case AnimeListType.Senpai:
                    animeModels = animeModels.Where(a => a.IsOnSenpai).ToList();
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
        public IEnumerable<MiruAnimeModel> FilterAnimeModelsByAirDayOfWeekAndOrderByAirTime(IEnumerable<MiruAnimeModel> animeModels, DayOfWeek dayOfWeek)
        {
            return animeModels.Where(a => a.LocalBroadcastTime.Value.DayOfWeek == dayOfWeek).OrderBy(s => s.LocalBroadcastTime.Value.TimeOfDay);
        }
    }
}