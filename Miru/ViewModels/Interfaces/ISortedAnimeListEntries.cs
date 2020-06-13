using JikanDotNet;
using MiruLibrary.Models;
using System.Collections.Generic;
using MiruLibrary;

namespace Miru.ViewModels
{
    public interface ISortedAnimeListEntries
    {
        IEnumerable<MiruAnimeModel> FridayAiringAnimeList { get; set; }
        IEnumerable<MiruAnimeModel> MondayAiringAnimeList { get; set; }
        IEnumerable<MiruAnimeModel> SaturdayAiringAnimeList { get; set; }
        IEnumerable<MiruAnimeModel> SundayAiringAnimeList { get; set; }
        IEnumerable<MiruAnimeModel> ThursdayAiringAnimeList { get; set; }
        IEnumerable<MiruAnimeModel> TuesdayAiringAnimeList { get; set; }
        IEnumerable<MiruAnimeModel> WednesdayAiringAnimeList { get; set; }
        void SetAnimeSortedByAirDayOfWeekAndFilteredByGivenAnimeListType(IEnumerable<MiruAnimeModel> animeModels, AnimeListType animeListType);
    }
}