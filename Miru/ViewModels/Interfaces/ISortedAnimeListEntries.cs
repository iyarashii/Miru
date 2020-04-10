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
        //TODO: remove this 2nd method of local image cache clearing
        //bool NoAnimesDisplayed { get; set; }
        //void HideAnimes();
        void SortAiringAnime(List<MiruAiringAnimeModel> airingAnimeModels, AnimeListType animeListType);
    }
}