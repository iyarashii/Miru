﻿using JikanDotNet;
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

        void ChangeDisplayedAnimeList(AnimeListType animeListType, TimeZoneInfo selectedTimeZone, AnimeType selectedAnimeType, string animeNameFilter);
        void ClearDb();
        Task<bool> GetDetailedSeasonAnimeListInfo(List<MiruAnimeModel> detailedUserAnimeList);
        Task<List<MiruAnimeModel>> GetDetailedUserAnimeList(IMiruDbContext db, ICollection<AnimeListEntry> currentUserAnimeListEntries);
        void LoadLastSyncedData();
        List<MiruAnimeModel> ParseTimeFromBroadcast(List<MiruAnimeModel> detailedAnimeList);
        Task<bool> SaveDetailedAnimeListData(bool seasonSyncOn);
        Task SaveSyncedUserData(string typedInUsername);
        Task<Anime> TryToGetAnimeInfo(long malId, int millisecondsDelay);
    }
}