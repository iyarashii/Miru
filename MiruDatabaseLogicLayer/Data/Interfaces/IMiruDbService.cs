// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using JikanDotNet;
using MiruLibrary.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MiruLibrary;
using AnimeType = MiruLibrary.AnimeType;

namespace MiruDatabaseLogicLayer
{
    public interface IMiruDbService
    {
        ICurrentSeasonModel CurrentSeason { get; }
        ICurrentUserAnimeListModel CurrentUserAnimeList { get; }
        string CurrentUsername { get; }

        event EventHandler<DateTime> UpdateSyncDate;
        event MiruDbService.SortedAnimeListEventHandler UpdateAnimeListEntriesUI;
        event EventHandler<string> UpdateCurrentUsername;
        event MiruDbService.UpdateAppStatusEventHandler UpdateAppStatusUI;
        event EventHandler<int> UpdateSyncProgress;

        void ChangeDisplayedAnimeList(AnimeListType animeListType, TimeZoneInfo selectedTimeZone, AnimeType selectedAnimeBroadcastType, string animeTitleToFilterBy);
        void ClearDb();
        Task<List<MiruAnimeModel>> GetDetailedUserAnimeList(IMiruDbContext db, ICollection<AnimeListEntry> currentUserAnimeListEntries, bool seasonSyncOn);
        void LoadLastSyncedData();
        Task<bool> SaveDetailedAnimeListData(bool seasonSyncOn);
        Task SaveSyncedUserData(string typedInUsername);
        List<MiruAnimeModel> GetFilteredUserAnimeList(IMiruDbContext db, AnimeType selectedBroadcastType, string title, TimeZoneInfo selectedTimeZone);

        DateTime SyncDateData { get; set; }
    }
}