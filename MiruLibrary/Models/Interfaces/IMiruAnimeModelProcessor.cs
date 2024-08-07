﻿// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using System;
using System.Collections.Generic;

namespace MiruLibrary.Models
{
    public interface IMiruAnimeModelProcessor
    {
        IEnumerable<MiruAnimeModel> FilterAnimeModelsByAirDayOfWeekAndOrderByAirTime(IEnumerable<MiruAnimeModel> animeModels, DayOfWeek? dayOfWeek);
        IEnumerable<MiruAnimeModel> FilterAnimeModelsByAnimeListType(IEnumerable<MiruAnimeModel> animeModels, AnimeListType animeListType);
    }
}