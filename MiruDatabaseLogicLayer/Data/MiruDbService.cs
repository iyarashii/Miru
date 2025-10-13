// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using JikanDotNet;
using MiruLibrary;
using MiruLibrary.Models;
using MiruLibrary.Services;
using MyInternetConnectionLibrary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AnimeType = MiruLibrary.AnimeType;
using AgeRating = MiruLibrary.AgeRating;

namespace MiruDatabaseLogicLayer
{
    // contains the business logic that uses the local db
    public class MiruDbService : IMiruDbService
    {
        private DateTime _syncDateData;
        private string currentUsername;
        private List<MiruAnimeModel> _cachedAnimeList;
        private AnimeType _lastBroadcastType = AnimeType.Any;
        private AgeRating _lastAgeRating = AgeRating.Any;
        private TimeZoneInfo _lastTimeZone = null;
        private List<MiruAnimeModel> _filteredByTypeAndAge = null;
        private List<MiruAnimeModel> _timeZoneConvertedList = null;

        // constructor
        public MiruDbService(
            IUserDataService userDataService,
            IJikan jikanWrapper, 
            Func<IMiruDbContext> createMiruDbContext, 
            IWebService webService, 
            Lazy<IFileSystemService> fileSystemService,
            Func<MiruAnimeModel> createMiruAnimeModel)
        {
            #region dependency injection

            CurrentSeason = userDataService.CurrentSeasonModel;
            CurrentUserAnimeList = userDataService.CurrentUserAnimeListModel;
            JikanWrapper = jikanWrapper;
            SyncedMyAnimeListUser = userDataService.SyncedMyAnimeListUser;
            WebService = webService;
            CreateMiruDbContext = createMiruDbContext;
            FileSystemService = fileSystemService;
            CreateMiruAnimeModel = createMiruAnimeModel;

            #endregion dependency injection
        }

        private Lazy<IFileSystemService> FileSystemService { get; }
        public IWebService WebService { get; }
        private IJikan JikanWrapper { get; }
        private ISyncedMyAnimeListUser SyncedMyAnimeListUser { get; }
        private Func<IMiruDbContext> CreateMiruDbContext { get; }
        private Func<MiruAnimeModel> CreateMiruAnimeModel { get; }

        public DateTime SyncDateData
        {
            get => _syncDateData;
            set
            {
                _syncDateData = value;
                UpdateSyncDate(this, value);
            }
        }

        public string CurrentUsername 
        { 
            get => currentUsername;
            private set
            {
                if(currentUsername != value)
                {
                    currentUsername = value;
                    UpdateCurrentUsername(this, value);
                }
            }
        }

        // stores data model of the current anime season
        public ICurrentSeasonModel CurrentSeason { get; }

        // stores data model of the currently synced user's anime list
        public ICurrentUserAnimeListModel CurrentUserAnimeList { get; }

        // create custom event handlers
        public delegate void SortedAnimeListEventHandler(List<MiruAnimeModel> animeModels, AnimeListType animeListType);
        public delegate void UpdateAppStatusEventHandler(MiruAppStatus newAppStatus, string detailedAppStatusDescription);

        // create events for updating UI with the data from the db
        public event EventHandler<DateTime> UpdateSyncDate;
        public event EventHandler<string> UpdateCurrentUsername;
        public event SortedAnimeListEventHandler UpdateAnimeListEntriesUI;
        public event UpdateAppStatusEventHandler UpdateAppStatusUI;
        public event EventHandler<int> UpdateSyncProgress;

        // load data from the last sync
        public void LoadLastSyncedData()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<MiruDbContext, Migrations.Configuration>());

            // create the database if it doesn't exist
            Database.SetInitializer(new CreateDatabaseIfNotExists<MiruDbContext>());

