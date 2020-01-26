using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JikanDotNet;
using Caliburn.Micro;

namespace Miru.Models
{
    public class SortedAnimeListEntries : PropertyChangedBase
    {
        private ICollection<AnimeAiringTime> _mondayAiringAnimeList;
        private ICollection<AnimeAiringTime> _tuesdayAiringAnimeList;
        private ICollection<AnimeAiringTime> _wednesdayAiringAnimeList;
        private ICollection<AnimeAiringTime> _thursdayAiringAnimeList;
        private ICollection<AnimeAiringTime> _fridayAiringAnimeList;
        private ICollection<AnimeAiringTime> _saturdayAiringAnimeList;
        private ICollection<AnimeAiringTime> _sundayAiringAnimeList;
        private ICollection<AnimeListEntry> _airedAnimeList;

        public ICollection<AnimeAiringTime> MondayAiringAnimeList
        {
            get { return _mondayAiringAnimeList; }
            set 
            { 
                _mondayAiringAnimeList = value;
                NotifyOfPropertyChange(() => MondayAiringAnimeList);
            }
        }

        //public ICollection<AnimeAiringTime> MondayAiringAnimeList { get; set; }
        //public ICollection<AnimeAiringTime> TuesdayAiringAnimeList { get; set; }
        public ICollection<AnimeAiringTime> TuesdayAiringAnimeList
        {
            get { return _tuesdayAiringAnimeList; }
            set { _tuesdayAiringAnimeList = value; NotifyOfPropertyChange(() => TuesdayAiringAnimeList); }
        }
        public ICollection<AnimeAiringTime> WednesdayAiringAnimeList
        {
            get { return _wednesdayAiringAnimeList; }
            set { _wednesdayAiringAnimeList = value; NotifyOfPropertyChange(() => WednesdayAiringAnimeList); }
        }
        public ICollection<AnimeAiringTime> ThursdayAiringAnimeList
        {
            get { return _thursdayAiringAnimeList; }
            set { _thursdayAiringAnimeList = value; NotifyOfPropertyChange(() => ThursdayAiringAnimeList); }
        }
        public ICollection<AnimeAiringTime> FridayAiringAnimeList
        {
            get { return _fridayAiringAnimeList; }
            set { _fridayAiringAnimeList = value; NotifyOfPropertyChange(() => FridayAiringAnimeList); }
        }
        public ICollection<AnimeAiringTime> SaturdayAiringAnimeList
        {
            get { return _saturdayAiringAnimeList; }
            set { _saturdayAiringAnimeList = value; NotifyOfPropertyChange(() => SaturdayAiringAnimeList); }
        }
        public ICollection<AnimeAiringTime> SundayAiringAnimeList
        {
            get { return _sundayAiringAnimeList; }
            set { _sundayAiringAnimeList = value; NotifyOfPropertyChange(() => SundayAiringAnimeList); }
        }
        public ICollection<AnimeListEntry> AiredAnimeList
        {
            get { return _airedAnimeList; }
            set { _airedAnimeList = value; NotifyOfPropertyChange(() => AiredAnimeList); }
        }
            //public ICollection<AnimeAiringTime> WednesdayAiringAnimeList { get; set; }
            //public ICollection<AnimeAiringTime> ThursdayAiringAnimeList { get; set; }
            //public ICollection<AnimeAiringTime> FridayAiringAnimeList { get; set; }
            //public ICollection<AnimeAiringTime> SaturdayAiringAnimeList { get; set; }
            //public ICollection<AnimeAiringTime> SundayAiringAnimeList { get; set; }
            //public ICollection<AnimeAiringTime> AiredAnimeList { get; set; }
        }
}
