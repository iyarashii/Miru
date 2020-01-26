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
        private ICollection<AnimeListEntry> mondayAiringAnimeList;

        public ICollection<AnimeListEntry> MondayAiringAnimeList
        {
            get { return mondayAiringAnimeList; }
            set 
            { 
                mondayAiringAnimeList = value;
                NotifyOfPropertyChange(() => MondayAiringAnimeList);
            }
        }

        //public ICollection<AnimeListEntry> MondayAiringAnimeList { get; set; }
        public ICollection<AnimeListEntry> TuesdayAiringAnimeList { get; set; }
        public ICollection<AnimeListEntry> WednesdayAiringAnimeList { get; set; }
        public ICollection<AnimeListEntry> ThursdayAiringAnimeList { get; set; }
        public ICollection<AnimeListEntry> FridayAiringAnimeList { get; set; }
        public ICollection<AnimeListEntry> SaturdayAiringAnimeList { get; set; }
        public ICollection<AnimeListEntry> SundayAiringAnimeList { get; set; }
        public ICollection<AnimeListEntry> AiredAnimeList { get; set; }
    }
}
