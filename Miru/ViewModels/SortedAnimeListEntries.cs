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
        private ICollection<MiruAiringAnimeModel> _mondayAiringAnimeList;
        private ICollection<MiruAiringAnimeModel> _tuesdayAiringAnimeList;
        private ICollection<MiruAiringAnimeModel> _wednesdayAiringAnimeList;
        private ICollection<MiruAiringAnimeModel> _thursdayAiringAnimeList;
        private ICollection<MiruAiringAnimeModel> _fridayAiringAnimeList;
        private ICollection<MiruAiringAnimeModel> _saturdayAiringAnimeList;
        private ICollection<MiruAiringAnimeModel> _sundayAiringAnimeList;

        public ICollection<MiruAiringAnimeModel> MondayAiringAnimeList
        {
            get { return _mondayAiringAnimeList; }
            set
            {
                _mondayAiringAnimeList = value;
                NotifyOfPropertyChange(() => MondayAiringAnimeList);
            }
        }

        public ICollection<MiruAiringAnimeModel> TuesdayAiringAnimeList
        {
            get { return _tuesdayAiringAnimeList; }
            set { _tuesdayAiringAnimeList = value; NotifyOfPropertyChange(() => TuesdayAiringAnimeList); }
        }

        public ICollection<MiruAiringAnimeModel> WednesdayAiringAnimeList
        {
            get { return _wednesdayAiringAnimeList; }
            set { _wednesdayAiringAnimeList = value; NotifyOfPropertyChange(() => WednesdayAiringAnimeList); }
        }

        public ICollection<MiruAiringAnimeModel> ThursdayAiringAnimeList
        {
            get { return _thursdayAiringAnimeList; }
            set { _thursdayAiringAnimeList = value; NotifyOfPropertyChange(() => ThursdayAiringAnimeList); }
        }

        public ICollection<MiruAiringAnimeModel> FridayAiringAnimeList
        {
            get { return _fridayAiringAnimeList; }
            set { _fridayAiringAnimeList = value; NotifyOfPropertyChange(() => FridayAiringAnimeList); }
        }

        public ICollection<MiruAiringAnimeModel> SaturdayAiringAnimeList
        {
            get { return _saturdayAiringAnimeList; }
            set { _saturdayAiringAnimeList = value; NotifyOfPropertyChange(() => SaturdayAiringAnimeList); }
        }

        public ICollection<MiruAiringAnimeModel> SundayAiringAnimeList
        {
            get { return _sundayAiringAnimeList; }
            set { _sundayAiringAnimeList = value; NotifyOfPropertyChange(() => SundayAiringAnimeList); }
        }
        //TODO: remove this 2nd method of local image cache clearing
        //private bool _noAnimesDisplayed;
        //public bool NoAnimesDisplayed
        //{
        //    get { return _noAnimesDisplayed; }
        //    set 
        //    { 
        //        _noAnimesDisplayed = value;  
        //        NotifyOfPropertyChange(() => NoAnimesDisplayed); 
        //    }
        //}


        // orders the airing anime list entries by the days
        public void SortAiringAnime(List<MiruAiringAnimeModel> airingAnimeModels, AnimeListType animeListType)
        {
            //TODO: remove this 2nd method of local image cache clearing
            //if(airingAnimeModels.Count != 0)
            //{
            //    NoAnimesDisplayed = false;
            //}

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
            MondayAiringAnimeList = airingAnimeModels.Where(a => a.LocalBroadcastTime.Value.DayOfWeek == DayOfWeek.Monday).OrderBy(s => s.LocalBroadcastTime.Value.TimeOfDay).ToList();
            TuesdayAiringAnimeList = airingAnimeModels.Where(a => a.LocalBroadcastTime.Value.DayOfWeek == DayOfWeek.Tuesday).OrderBy(s => s.LocalBroadcastTime.Value.TimeOfDay).ToList();
            WednesdayAiringAnimeList = airingAnimeModels.Where(a => a.LocalBroadcastTime.Value.DayOfWeek == DayOfWeek.Wednesday).OrderBy(s => s.LocalBroadcastTime.Value.TimeOfDay).ToList();
            ThursdayAiringAnimeList = airingAnimeModels.Where(a => a.LocalBroadcastTime.Value.DayOfWeek == DayOfWeek.Thursday).OrderBy(s => s.LocalBroadcastTime.Value.TimeOfDay).ToList();
            FridayAiringAnimeList = airingAnimeModels.Where(a => a.LocalBroadcastTime.Value.DayOfWeek == DayOfWeek.Friday).OrderBy(s => s.LocalBroadcastTime.Value.TimeOfDay).ToList();
            SaturdayAiringAnimeList = airingAnimeModels.Where(a => a.LocalBroadcastTime.Value.DayOfWeek == DayOfWeek.Saturday).OrderBy(s => s.LocalBroadcastTime.Value.TimeOfDay).ToList();
            SundayAiringAnimeList = airingAnimeModels.Where(a => a.LocalBroadcastTime.Value.DayOfWeek == DayOfWeek.Sunday).OrderBy(s => s.LocalBroadcastTime.Value.TimeOfDay).ToList();
        }
        //TODO: remove this 2nd method of local image cache clearing
        //public void HideAnimes()
        //{
        //    MondayAiringAnimeList = null;
        //    TuesdayAiringAnimeList = null;
        //    WednesdayAiringAnimeList = null;
        //    ThursdayAiringAnimeList = null;
        //    FridayAiringAnimeList = null;
        //    SaturdayAiringAnimeList = null;
        //    SundayAiringAnimeList = null;
        //    NoAnimesDisplayed = true;
        //}
    }
}