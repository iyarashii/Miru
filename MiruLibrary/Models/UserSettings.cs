﻿// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

namespace MiruLibrary.Models
{
    public class UserSettings
    {
        public double AnimeImageSize { get; set; } = 134.0;
        public AnimeListType DisplayedAnimeListType { get; set; } = AnimeListType.Watching;
        public AnimeType DisplayedAnimeType { get; set; } = AnimeType.TV;
        public bool GetDroppedAnimeData { get; set; } = false;
        public double WatchingStatusHighlightOpacity { get; set; } = 0.66;
    }
}
