using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miru.Models
{
    public class MiruAnimeModelProcessor
    {
        // returns a list of anime models which belong to the specified anime list type
        //public List<MiruAnimeModel> FilterAnimeModelsByAnimeListType(List<MiruAnimeModel> airingAnimeModels, AnimeListType animeListType)
        //{
        //    switch (animeListType)
        //    {
        //        case AnimeListType.AiringAndWatching:
        //            airingAnimeModels = airingAnimeModels.Where(a => a.IsOnWatchingList && a.CurrentlyAiring).ToList();
        //            break;

        //        case AnimeListType.Watching:
        //            airingAnimeModels = airingAnimeModels.Where(a => a.IsOnWatchingList).ToList();
        //            break;

        //        case AnimeListType.Season:
        //            airingAnimeModels = airingAnimeModels.Where(a => a.CurrentlyAiring).ToList();
        //            break;
        //    }
        //    return airingAnimeModels;
        //}

        //public void SplitAnimeModelsByDayOfWeek()
        //{
        //    MondayAiringAnimeList = animeModels.Where(a => a.LocalBroadcastTime.Value.DayOfWeek == DayOfWeek.Monday).OrderBy(s => s.LocalBroadcastTime.Value.TimeOfDay).ToList();
        //    TuesdayAiringAnimeList = animeModels.Where(a => a.LocalBroadcastTime.Value.DayOfWeek == DayOfWeek.Tuesday).OrderBy(s => s.LocalBroadcastTime.Value.TimeOfDay).ToList();
        //    WednesdayAiringAnimeList = animeModels.Where(a => a.LocalBroadcastTime.Value.DayOfWeek == DayOfWeek.Wednesday).OrderBy(s => s.LocalBroadcastTime.Value.TimeOfDay).ToList();
        //    ThursdayAiringAnimeList = animeModels.Where(a => a.LocalBroadcastTime.Value.DayOfWeek == DayOfWeek.Thursday).OrderBy(s => s.LocalBroadcastTime.Value.TimeOfDay).ToList();
        //    FridayAiringAnimeList = animeModels.Where(a => a.LocalBroadcastTime.Value.DayOfWeek == DayOfWeek.Friday).OrderBy(s => s.LocalBroadcastTime.Value.TimeOfDay).ToList();
        //    SaturdayAiringAnimeList = animeModels.Where(a => a.LocalBroadcastTime.Value.DayOfWeek == DayOfWeek.Saturday).OrderBy(s => s.LocalBroadcastTime.Value.TimeOfDay).ToList();
        //    SundayAiringAnimeList = animeModels.Where(a => a.LocalBroadcastTime.Value.DayOfWeek == DayOfWeek.Sunday).OrderBy(s => s.LocalBroadcastTime.Value.TimeOfDay).ToList();
        //}
    }
}
