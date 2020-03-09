﻿using JikanDotNet;
using Miru.Models;
using Miru.ViewModels;
using MyInternetConnectionLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miru.Data
{
    // contains the business logic that uses the local db
    public class MiruDbService
    {
        // constructor
        public MiruDbService(ShellViewModel viewModelContext)
        {
            ViewModelContext = viewModelContext;
        }

        // stores view model's context
        public ShellViewModel ViewModelContext { get; set; }

        // stores data model of the current anime season
        public CurrentSeasonModel CurrentSeason { get; set; } = new CurrentSeasonModel();

        // stores data model of the currently synced user's anime list
        public CurrentUserAnimeListModel CurrentUserAnimeList { get; set; } = new CurrentUserAnimeListModel();

        // load data from the last sync
        public void LoadLastSyncedData()
        {
            // open temporary connection to the database
            using (var db = new MiruDbContext())
            {
                // if SyncedMyAnimeListUsers table is not empty
                if (db.SyncedMyAnimeListUsers.Any())
                {
                    // set SyncDate prop to the sync time of the last synchronization
                    ViewModelContext.SyncDate = db.SyncedMyAnimeListUsers.FirstOrDefault().SyncTime;

                    // set SyncStatusText and TypedInUsername props to the username of the last synchronized user
                    ViewModelContext.SyncStatusText = ViewModelContext.TypedInUsername = db.SyncedMyAnimeListUsers.FirstOrDefault().Username;

                    // get the user's list of the airing animes from the db
                    var airingAnimeList = db.MiruAiringAnimeModels.ToList();

                    // set airing anime list entries for each day of the week
                    ViewModelContext.SortedAnimeListEntries.SortAiringAnime(airingAnimeList);
                }
            }
        }

        // changes data for the displayed anime list to match time zone and anime list type passed as parameters
        public void ChangeDisplayedAnimeList(AnimeListType animeListType, TimeZoneInfo selectedTimeZone)
        {
            using (var db = new MiruDbContext())
            {
                // get the user's list of the airing animes from the db
                var airingAnimeList = db.MiruAiringAnimeModels.ToList();
                DateTime utc;
                foreach (var airingAnime in airingAnimeList)
                {
                    // covert JST to utc
                    utc = airingAnime.JSTBroadcastTime.Value.AddHours(-9);
                    // set selected timezone as local broadcast time
                    airingAnime.LocalBroadcastTime = utc.AddHours(selectedTimeZone.BaseUtcOffset.Hours);
                }

                if (animeListType == AnimeListType.Watching)
                {
                    // set airing anime list entries for each day of the week
                    ViewModelContext.SortedAnimeListEntries.SortAiringAnime(airingAnimeList);
                }
                else if (animeListType == AnimeListType.Season)
                {
                    ViewModelContext.SortedAnimeListEntries.SortCurrentSeasonAiringAnime(airingAnimeList);
                }
            }
        }

        // saves user data to the local database
        public async Task SaveSyncedUserData()
        {
            // open temporary connection to the database
            using (var db = new MiruDbContext())
            {
                // if SyncedMyAnimeListUsers table is not empty then delete all rows
                if (db.SyncedMyAnimeListUsers.Any())
                {
                    db.Database.ExecuteSqlCommand("TRUNCATE TABLE [SyncedMyAnimeListUsers]");
                }

                // store the current user's username and sync date to the SyncedMyAnimeListUsers table
                db.SyncedMyAnimeListUsers.Add(new SyncedMyAnimeListUser
                {
                    Username = ViewModelContext.TypedInUsername,
                    SyncTime = ViewModelContext.SyncDate = DateTime.Now
                });

                // save changes to the database
                await db.SaveChangesAsync();
            }
        }

        // saves data received from the jikan API to the local database
        public async Task<bool> SaveDetailedAnimeListData(bool seasonSyncOn)
        {
            List<MiruAiringAnimeModel> detailedAnimeList;

            // open temporary connection to the database
            using (var db = new MiruDbContext())
            {
                ViewModelContext.AppStatusText = "Getting detailed user anime list data...";
                // get user anime list with the detailed info
                detailedAnimeList = await GetDetailedUserAnimeList(db, CurrentUserAnimeList.UserAnimeListData.Anime);

                // if the user anime list is empty there were internet connection problems return false
                if (detailedAnimeList == null)
                {
                    return false;
                }

                // if 'get current season list' button was used to sync data
                if (seasonSyncOn)
                {
                    ViewModelContext.AppStatusText = "Getting detailed current season anime data...";
                    // update user anime list with season anime detailed data if it fails return false
                    if (!await GetDetailedSeasonAnimeListInfo(detailedAnimeList))
                    {
                        return false;
                    }
                }
                ViewModelContext.AppStatusText = "Parse day and time from the broadcast string...";
                // parse day and time from the broadcast string
                detailedAnimeList = ParseTimeFromBroadcast(detailedAnimeList);

                // clear MiruAiringAnimeModels table from any data
                db.Database.ExecuteSqlCommand("TRUNCATE TABLE [MiruAiringAnimeModels]");

                // add miruAnimeModelsList to the MiruAiringAnimeModels table
                db.MiruAiringAnimeModels.AddRange(detailedAnimeList);

                // update database
                await db.SaveChangesAsync();
            }
            return true;
        }

        /// <summary>
        /// Gets detailed anime info for each AnimeListEntry in the collection and saves it as
        /// MiruAiringAnimeModels list that contains the local broadcast time and the number of watched episodes by the user.
        /// </summary>
        /// <param name="db">Context of the database that is going to be updated.</param>
        /// <param name="currentUserAnimeListEntries">Collection of AnimeListEntries that are going to receive broadcast time data.</param>
        /// <returns></returns>
        public async Task<List<MiruAiringAnimeModel>> GetDetailedUserAnimeList(MiruDbContext db, ICollection<AnimeListEntry> currentUserAnimeListEntries)
        {
            // get anime data from the db
            var detailedUserAnimeList = db.MiruAiringAnimeModels.ToList();

            // set IsOnWatchingList flag to false for all anime models in the db
            detailedUserAnimeList.ForEach(x => { x.IsOnWatchingList = false; x.WatchedEpisodes = 0; });

            // get mal ids of the anime models that were in the db
            var malIdsFromDb = new HashSet<long>(detailedUserAnimeList.Select(x => x.MalId));

            // local variable that temporarily stores detailed anime info from the jikan API
            Anime animeInfo;

            // for each airing anime from the animeListEntries collection
            foreach (var animeListEntry in currentUserAnimeListEntries.Where(a => a.AiringStatus == AiringStatus.Airing))
            {
                // try to get detailed anime info from the jikan API
                try
                {
                    animeInfo = await TryToGetAnimeInfo(animeListEntry.MalId, 2000);
                }
                catch (NoInternetConnectionException)
                {
                    return null;
                }

                // if the anime is already in the db just set IsOnWatchingList flag instead of adding it again
                if (malIdsFromDb.Contains(animeInfo.MalId))
                {
                    var modelToBeUpdated = detailedUserAnimeList.FirstOrDefault(x => x.MalId == animeInfo.MalId);
                    modelToBeUpdated.IsOnWatchingList = true;
                    modelToBeUpdated.WatchedEpisodes = animeListEntry.WatchedEpisodes;
                    modelToBeUpdated.TotalEpisodes = animeListEntry.TotalEpisodes;
                }
                else
                {
                    // add airing anime created from the animeInfo data to the airingAnimes list
                    detailedUserAnimeList.Add(new MiruAiringAnimeModel
                    {
                        MalId = animeInfo.MalId,
                        Broadcast = animeInfo.Broadcast,
                        Title = animeInfo.Title,
                        ImageURL = animeInfo.ImageURL,
                        TotalEpisodes = animeListEntry.TotalEpisodes,
                        URL = animeListEntry.URL,
                        WatchedEpisodes = animeListEntry.WatchedEpisodes,
                        IsOnWatchingList = true
                    });
                }
            }
            return detailedUserAnimeList;
        }

        /// <summary>
        /// Updates detailedUserAnimeList by adding detailed current season anime data to the list
        /// </summary>
        /// <param name="detailedUserAnimeList">List of detailed user anime data that is going to be updated with seasonal anime data.</param>
        /// <returns></returns>
        public async Task<bool> GetDetailedSeasonAnimeListInfo(List<MiruAiringAnimeModel> detailedUserAnimeList)
        {
            // set of all anime ids from the db
            HashSet<long> airingAnimesMalIDs = new HashSet<long>(detailedUserAnimeList.Select(x => x.MalId));

            // list of anime entries in the current season
            var currentSeasonList = CurrentSeason.SeasonData.SeasonEntries.ToList();

            // remove anime entries with types other than 'TV' from the list
            currentSeasonList.RemoveAll(x => x.Type != "TV");

            // remove anime entries marked as 'for kids' from the list
            currentSeasonList.RemoveAll(x => x.Kids == true);

            // remove anime entries that are already in the local db from the list
            currentSeasonList.RemoveAll(x => airingAnimesMalIDs.Contains(x.MalId));

            // add season animes that are not a part of miruAnimeModelsList
            foreach (var seasonEntry in currentSeasonList)
            {
                // local variable that temporarily stores detailed anime info from the jikan API
                Anime animeInfo;

                // get detailed anime info from the jikan API
                try
                {
                    animeInfo = await TryToGetAnimeInfo(seasonEntry.MalId, 3000);
                }
                catch (NoInternetConnectionException)
                {
                    return false;
                }
                // add airing anime created from the animeInfo data to the miruAnimeModelsList
                detailedUserAnimeList.Add(new MiruAiringAnimeModel
                {
                    MalId = animeInfo.MalId,
                    Broadcast = animeInfo.Broadcast,
                    Title = animeInfo.Title,
                    ImageURL = animeInfo.ImageURL,
                    //TotalEpisodes = eps,
                    URL = seasonEntry.URL,
                    IsOnWatchingList = false
                });
            }

            // detailed anime list updated successfully
            return true;
        }

        /// <summary>
        /// Parses day and time from the broadcast string, converts it to the local time zone and saves to the LocalBroadcastTime property of the MiruAiringAnimeModel.
        /// </summary>
        /// <param name="detailedAnimeList">List of airing animes with broadcast strings but without parsed day and time data.</param>
        /// <returns>List of airing animes with parsed data saved in LocalBroadcastTime properties.</returns>
        public List<MiruAiringAnimeModel> ParseTimeFromBroadcast(List<MiruAiringAnimeModel> detailedAnimeList)
        {
            // local variables
            string dayOfTheWeek;
            string[] broadcastWords;
            DateTime broadcastTime;
            DateTime time;
            DateTime localBroadcastTime;

            // remove airing animes without specified broadcast time (like OVAs)
            detailedAnimeList.RemoveAll(x => string.IsNullOrWhiteSpace(x.Broadcast));
            detailedAnimeList.RemoveAll(x => x.Broadcast.Contains("Unknown") || x.Broadcast.Contains("Not scheduled"));

            // for each airingAnime parse time and day of the week from the broadcast string
            foreach (var airingAnime in detailedAnimeList)
            {
                // split the broadcast string into words
                broadcastWords = airingAnime.Broadcast.Split(' ');

                // set the first word of the broadcast string as a day of the week
                dayOfTheWeek = broadcastWords[0];

                // parse time from the 2nd broadcast string word
                time = DateTime.Parse(broadcastWords[2]);

                // depending on the 1st word set the correct day
                switch (dayOfTheWeek)
                {
                    case "Mondays":
                        broadcastTime = new DateTime(2020, 01, 20, time.Hour, time.Minute, 0);
                        break;

                    case "Tuesdays":
                        broadcastTime = new DateTime(2020, 01, 21, time.Hour, time.Minute, 0);
                        break;

                    case "Wednesdays":
                        broadcastTime = new DateTime(2020, 01, 22, time.Hour, time.Minute, 0);
                        break;

                    case "Thursdays":
                        broadcastTime = new DateTime(2020, 01, 23, time.Hour, time.Minute, 0);
                        break;

                    case "Fridays":
                        broadcastTime = new DateTime(2020, 01, 24, time.Hour, time.Minute, 0);
                        break;

                    case "Saturdays":
                        broadcastTime = new DateTime(2020, 01, 25, time.Hour, time.Minute, 0);
                        break;

                    case "Sundays":
                        broadcastTime = new DateTime(2020, 01, 26, time.Hour, time.Minute, 0);
                        break;

                    default:
                        broadcastTime = new DateTime();
                        break;
                }

                // save JST broadcast time parsed from the Broadcast string
                airingAnime.JSTBroadcastTime = broadcastTime;

                // covert JST to UTC
                var utc = broadcastTime.AddHours(-9);

                // get local time zone info
                var localTimeZone = TimeZoneInfo.Local;

                // convert JST to your computer's local time
                localBroadcastTime = utc.AddHours(localTimeZone.BaseUtcOffset.Hours);

                // save parsed date and time to the model's property
                airingAnime.LocalBroadcastTime = localBroadcastTime;
            }

            // return list of airing animes with parsed data saved in LocalBroadcastTime properties
            return detailedAnimeList;
        }

        // tries to get the detailed anime information about anime with the given mal id, retries after given delay until the internet connection is working
        public async Task<Anime> TryToGetAnimeInfo(long malId, int millisecondsDelay)
        {
            Anime output;
            try
            {
                // get detailed anime info from the jikan API
                output = await Constants.jikan.GetAnime(malId);
            }
            catch (System.Net.Http.HttpRequestException)
            {
                throw new NoInternetConnectionException("No internet connection");
            }

            // if there is no response from API wait fo the given time and retry
            while (output == null)
            {
                //await Task.Delay(millisecondsDelay);
                //if (!await InternetConnectionViewModel.CheckAppInternetConnectionStatus(ViewModelContext))
                //{
                //    throw new NoInternetConnectionException("No internet connection");
                //}
                try
                {
                    // get detailed anime info from the jikan API
                    output = await Constants.jikan.GetAnime(malId);
                }
                catch (System.Net.Http.HttpRequestException)
                {
                    throw new NoInternetConnectionException("No internet connection");
                }
            }
            return output;
        }
    }
}