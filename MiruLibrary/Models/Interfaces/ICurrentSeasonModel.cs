// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using JikanDotNet;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiruLibrary.Models
{
    public interface ICurrentSeasonModel
    {
        Season SeasonData { get; }

        Task<bool> GetCurrentSeasonList(int requestRetryDelayInMs);
        List<AnimeSubEntry> GetFilteredSeasonList();
    }
}