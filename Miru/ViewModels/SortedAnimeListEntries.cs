using Caliburn.Micro;
using JikanDotNet;
using MiruLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using MiruLibrary;

namespace Miru.ViewModels
{
    // wires up sorted anime list data to the correct properties that are used by the view
    public class SortedAnimeListEntries : PropertyChangedBase, ISortedAnimeListEntries
    {
        public SortedAnimeListEntries(IMiruAnimeModelProcessor miruAnimeModelProcessor)
        {
            MiruAnimeModelProcessor = miruAnimeModelProcessor;
        }

        public IMiruAnimeModelProcessor MiruAnimeModelProcessor { get; }

        private IEnumerable<MiruAnimeModel> _mondayAiringAnimeList;
        private IEnumerable<MiruAnimeModel> _tuesdayAiringAnimeList;
        private IEnumerable<MiruAnimeModel> _wednesdayAiringAnimeList;
        private IEnumerable<MiruAnimeModel> _thursdayAiringAnimeList;
        private IEnumerable<MiruAnimeModel> _fridayAiringAnimeList;
        private IEnumerable<MiruAnimeModel> _saturdayAiringAnimeList;
        private IEnumerable<MiruAnimeModel> _sundayAiringAnimeList;

        public IEnumerable<MiruAnimeModel> MondayAiringAnimeList
        {
            get { return _mondayAiringAnimeList; }
            set
            {
                _mondayAiringAnimeList = value;
                NotifyOfPropertyChange(() => MondayAiringAnimeList);
            }
        }

        public IEnumerable<MiruAnimeModel> TuesdayAiringAnimeList
        {
            get { return _tuesdayAiringAnimeList; }
            set { _tuesdayAiringAnimeList = value; NotifyOfPropertyChange(() => TuesdayAiringAnimeList); }
        }

        public IEnumerable<MiruAnimeModel> WednesdayAiringAnimeList
        {
            get { return _wednesdayAiringAnimeList; }
            set { _wednesdayAiringAnimeList = value; NotifyOfPropertyChange(() => WednesdayAiringAnimeList); }
        }

        public IEnumerable<MiruAnimeModel> ThursdayAiringAnimeList
        {
            get { return _thursdayAiringAnimeList; }
            set { _thursdayAiringAnimeList = value; NotifyOfPropertyChange(() => ThursdayAiringAnimeList); }
        }

        public IEnumerable<MiruAnimeModel> FridayAiringAnimeList
        {
            get { return _fridayAiringAnimeList; }
            set { _fridayAiringAnimeList = value; NotifyOfPropertyChange(() => FridayAiringAnimeList); }
        }

        public IEnumerable<MiruAnimeModel> SaturdayAiringAnimeList
        {
            get { return _saturdayAiringAnimeList; }
            set { _saturdayAiringAnimeList = value; NotifyOfPropertyChange(() => SaturdayAiringAnimeList); }
        }

        public IEnumerable<MiruAnimeModel> SundayAiringAnimeList
        {
            get { return _sundayAiringAnimeList; }
            set { _sundayAiringAnimeList = value; NotifyOfPropertyChange(() => SundayAiringAnimeList); }
        }

        /// <summary>
        /// Assigns animes to the correct day of week airing list properties. 
        /// </summary>
        /// <param name="animeModels"></param>
        /// <param name="animeListType"></param>
        public void SetAnimeSortedByAirDayOfWeekAndFilteredByGivenAnimeListType(IEnumerable<MiruAnimeModel> animeModels, AnimeListType animeListType)
        {
            animeModels = MiruAnimeModelProcessor.FilterAnimeModelsByAnimeListType(animeModels, animeListType);

            MondayAiringAnimeList = MiruAnimeModelProcessor.FilterAnimeModelsByAirDayOfWeekAndOrderByAirTime(animeModels, DayOfWeek.Monday);
            TuesdayAiringAnimeList = MiruAnimeModelProcessor.FilterAnimeModelsByAirDayOfWeekAndOrderByAirTime(animeModels, DayOfWeek.Tuesday);
            WednesdayAiringAnimeList = MiruAnimeModelProcessor.FilterAnimeModelsByAirDayOfWeekAndOrderByAirTime(animeModels, DayOfWeek.Wednesday);
            ThursdayAiringAnimeList = MiruAnimeModelProcessor.FilterAnimeModelsByAirDayOfWeekAndOrderByAirTime(animeModels, DayOfWeek.Thursday);
            FridayAiringAnimeList = MiruAnimeModelProcessor.FilterAnimeModelsByAirDayOfWeekAndOrderByAirTime(animeModels, DayOfWeek.Friday);
            SaturdayAiringAnimeList = MiruAnimeModelProcessor.FilterAnimeModelsByAirDayOfWeekAndOrderByAirTime(animeModels, DayOfWeek.Saturday);
            SundayAiringAnimeList = MiruAnimeModelProcessor.FilterAnimeModelsByAirDayOfWeekAndOrderByAirTime(animeModels, DayOfWeek.Sunday);
        }
    }
}