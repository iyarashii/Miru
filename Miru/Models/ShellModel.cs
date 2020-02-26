﻿using JikanDotNet;
using Miru.Data;
using Miru.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using MyInternetConnectionLibrary;

namespace Miru.Models
{
    public class ShellModel
    {
        public ShellModel(ShellViewModel viewModelContext)
        {
            ViewModelContext = viewModelContext;
        }

        // stores anime list of the currently synced user
        public UserAnimeList CurrentUserAnimeList { get; set; }

        public Season CurrentSeason { get; set; }

        private readonly ShellViewModel ViewModelContext;

        public ReadOnlyCollection<TimeZoneInfo> TimeZones { get; } = TimeZoneInfo.GetSystemTimeZones();

        // get user's watching status anime list
        public async Task<bool> GetCurrentUserAnimeList()
        {
            // get user's watching status anime list
            CurrentUserAnimeList = await Constants.jikan.GetUserAnimeList(ViewModelContext.TypedInUsername, UserAnimeListExtension.Watching);

            // if there is no response from API wait 1 second and retry
            while (CurrentUserAnimeList == null)
            {
                await Task.Delay(1000);
                if (!await InternetConnectionViewModel.CheckAppInternetConnectionStatus(ViewModelContext)) return false;
                CurrentUserAnimeList = await Constants.jikan.GetUserAnimeList(ViewModelContext.TypedInUsername, UserAnimeListExtension.Watching);
            }
            return true;
        }

        public async Task<bool> GetCurrentSeasonList()
        {
            // get current season
            CurrentSeason = await Constants.jikan.GetSeason();

            while (CurrentSeason == null)
            {
                await Task.Delay(2000);
                if (!await InternetConnectionViewModel.CheckAppInternetConnectionStatus(ViewModelContext)) return false;
                CurrentSeason = await Constants.jikan.GetSeason();
            }
            return true;
        }

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

        public async Task<bool> SaveSyncData(bool syncSeasonList)
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
                db.SyncedMyAnimeListUsers.Add(new SyncedMyAnimeListUser { Username = ViewModelContext.TypedInUsername, SyncTime = ViewModelContext.SyncDate = DateTime.Now });

                // save changes to the database
                await db.SaveChangesAsync();

                // get the anime broadcast times, convert them to the GMT+1 timezone and save them to the database
                if (!await GetAiringAnimeBroadcastTimes(db, CurrentUserAnimeList.Anime, syncSeasonList)) return false;
            }

            // clear data from properties
            CurrentSeason = null;
            CurrentUserAnimeList = null;