            // open temporary connection to the database
            using (var db = CreateMiruDbContext.Invoke())
            {
                // Force database creation if it doesn't exist
                db.Database.CreateIfNotExists();

                // if SyncedMyAnimeListUsers table is not empty
                if (db.SyncedMyAnimeListUsers.Any())
                {
                    // load the sync time of the last synchronization
                    SyncDateData = db.SyncedMyAnimeListUsers.FirstOrDefault().SyncTime;

                    // load the username of the last synchronized user
                    CurrentUsername = db.SyncedMyAnimeListUsers.FirstOrDefault().Username;
                }

                // Cache all anime models
                _cachedAnimeList = db.MiruAnimeModels.ToList();
            }
        }


        // clears anime models and synced users tables
        public void ClearDb()
        {
            using (var db = CreateMiruDbContext.Invoke())
            {
                db.ExecuteSqlCommand("TRUNCATE TABLE [MiruAnimeModels]");
                db.ExecuteSqlCommand("TRUNCATE TABLE [SyncedMyAnimeListUsers]");
            }
            _cachedAnimeList = null;
        }

        // changes data for the displayed anime list to match time zone and anime list type passed as parameters
        public void ChangeDisplayedAnimeList(AnimeListType animeListType, 
                                             TimeZoneInfo selectedTimeZone, 
                                             AnimeType selectedAnimeBroadcastType, 
                                             string animeTitleToFilterBy,
                                             AgeRating ageRating)
        {
            if (_cachedAnimeList == null)
            {
                using (var db = CreateMiruDbContext.Invoke())
                {
                    _cachedAnimeList = db.MiruAnimeModels.ToList();
                }
            }
            var userAnimeList = GetFilteredUserAnimeList(selectedAnimeBroadcastType, animeTitleToFilterBy, selectedTimeZone, ageRating);

            UpdateAnimeListEntriesUI(userAnimeList, animeListType);
        }

        public List<MiruAnimeModel> GetFilteredUserAnimeList(AnimeType selectedBroadcastType,
                                                             string title,
                                                             TimeZoneInfo selectedTimeZone,
                                                             AgeRating ageRating)
        {
            if (_cachedAnimeList == null || _cachedAnimeList.Count == 0)
                return new List<MiruAnimeModel>();

            // Step 1: Filter by type and age rating if changed
            if (_filteredByTypeAndAge == null ||
                selectedBroadcastType != _lastBroadcastType ||
                ageRating != _lastAgeRating)
            {
                _filteredByTypeAndAge = _cachedAnimeList
                    .Where(x => x.MatchesBroadcastType(selectedBroadcastType)
                             && x.MatchesAgeRating(ageRating))
                    .ToList();

                _lastBroadcastType = selectedBroadcastType;
                _lastAgeRating = ageRating;
                _timeZoneConvertedList = null; // Invalidate time zone cache
            }

            // Step 2: Convert time zone if changed
            if (_timeZoneConvertedList == null || selectedTimeZone != _lastTimeZone)
            {
                _timeZoneConvertedList = _filteredByTypeAndAge
                    .Select(x =>
                    {
                        // Clone or shallow copy if needed to avoid mutating cache
                        var animeCopy = x; // If you need a deep copy, implement Clone()
                        animeCopy.ConvertJstBroadcastTimeToSelectedTimeZone(selectedTimeZone);
                        return animeCopy;
                    })
                    .ToList();

                _lastTimeZone = selectedTimeZone;
            }

            // Step 3: Filter by title (fast, on already filtered+converted list)
            List<MiruAnimeModel> userAnimeList;
            if (string.IsNullOrWhiteSpace(title))
            {
                userAnimeList = _timeZoneConvertedList.ToList();
            }
            else
            {
                userAnimeList = _timeZoneConvertedList
                    .Where(x => x.MatchesTitle(title))
                    .ToList();
            }

            return userAnimeList;
        }

        // saves user data to the local database
        public async Task SaveSyncedUserData(string typedInUsername)
        {
            // open temporary connection to the database
            using (var db = CreateMiruDbContext.Invoke())
            {
                // if SyncedMyAnimeListUsers table is not empty then delete all rows
                if (db.SyncedMyAnimeListUsers.Any())
                {
                    db.ExecuteSqlCommand("TRUNCATE TABLE [SyncedMyAnimeListUsers]");
                }

                // store the current user's username and sync date to the SyncedMyAnimeListUsers table
                SyncedMyAnimeListUser.Username = typedInUsername;
                SyncedMyAnimeListUser.SyncTime = SyncDateData = DateTime.Now;

                db.SyncedMyAnimeListUsers.Add(SyncedMyAnimeListUser as SyncedMyAnimeListUser);

                // save changes to the database
                await db.SaveChangesAsync();
            }
        }

        // saves data received from the jikan API to the local database
        public async Task<bool> SaveDetailedAnimeListData(bool seasonSyncOn)
        {
            List<MiruAnimeModel> detailedAnimeList;

            // open temporary connection to the database
            using (var db = CreateMiruDbContext.Invoke())
            {
                UpdateAppStatusUI(MiruAppStatus.Busy, "Downloading detailed user anime list data...");

                // get user anime list with the detailed info
                detailedAnimeList = await GetDetailedUserAnimeList(db, CurrentUserAnimeList.UserAnimeListData.Anime, seasonSyncOn);

                // if the user anime list is empty there were internet connection problems return false
                if (detailedAnimeList == null)
                {
                    return false;
                }

                UpdateAppStatusUI(MiruAppStatus.Busy, "Parse day and time from the broadcast string...");

                // parse day and time from the broadcast string
                detailedAnimeList.ParseTimeFromBroadcast(FileSystemService.Value);

                // clear MiruAnimeModels table from any data
                db.ExecuteSqlCommand("TRUNCATE TABLE [MiruAnimeModels]");

                // add miruAnimeModelsList to the MiruAnimeModels table
                db.MiruAnimeModels.AddRange(detailedAnimeList);

                // update database
                await db.SaveChangesAsync();

                // Refresh cache
                _cachedAnimeList = db.MiruAnimeModels.ToList();

                // Clear all dependent caches so next filter will use fresh data
                _filteredByTypeAndAge = null;
                _timeZoneConvertedList = null;
                _lastBroadcastType = AnimeType.Any;
                _lastAgeRating = AgeRating.Any;
                _lastTimeZone = null;
            }
            return true;
        }

        /// <summary>
        /// Gets detailed anime info for each AnimeListEntry in the collection and saves it as
        /// MiruAnimeModels list that contains the local broadcast time and the number of watched episodes by the user.
        /// </summary>
        /// <param name="db">Context of the database that is going to be updated.</param>
        /// <param name="currentUserAnimeListEntries">Collection of AnimeListEntries that are going to receive broadcast time data.</param>
        /// <returns>List of Anime models with all details required for further operations.</returns>
        public async Task<List<MiruAnimeModel>> GetDetailedUserAnimeList(IMiruDbContext db, ICollection<AnimeListEntry> currentUserAnimeListEntries, bool seasonSyncOn)
        {
            // get anime data from the db
            var detailedUserAnimeList = db.MiruAnimeModels.ToList();

            // set IsOnWatchingList flag to false for all anime models in the db
            detailedUserAnimeList.ForEach(x => { x.IsOnWatchingList = false; x.WatchedEpisodes = 0; });

            // get mal ids of the anime models that were in the db
            var malIdsFromDb = detailedUserAnimeList.Select(x => x.MalId).ToHashSet();

            using (var client = WebService.CreateWebClient.Invoke())
            {
                int currentCompletedCount = 0;
                // for each airing anime from the animeListEntries collection
                foreach (var animeListEntry in currentUserAnimeListEntries)
                {
                    // local variable that temporarily stores detailed anime info from the jikan API
                    Anime animeInfo;

                    // try to get detailed anime info from the jikan API
                    try
                    {
                        animeInfo = await WebService.TryToGetAnimeInfo(animeListEntry.MalId, 500, JikanWrapper);
                    }
                    catch (NoInternetConnectionException)
                    {
                        return null;
                    }

                    string localImagePath;

                    // if the anime is already in the db just set IsOnWatchingList flag instead of adding it again
                    if (malIdsFromDb.Contains(animeListEntry.MalId))
                    {
                        var modelToBeUpdated = detailedUserAnimeList.FirstOrDefault(x => x.MalId == animeListEntry.MalId);
                        modelToBeUpdated.IsOnWatchingList = true;
                        modelToBeUpdated.WatchedEpisodes = animeListEntry.WatchedEpisodes;
                        modelToBeUpdated.TotalEpisodes = animeListEntry.TotalEpisodes;
                        modelToBeUpdated.CurrentlyAiring = (animeListEntry.AiringStatus == AiringStatus.Airing);
                        localImagePath = modelToBeUpdated.LocalImagePath;
                        modelToBeUpdated.UpdateDroppedStatus(CurrentUserAnimeList);
                        modelToBeUpdated.OpeningThemes = JsonConvert.SerializeObject(animeInfo.OpeningTheme);
                        modelToBeUpdated.EndingThemes = JsonConvert.SerializeObject(animeInfo.EndingTheme);
                    }
                    else
                    {
                        var animeModel = CreateMiruAnimeModel.Invoke();
                        animeModel.SetAiringAnimeModelData(animeInfo, animeListEntry, CurrentUserAnimeList);
                        // add airing anime created from the animeInfo data to the airingAnimes list
                        detailedUserAnimeList.Add(animeModel);

                        localImagePath = animeModel.LocalImagePath;
                    }

                    FileSystemService.Value.DownloadFile(client, localImagePath, animeInfo.ImageURL);
                    currentCompletedCount++;
                    UpdateSyncProgress(nameof(GetDetailedUserAnimeList), currentCompletedCount);
                }

                if (seasonSyncOn)
                {
                    detailedUserAnimeList = await GetDetailedSeasonAnimeListInfo(detailedUserAnimeList, client);
                }
            }
            return detailedUserAnimeList;
        }

        /// <summary>
        /// Updates detailedUserAnimeList by adding detailed current season anime data to the list
        /// </summary>
        /// <param name="detailedUserAnimeList">List of detailed user anime data that is going to be updated with seasonal anime data.</param>
        /// <returns></returns>
        private async Task<List<MiruAnimeModel>> GetDetailedSeasonAnimeListInfo(List<MiruAnimeModel> detailedUserAnimeList, IWebClientWrapper client)
        {
            UpdateAppStatusUI(MiruAppStatus.Busy, "Downloading detailed current season anime data...");

            // set airing flag to false for whole list to remove anime that already ended
            detailedUserAnimeList.ForEach(x => x.CurrentlyAiring = false);

            HashSet<long> airingAnimesMalIDs = detailedUserAnimeList.Select(x => x.MalId).ToHashSet();
            var currentFilteredSeasonList = CurrentSeason.GetFilteredSeasonList();
            int currentCompletedCount = 0;

            // add season animes that are not a part of miruAnimeModelsList
            foreach (var seasonEntry in currentFilteredSeasonList)
            {
                if (airingAnimesMalIDs.Contains(seasonEntry.MalId))
                {
                    var modelToBeUpdated = detailedUserAnimeList.FirstOrDefault(x => x.MalId == seasonEntry.MalId);
                    modelToBeUpdated.CurrentlyAiring = true;
                    modelToBeUpdated.UpdateDroppedStatus(CurrentUserAnimeList);
                    // TODO: add here updating age rating or just always do full update
                }
                else
                {
                    // local variable that temporarily stores detailed anime info from the jikan API
                    Anime animeInfo;
                    // get detailed anime info from the jikan API
                    try
                    {
                        animeInfo = await WebService.TryToGetAnimeInfo(seasonEntry.MalId, 500, JikanWrapper);
                    }
                    catch (NoInternetConnectionException)
                    {
                        return null;
                    }

                    var animeModel = CreateMiruAnimeModel.Invoke();
                    animeModel.SetSeasonalAnimeModelData(animeInfo, seasonEntry, CurrentUserAnimeList);
                    string localImagePath = animeModel.LocalImagePath;
                    // add airing anime created from the animeInfo data to the miruAnimeModelsList
                    detailedUserAnimeList.Add(animeModel);

                    FileSystemService.Value.DownloadFile(client, localImagePath, animeInfo.ImageURL);
                }
                currentCompletedCount++;
                UpdateSyncProgress(nameof(GetDetailedSeasonAnimeListInfo), currentCompletedCount);
            }

            // detailed anime list updated successfully
            return detailedUserAnimeList;
        }
    }
}