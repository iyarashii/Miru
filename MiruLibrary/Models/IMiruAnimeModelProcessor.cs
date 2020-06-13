using System;
using System.Collections.Generic;

namespace MiruLibrary.Models
{
    public interface IMiruAnimeModelProcessor
    {
        IEnumerable<MiruAnimeModel> FilterAnimeModelsByAirDayOfWeekAndOrderByAirTime(IEnumerable<MiruAnimeModel> animeModels, DayOfWeek dayOfWeek);
        IEnumerable<MiruAnimeModel> FilterAnimeModelsByAnimeListType(IEnumerable<MiruAnimeModel> animeModels, AnimeListType animeListType);
    }
}