            return true;
        }

        /// <summary>
        /// Gets detailed anime info for each AnimeListEntry in the collection and saves it as
        /// MiruAiringAnimeModels that contain the local broadcast time and the number of watched episodes by the user.
        /// </summary>
        /// <param name="db">Context of the database that is going to be updated.</param>
        /// <param name="animeListEntries">Collection of AnimeListEntries that are going to receive broadcast time data.</param>
        /// <returns></returns>
        public async Task<bool> GetAiringAnimeBroadcastTimes(MiruDbContext db, ICollection<AnimeListEntry> animeListEntries, bool syncSeasonList)
        {
            // get anime data from the db
            var miruAnimeModelsList = db.MiruAiringAnimeModels.ToList();

            // set IsOnWatchingList flag to false for all anime models in the db
            miruAnimeModelsList.ForEach(x => x.IsOnWatchingList = false);

            // get mal ids of the anime models that were in the db
            var malIdsFromDb = new HashSet<long>(miruAnimeModelsList.Select(x => x.MalId));

            // local variable that temporarily stores detailed anime info from the jikan API
            Anime animeInfo;

            // for each airing anime from the animeListEntries collection
            foreach (var animeListEntry in animeListEntries.Where(a => a.AiringStatus == AiringStatus.Airing))
            {
                // try to get detailed anime info from the jikan API
                try
                {
                    animeInfo = await TryToGetAnimeInfo(animeListEntry.MalId, 1100);
                }
                catch (NoInternetConnectionException)
                {
                    return false;
                }

                // if the anime is already in the db just set IsOnWatchingList flag instead of adding it again
                if (malIdsFromDb.Contains(animeInfo.MalId))
                {
                    miruAnimeModelsList.FirstOrDefault(x => x.MalId == animeInfo.MalId).IsOnWatchingList = true;
                }
                else
                {
                    // add airing anime created from the animeInfo data to the airingAnimes list
                    miruAnimeModelsList.Add(new MiruAiringAnimeModel
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

            if (syncSeasonList)
            {
                HashSet<long> airingAnimesMalIDs = new HashSet<long>(miruAnimeModelsList.Select(x => x.MalId));

                var currentSeasonList = CurrentSeason.SeasonEntries.ToList();

                currentSeasonList.RemoveAll(x => x.Type != "TV");
                currentSeasonList.RemoveAll(x => x.Kids == true);

                currentSeasonList.RemoveAll(x => airingAnimesMalIDs.Contains(x.MalId));

                // add season animes that are not on watching list
                foreach (var seasonEntry in currentSeasonList)
                {
                    // get detailed anime info from the jikan API
                    try
                    {
                        animeInfo = await TryToGetAnimeInfo(seasonEntry.MalId, 3000);
                    }
                    catch (NoInternetConnectionException)
                    {
                        return false;
                    }
                    // add airing anime created from the animeInfo data to the airingAnimes list
                    miruAnimeModelsList.Add(new MiruAiringAnimeModel
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
            }

            // parse day and time from broadcast string
            miruAnimeModelsList = ParseTimeFromBroadcast(miruAnimeModelsList);

            // clear MiruAiringAnimeModels table from any data
            db.Database.ExecuteSqlCommand("TRUNCATE TABLE [MiruAiringAnimeModels]");

            // add airingAnimes list to the MiruAiringAnimeModels table
            db.MiruAiringAnimeModels.AddRange(miruAnimeModelsList);

            // update database
            await db.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Parses day and time from the broadcast string, converts it to the local time zone and saves to the LocalBroadcastTime property of the MiruAiringAnimeModel.
        /// </summary>
        /// <param name="airingAnimes">List of airing animes without parsed day and time data.</param>
        /// <returns>List of airing animes with parsed data saved in LocalBroadcastTime properties.</returns>
        public List<MiruAiringAnimeModel> ParseTimeFromBroadcast(List<MiruAiringAnimeModel> airingAnimes)
        {
            // local variables
            string dayOfTheWeek;
            string[] broadcastWords;
            DateTime broadcastTime;
            DateTime time;
            DateTime localBroadcastTime;

            // remove airing animes without specified broadcast time (like OVAs)
            airingAnimes.RemoveAll(x => string.IsNullOrWhiteSpace(x.Broadcast));
            airingAnimes.RemoveAll(x => x.Broadcast.Contains("Unknown") || x.Broadcast.Contains("Not scheduled"));

            // for each airingAnime parse time and day of the week from the broadcast string
            foreach (var airingAnime in airingAnimes)
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

                airingAnime.JSTBroadcastTime = broadcastTime;

                // covert JST to utc
                var utc = broadcastTime.AddHours(-9);

                // get local time zone info
                var localTimeZone = TimeZoneInfo.Local;

                // convert JST to your computer's local time
                localBroadcastTime = utc.AddHours(localTimeZone.BaseUtcOffset.Hours);

                // save parsed date and time to the model's property
                airingAnime.LocalBroadcastTime = localBroadcastTime;
            }

            // return list of airing animes with parsed data saved in LocalBroadcastTime properties
            return airingAnimes;
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
                await Task.Delay(millisecondsDelay);
                if (!await InternetConnectionViewModel.CheckAppInternetConnectionStatus(ViewModelContext)) 
                {
                    throw new NoInternetConnectionException("No internet connection");
                }
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