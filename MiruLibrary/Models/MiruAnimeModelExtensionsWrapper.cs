using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiruLibrary.Models
{
    public class MiruAnimeModelExtensionsWrapper : IMiruAnimeModelExtensionsWrapper
    {
        public void FilterByTitle(List<MiruAnimeModel> animeList, string title)
        {
            animeList.FilterByTitle(title);
        }
        
        public void FilterByBroadcastType(List<MiruAnimeModel> animeList, AnimeType broadcastType)
        {
            animeList.FilterByBroadcastType(broadcastType);
        }

        public void ConvertJstBroadcastTimeToSelectedTimeZone(MiruAnimeModel anime, TimeZoneInfo selectedTimeZone)
        {
            anime.ConvertJstBroadcastTimeToSelectedTimeZone(selectedTimeZone);
        }
    }
}
