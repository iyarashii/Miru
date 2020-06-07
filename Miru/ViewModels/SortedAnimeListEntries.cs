using Caliburn.Micro;
using JikanDotNet;
using Miru.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Miru.ViewModels
{
    // wires up sorted anime list data to the correct properties that are used by the view
    public class SortedAnimeListEntries : PropertyChangedBase, ISortedAnimeListEntries
    {
        private ICollection<MiruAnimeModel> _mondayAiringAnimeList;
        private ICollection<MiruAnimeModel> _tuesdayAiringAnimeList;
        private ICollection<MiruAnimeModel> _wednesdayAiringAnimeList;
        private ICollection<MiruAnimeModel> _thursdayAiringAnimeList;
        private ICollection<MiruAnimeModel> _fridayAiringAnimeList;
        private ICollection<MiruAnimeModel> _saturdayAiringAnimeList;
        private ICollection<MiruAnimeModel> _sundayAiringAnimeList;

        public ICollection<MiruAnimeModel> MondayAiringAnimeList
        {
            get { return _mondayAiringAnimeList; }
            set
            {
                _mondayAiringAnimeList = value;
                NotifyOfPropertyChange(() => MondayAiringAnimeList);
            }
        }

        public ICollection<MiruAnimeModel> TuesdayAiringAnimeList
        {
            get { return _tuesdayAiringAnimeList; }
            set { _tuesdayAiringAnimeList = value; NotifyOfPropertyChange(() => TuesdayAiringAnimeList); }
        }

        public ICollection<MiruAnimeModel> WednesdayAiringAnimeList
        {
            get { return _wednesdayAiringAnimeList; }
            set { _wednesdayAiringAnimeList = value; NotifyOfPropertyChange(() => WednesdayAiringAnimeList); }
        }

        public ICollection<MiruAnimeModel> ThursdayAiringAnimeList
        {
            get { return _thursdayAiringAnimeList; }
            set { _thursdayAiringAnimeList = value; NotifyOfPropertyChange(() => ThursdayAiringAnimeList); }
        }

        public ICollection<MiruAnimeModel> FridayAiringAnimeList
        {
            get { return _fridayAiringAnimeList; }
            set { _fridayAiringAnimeList = value; NotifyOfPropertyChange(() => FridayAiringAnimeList); }
        }

        public ICollection<MiruAnimeModel> SaturdayAiringAnimeList
        {
            get { return _saturdayAiringAnimeList; }
            set { _saturdayAiringAnimeList = value; NotifyOfPropertyChange(() => SaturdayAiringAnimeList); }
        }

        public ICollection<MiruAnimeModel> SundayAiringAnimeList
        {
            get { return _sundayAiringAnimeList; }
            set { _sundayAiringAnimeList = value; NotifyOfPropertyChange(() => SundayAiringAnimeList); }
        }

        // orders the airing anime list entries by the days
        public void SortAnimeByAirDayOfWeek(List<MiruAnimeModel> animeModels, AnimeListType animeListType)
        {
            animeModels = FilterAnimeModelsByAnimeListType(animeModels, animeListType);

            MondayAiringAnimeList = animeModels.Where(a => a.LocalBroadcastTime.Value.DayOfWeek == DayOfWeek.Monday).OrderBy(s => s.LocalBroadcastTime.Value.TimeOfDay).ToList();
            TuesdayAiringAnimeList = animeModels.Where(a => a.LocalBroadcastTime.Value.DayOfWeek == DayOfWeek.Tuesday).OrderBy(s => s.LocalBroadcastTime.Value.TimeOfDay).ToList();
            WednesdayAiringAnimeList = animeModels.Where(a => a.LocalBroadcastTime.Value.DayOfWeek == DayOfWeek.Wednesday).OrderBy(s => s.LocalBroadcastTime.Value.TimeOfDay).ToList();
            ThursdayAiringAnimeList = animeModels.Where(a => a.LocalBroadcastTime.Value.DayOfWeek == DayOfWeek.Thursday).OrderBy(s => s.LocalBroadcastTime.Value.TimeOfDay).ToList();
            FridayAiringAnimeList = animeModels.Where(a => a.LocalBroadcastTime.Value.DayOfWeek == DayOfWeek.Friday).OrderBy(s => s.LocalBroadcastTime.Value.TimeOfDay).ToList();
            SaturdayAiringAnimeList = animeModels.Where(a => a.LocalBroadcastTime.Value.DayOfWeek == DayOfWeek.Saturday).OrderBy(s => s.LocalBroadcastTime.Value.TimeOfDay).ToList();
            SundayAiringAnimeList = animeModels.Where(a => a.LocalBroadcastTime.Value.DayOfWeek == DayOfWeek.Sunday).OrderBy(s => s.LocalBroadcastTime.Value.TimeOfDay).ToList();
        }

        // returns a list of anime models which belong to the specified anime list type
        public List<MiruAnimeModel> FilterAnimeModelsByAnimeListType(List<MiruAnimeModel> airingAnimeModels, AnimeListType animeListType)
        {
            switch (animeListType)
            {
                case AnimeListType.AiringAndWatching:
                    airingAnimeModels = airingAnimeModels.Where(a => a.IsOnWatchingList && a.CurrentlyAiring).ToList();
                    break;

                case AnimeListType.Watching:
                    airingAnimeModels = airingAnimeModels.Where(a => a.IsOnWatchingList).ToList();
                    break;

                case AnimeListType.Season:
                    airingAnimeModels = airingAnimeModels.Where(a => a.CurrentlyAiring).ToList();
                    break;
            }
            return airingAnimeModels;
        }
    }
}