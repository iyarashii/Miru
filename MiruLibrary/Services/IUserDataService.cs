// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using MiruLibrary.Models;

namespace MiruLibrary.Services
{
    public interface IUserDataService
    {
        ICurrentSeasonModel CurrentSeasonModel { get; }
        ICurrentUserAnimeListModel CurrentUserAnimeListModel { get; }
        ISyncedMyAnimeListUser SyncedMyAnimeListUser { get; }
    }
}