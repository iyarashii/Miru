using System;
using System.Collections.Generic;

namespace MiruLibrary.Models
{
    public interface IMiruAnimeModelExtensionsWrapper
    {
        void ConvertJstBroadcastTimeToSelectedTimeZone(MiruAnimeModel anime, TimeZoneInfo selectedTimeZone);
        void FilterByBroadcastType(List<MiruAnimeModel> animeList, AnimeType broadcastType);
        void FilterByTitle(List<MiruAnimeModel> animeList, string title);
    }
}