using JikanDotNet;
using Miru.Data;
using Miru.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        private ShellViewModel ViewModelContext;

        // get user's watching status anime list
        public async Task<bool> GetCurrentUserAnimeList()
        {
            // get user's watching status anime list
            CurrentUserAnimeList = await Constants.jikan.GetUserAnimeList(ViewModelContext.TypedInUsername, UserAnimeListExtension.Watching);

            // if there is no response from API wait 1 second and retry
            while (CurrentUserAnimeList == null)
            {
                await Task.Delay(1000);
                if (!await InternetConnectionViewModel.CheckAppInternetConnectionStatus(ViewModelContext)) return false; ;
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

        public void ChangeDisplayedAnimeList(AnimeListType animeListType)
        {
            using (var db = new MiruDbContext())
            {
                // get the user's list of the airing animes from the db
                var airingAnimeList = db.MiruAiringAnimeModels.ToList();

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
                // delete all table rows -- slower version
                //db.AnimeListEntries.RemoveRange(db.AnimeListEntries);

                // check if anime entries table is empty
                if (db.AnimeListEntries.Any())
                {
                    // delete all table rows -- faster version
                    db.Database.ExecuteSqlCommand("TRUNCATE TABLE [AnimeListEntries]");
                }

                // add anime info from the user's watching anime list to the AnimeListEntries table
                db.AnimeListEntries.AddRange(CurrentUserAnimeList.Anime);

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
                if (await GetAiringAnimeBroadcastTimes(db, CurrentUserAnimeList.Anime, syncSeasonList) == false) return false;
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
            // clear MiruAiringAnimeModels table from any data
            if (db.MiruAiringAnimeModels.Any())
            {
                // TODO: find a fix
                //if (syncSeasonList)
                //{
                //    db.Database.ExecuteSqlCommand("TRUNCATE TABLE [MiruAiringAnimeModels]");
                //}
                //else
                //{
                //    var miruAnimeModelsList = db.MiruAiringAnimeModels.ToList();
                //    miruAnimeModelsList.RemoveAll(x => x.IsOnWatchingList == false);
                //    db.MiruAiringAnimeModels.RemoveRange(miruAnimeModelsList);
                //    //db.SaveChanges();
                //    miruAnimeModelsList = null;
                //}
                db.Database.ExecuteSqlCommand("TRUNCATE TABLE [MiruAiringAnimeModels]");
            }

            // initialize empty list of MiruAiringAnimeModels
            List<MiruAiringAnimeModel> airingAnimes = new List<MiruAiringAnimeModel>();

            // local variable that temporarily stores detailed anime info from the jikan API
            Anime animeInfo;

            // for each airing anime from the animeListEntries collection
            foreach (var animeListEntry in animeListEntries.Where(a => a.AiringStatus == AiringStatus.Airing))
            {
                // get detailed anime info from the jikan API
                animeInfo = await Constants.jikan.GetAnime(animeListEntry.MalId);

                // if there is no response from API wait 1.1 second and retry
                while (animeInfo == null)
                {
                    await Task.Delay(1100);
                    if (!await InternetConnectionViewModel.CheckAppInternetConnectionStatus(ViewModelContext)) return false;
                    animeInfo = await Constants.jikan.GetAnime(animeListEntry.MalId);
                }

                // add airing anime created from the animeInfo data to the airingAnimes list
                airingAnimes.Add(new MiruAiringAnimeModel
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

            if (syncSeasonList)
            {
                HashSet<long> airingAnimesMalIDs = new HashSet<long>(airingAnimes.Select(x => x.MalId));

                var currentSeasonList = CurrentSeason.SeasonEntries.ToList();
                var seasonListStrings = currentSeasonList.Select(x => x.Type);
                currentSeasonList.RemoveAll(x => x.Type != "TV");
                currentSeasonList.RemoveAll(x => x.Kids == true);
                currentSeasonList.RemoveAll(x => x.Continued == true);

                //currentSeasonList.RemoveAll(x => x.Type == "OVA");
                //currentSeasonList.RemoveAll(x => x.Type == "Special");
                //currentSeasonList.RemoveAll(x => x.Type == "Movie");

                currentSeasonList.RemoveAll(x => airingAnimesMalIDs.Contains(x.MalId));

                // add season animes that are not on watching list
                foreach (var seasonEntry in currentSeasonList)
                {
                    // get detailed anime info from the jikan API
                    animeInfo = await Constants.jikan.GetAnime(seasonEntry.MalId);

                    // if there is no response from API wait 2 seconds and retry
                    while (animeInfo == null)
                    {
                        await Task.Delay(3000);
                        if (!await InternetConnectionViewModel.CheckAppInternetConnectionStatus(ViewModelContext)) return false;
                        animeInfo = await Constants.jikan.GetAnime(seasonEntry.MalId);
                    }

                    // add airing anime created from the animeInfo data to the airingAnimes list
                    airingAnimes.Add(new MiruAiringAnimeModel
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
            airingAnimes = ParseTimeFromBroadcast(airingAnimes);

            // add airingAnimes list to the MiruAiringAnimeModels table
            db.MiruAiringAnimeModels.AddRange(airingAnimes);

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
            airingAnimes.RemoveAll(x => x.Broadcast == "Unknown");

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

                // convert JST to GMT+1 time
                localBroadcastTime = broadcastTime.AddHours(-8);

                // save parsed date and time to the model's property
                airingAnime.LocalBroadcastTime = localBroadcastTime;
            }

            // set airing anime list entries for each day of the week
            ViewModelContext.SortedAnimeListEntries.SortAiringAnime(airingAnimes);

            // return list of airing animes with parsed data saved in LocalBroadcastTime properties
            return airingAnimes;
        }
    }
}
