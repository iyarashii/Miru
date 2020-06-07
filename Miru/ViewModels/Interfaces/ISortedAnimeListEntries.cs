using JikanDotNet;
using Miru.Models;
using System.Collections.Generic;

namespace Miru.ViewModels
{
    public interface ISortedAnimeListEntries
    {
        ICollection<MiruAnimeModel> FridayAiringAnimeList { get; set; }
        ICollection<MiruAnimeModel> MondayAiringAnimeList { get; set; }
        ICollection<MiruAnimeModel> SaturdayAiringAnimeList { get; set; }
        ICollection<MiruAnimeModel> SundayAiringAnimeList { get; set; }
        ICollection<MiruAnimeModel> ThursdayAiringAnimeList { get; set; }
        ICollection<MiruAnimeModel> TuesdayAiringAnimeList { get; set; }
        ICollection<MiruAnimeModel> WednesdayAiringAnimeList { get; set; }
        void SortAnimeByAirDayOfWeek(List<MiruAnimeModel> airingAnimeModels, AnimeListType animeListType);
    }
}