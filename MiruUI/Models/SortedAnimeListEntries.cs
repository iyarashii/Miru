using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JikanDotNet;

namespace Miru.Models
{
    public class SortedAnimeListEntries
    {
        public ICollection<AnimeListEntry> MondayAiringAnimeList { get; set; }
        public ICollection<AnimeListEntry> TuesdayAiringAnimeList { get; set; }
        public ICollection<AnimeListEntry> WednesdayAiringAnimeList { get; set; }
        public ICollection<AnimeListEntry> ThursdayAiringAnimeList { get; set; }
        public ICollection<AnimeListEntry> FridayAiringAnimeList { get; set; }
        public ICollection<AnimeListEntry> SaturdayAiringAnimeList { get; set; }
        public ICollection<AnimeListEntry> SundayAiringAnimeList { get; set; }
        public ICollection<AnimeListEntry> AiredAnimeList { get; set; }
    }
}
