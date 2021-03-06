﻿using JikanDotNet;
using MiruLibrary.Models;
using System.Collections.Generic;
using MiruLibrary;

namespace Miru.ViewModels
{
    public interface ISortedAnimeListsViewModel
    {
        IEnumerable<MiruAnimeModel> FridayAiringAnimeList { get; set; }
        IEnumerable<MiruAnimeModel> MondayAiringAnimeList { get; set; }
        IEnumerable<MiruAnimeModel> SaturdayAiringAnimeList { get; set; }
        IEnumerable<MiruAnimeModel> SundayAiringAnimeList { get; set; }
        IEnumerable<MiruAnimeModel> ThursdayAiringAnimeList { get; set; }
        IEnumerable<MiruAnimeModel> TuesdayAiringAnimeList { get; set; }
        IEnumerable<MiruAnimeModel> WednesdayAiringAnimeList { get; set; }
        IMiruAnimeModelProcessor MiruAnimeModelProcessor { get; }

        void SetAnimeSortedByAirDayOfWeekAndFilteredByGivenAnimeListType(IEnumerable<MiruAnimeModel> animeModels, AnimeListType animeListType);
    }
}