using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiruLibrary.Models
{
    public class UserSettings
    {
        public double AnimeImageSize { get; set; } = 134.0;
        public AnimeListType DisplayedAnimeListType { get; set; } = AnimeListType.Watching;
        public AnimeType DisplayedAnimeType { get; set; } = AnimeType.TV;
    }
}
