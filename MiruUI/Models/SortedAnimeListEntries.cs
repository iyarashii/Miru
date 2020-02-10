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
        private ICollection<MiruAiringAnimeModel> _mondayAiringAnimeList;
        private ICollection<MiruAiringAnimeModel> _tuesdayAiringAnimeList;
        private ICollection<MiruAiringAnimeModel> _wednesdayAiringAnimeList;
        private ICollection<MiruAiringAnimeModel> _thursdayAiringAnimeList;
        private ICollection<MiruAiringAnimeModel> _fridayAiringAnimeList;
        private ICollection<MiruAiringAnimeModel> _saturdayAiringAnimeList;
        private ICollection<MiruAiringAnimeModel> _sundayAiringAnimeList;
        private ICollection<AnimeListEntry> _airedAnimeList;

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
        public ICollection<AnimeListEntry> AiredAnimeList
        {
            get { return _airedAnimeList; }
            set { _airedAnimeList = value; NotifyOfPropertyChange(() => AiredAnimeList); }
        }
    }
}
