using JikanDotNet;
using Miru.Models;
using System.Collections.Generic;

namespace Miru.ViewModels
{
    public interface ISortedAnimeListEntries
    {
        ICollection<MiruAiringAnimeModel> FridayAiringAnimeList { get; set; }
        ICollection<MiruAiringAnimeModel> MondayAiringAnimeList { get; set; }
        ICollection<MiruAiringAnimeModel> SaturdayAiringAnimeList { get; set; }
        ICollection<MiruAiringAnimeModel> SundayAiringAnimeList { get; set; }
        ICollection<MiruAiringAnimeModel> ThursdayAiringAnimeList { get; set; }
        ICollection<MiruAiringAnimeModel> TuesdayAiringAnimeList { get; set; }
        ICollection<MiruAiringAnimeModel> WednesdayAiringAnimeList { get; set; }

        void SortAiringAnime(List<MiruAiringAnimeModel> airingAnimeModels, AnimeListType animeListType);
    }
}