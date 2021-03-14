using JikanDotNet;
using MiruLibrary.Models;
using MyInternetConnectionLibrary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MiruLibrary;
using AnimeType = MiruLibrary.AnimeType;

namespace MiruDatabaseLogicLayer
{
    // contains the business logic that uses the local db
    // TODO: separate filesystem methods from this class and create DataService for ShellViewModel to contain both services
    public class MiruDbService : IMiruDbService
    {
        private DateTime _syncDateData;
        private string currentUsername;

        // constructor
        public MiruDbService(ICurrentSeasonModel currentSeasonModel, ICurrentUserAnimeListModel currentUserAnimeListModel, IJikan jikanWrapper, Func<IMiruDbContext> createMiruDbContext, ISyncedMyAnimeListUser syncedMyAnimeListUser, IWebService webService)
        {
            #region dependency injection

            CurrentSeason = currentSeasonModel;
            CurrentUserAnimeList = currentUserAnimeListModel;
            JikanWrapper = jikanWrapper;
            SyncedMyAnimeListUser = syncedMyAnimeListUser;
            WebService = webService;
            CreateMiruDbContext = createMiruDbContext;

            #endregion dependency injection
        }

        public IWebService WebService { get; }
        private IJikan JikanWrapper { get; }
        private ISyncedMyAnimeListUser SyncedMyAnimeListUser { get; }
        private Func<IMiruDbContext> CreateMiruDbContext { get; }

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

        // load data from the last sync
        public void LoadLastSyncedData()
        {
            // open temporary connection to the database
            using (var db = CreateMiruDbContext.Invoke())
            {
                // if SyncedMyAnimeListUsers table is not empty
                if (db.SyncedMyAnimeListUsers.Any())
                {
                    // load the sync time of the last synchronization
                    SyncDateData = db.SyncedMyAnimeListUsers.FirstOrDefault().SyncTime;

                    // load the username of the last synchronized user
                    CurrentUsername = db.SyncedMyAnimeListUsers.FirstOrDefault().Username;
                }
            }
        }

        // clears anime models and synced users tables
        public void ClearDb()
        {
            using (var db = CreateMiruDbContext.Invoke())
            {
                db.Database.ExecuteSqlCommand("TRUNCATE TABLE [MiruAnimeModels]");
                db.Database.ExecuteSqlCommand("TRUNCATE TABLE [SyncedMyAnimeListUsers]");
            }
        }

        // changes data for the displayed anime list to match time zone and anime list type passed as parameters
        public void ChangeDisplayedAnimeList(AnimeListType animeListType, TimeZoneInfo selectedTimeZone, AnimeType selectedAnimeType, string animeNameFilter)
        {
            using (var db = CreateMiruDbContext.Invoke())
            {
                // get the user's list of the airing animes from the db
                var airingAnimeList = db.MiruAnimeModels.ToList();

                // filter the anime list
                airingAnimeList.FilterByBroadcastType(selectedAnimeType);
                airingAnimeList.FilterByTitle(animeNameFilter);

                foreach (var airingAnime in airingAnimeList)
                {
                    // save JST broadcast time converted to the selected timezone as local broadcast time
                    airingAnime.ConvertJstBroadcastTimeToSelectedTimeZone(selectedTimeZone);
                }

                // set airing anime list entries for each day of the week
                UpdateAnimeListEntriesUI(airingAnimeList, animeListType);
            }
        }

        // saves user data to the local database
        public async Task SaveSyncedUserData(string typedInUsername)
        {
            // open temporary connection to the database
            using (var db = CreateMiruDbContext.Invoke())
            {
                //TODO: Change here if multiple users should be supported
                // if SyncedMyAnimeListUsers table is not empty then delete all rows
                if (db.SyncedMyAnimeListUsers.Any())
                {
                    db.Database.ExecuteSqlCommand("TRUNCATE TABLE [SyncedMyAnimeListUsers]");
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
                UpdateAppStatusUI(MiruAppStatus.Busy, "Getting detailed user anime list data...");

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
                    UpdateAppStatusUI(MiruAppStatus.Busy, "Getting detailed current season anime data...");

                    // update user anime list with season anime detailed data if it fails return false
                    if (!await GetDetailedSeasonAnimeListInfo(detailedAnimeList))
                    {
                        return false;
                    }
                }
                UpdateAppStatusUI(MiruAppStatus.Busy, "Parse day and time from the broadcast string...");

                // parse day and time from the broadcast string
                detailedAnimeList = ParseTimeFromBroadcast(detailedAnimeList);

                // clear MiruAnimeModels table from any data
                db.Database.ExecuteSqlCommand("TRUNCATE TABLE [MiruAnimeModels]");

                // add miruAnimeModelsList to the MiruAnimeModels table
                db.MiruAnimeModels.AddRange(detailedAnimeList);

                // update database
                await db.SaveChangesAsync();
            }
            return true;
        }

        /// <summary>
        /// Gets detailed anime info for each AnimeListEntry in the collection and saves it as
        /// MiruAnimeModels list that contains the local broadcast time and the number of watched episodes by the user.
        /// </summary>
        /// <param name="db">Context of the database that is going to be updated.</param>
        /// <param name="currentUserAnimeListEntries">Collection of AnimeListEntries that are going to receive broadcast time data.</param>
        /// <returns></returns>
        public async Task<List<MiruAnimeModel>> GetDetailedUserAnimeList(IMiruDbContext db, ICollection<AnimeListEntry> currentUserAnimeListEntries)
        {
            // get anime data from the db
            var detailedUserAnimeList = db.MiruAnimeModels.ToList();

            // set IsOnWatchingList flag to false for all anime models in the db
            detailedUserAnimeList.ForEach(x => { x.IsOnWatchingList = false; x.WatchedEpisodes = 0; });

            // get mal ids of the anime models that were in the db
            var malIdsFromDb = new HashSet<long>(detailedUserAnimeList.Select(x => x.MalId));

            using (var client = WebService.CreateWebClient.Invoke())
            {
                // for each airing anime from the animeListEntries collection
                foreach (var animeListEntry in currentUserAnimeListEntries)
                {
                    // local variable that temporarily stores detailed anime info from the jikan API
                    Anime animeInfo;

                    // try to get detailed anime info from the jikan API
                    try
                    {
                        animeInfo = await TryToGetAnimeInfo(animeListEntry.MalId, 500);
                    }
                    catch (NoInternetConnectionException)
                    {
                        return null;
                    }

                    string localImagePath = Path.Combine(Constants.ImageCacheFolderPath, $"{ animeInfo.MalId }.jpg");

                    // if the anime is already in the db just set IsOnWatchingList flag instead of adding it again
                    if (malIdsFromDb.Contains(animeListEntry.MalId))
                    {
                        var modelToBeUpdated = detailedUserAnimeList.FirstOrDefault(x => x.MalId == animeListEntry.MalId);
                        modelToBeUpdated.IsOnWatchingList = true;
                        modelToBeUpdated.WatchedEpisodes = animeListEntry.WatchedEpisodes;
                        modelToBeUpdated.TotalEpisodes = animeListEntry.TotalEpisodes;
                        modelToBeUpdated.CurrentlyAiring = (animeListEntry.AiringStatus == AiringStatus.Airing);
                    }
                    else
                    {
                        // add airing anime created from the animeInfo data to the airingAnimes list
                        detailedUserAnimeList.Add(new MiruAnimeModel
                        {
                            MalId = animeInfo.MalId,
                            Broadcast = animeInfo.Broadcast ?? animeInfo.Aired.From.ToString(),
                            Title = animeInfo.Title,
                            ImageURL = animeInfo.ImageURL,
                            LocalImagePath = localImagePath,
                            TotalEpisodes = animeListEntry.TotalEpisodes,
                            URL = animeListEntry.URL,
                            WatchedEpisodes = animeListEntry.WatchedEpisodes,
                            IsOnWatchingList = true,
                            CurrentlyAiring = animeListEntry.AiringStatus == AiringStatus.Airing,
                            Type = animeInfo.Type
                        });

                    }
                    
                    if (!File.Exists(localImagePath))
                    {
                        client.DownloadFile(animeInfo.ImageURL, localImagePath);
                    }
                }
            }
            return detailedUserAnimeList;
        }

        /// <summary>
        /// Updates detailedUserAnimeList by adding detailed current season anime data to the list
        /// </summary>
        /// <param name="detailedUserAnimeList">List of detailed user anime data that is going to be updated with seasonal anime data.</param>
        /// <returns></returns>
        public async Task<bool> GetDetailedSeasonAnimeListInfo(List<MiruAnimeModel> detailedUserAnimeList)
        {
            // TODO: Added something to delete animes from the previous season that are no longer on your watching list and season ended -- check if good
            // set airing flag to false for whole list to remove anime that already ended
            detailedUserAnimeList.ForEach(x => x.CurrentlyAiring = false);

            // set of all anime ids from the db
            HashSet<long> airingAnimesMalIDs = new HashSet<long>(detailedUserAnimeList.Select(x => x.MalId));


            // list of anime entries in the current season
            var currentSeasonList = CurrentSeason.SeasonData.SeasonEntries.ToList();

            // remove anime entries with types other than 'TV' from the list
            currentSeasonList.RemoveAll(x => x.Type != "TV" && x.Type != "ONA");

            // remove anime entries marked as 'for kids' from the list
            currentSeasonList.RemoveAll(x => x.Kids == true);

            using (var client = WebService.CreateWebClient.Invoke())
            {
                // add season animes that are not a part of miruAnimeModelsList
                foreach (var seasonEntry in currentSeasonList)
                {
                    // local variable that temporarily stores detailed anime info from the jikan API
                    Anime animeInfo;
                    // get detailed anime info from the jikan API
                    try
                    {
                        animeInfo = await TryToGetAnimeInfo(seasonEntry.MalId, 500);
                    }
                    catch (NoInternetConnectionException)
                    {
                        return false;
                    }

                    string localImagePath = Path.Combine(Constants.ImageCacheFolderPath, $"{ animeInfo.MalId }.jpg");

                    if (airingAnimesMalIDs.Contains(seasonEntry.MalId))
                    {
                        var modelToBeUpdated = detailedUserAnimeList.FirstOrDefault(x => x.MalId == seasonEntry.MalId);
                        modelToBeUpdated.CurrentlyAiring = true;
                    }
                    else
                    {
                        // add airing anime created from the animeInfo data to the miruAnimeModelsList
                        detailedUserAnimeList.Add(new MiruAnimeModel
                        {
                            MalId = animeInfo.MalId,
                            Broadcast = animeInfo.Broadcast ?? animeInfo.Aired.From.ToString(),
                            Title = animeInfo.Title,
                            ImageURL = animeInfo.ImageURL,
                            LocalImagePath = localImagePath,
                            //TotalEpisodes = eps,
                            URL = seasonEntry.URL,
                            IsOnWatchingList = false,
                            CurrentlyAiring = true,
                            Type = animeInfo.Type
                        });
                    }

                    if (!File.Exists(localImagePath))
                    {
                        client.DownloadFile(animeInfo.ImageURL, localImagePath); 
                    }

                }
            }

            // detailed anime list updated successfully
            return true;
        }

        /// <summary>
        /// Parses day and time from the broadcast string, converts it to the local time zone and saves to the LocalBroadcastTime property of the MiruAnimeModel.
        /// </summary>
        /// <param name="detailedAnimeList">List of airing animes with broadcast strings but without parsed day and time data.</param>
        /// <returns>List of airing animes with parsed data saved in LocalBroadcastTime properties.</returns>
        public List<MiruAnimeModel> ParseTimeFromBroadcast(List<MiruAnimeModel> detailedAnimeList)
        {
            // local variables
            string dayOfTheWeek;
            string[] broadcastWords;
            DateTime broadcastTime;
            DateTime time;

            // deserialize data from senpai as a backup source of anime broadcast time
            var senpaiEntries = JsonConvert.DeserializeObject<SenpaiEntryModel>(File.ReadAllText(Constants.SenpaiFilePath));
            var senpaiIDs = senpaiEntries.Items.Select(x => x.MALID);

            // for each airingAnime parse time and day of the week from the broadcast string
            foreach (var airingAnime in detailedAnimeList)
            {
                if (senpaiIDs.Contains(airingAnime.MalId))
                {
                    airingAnime.Broadcast = senpaiEntries.Items.First(x => x.MALID == airingAnime.MalId).airdate;
                    broadcastTime = DateTime.Parse(airingAnime.Broadcast);
                }
                else if (DateTime.TryParse(airingAnime.Broadcast, out DateTime parsedBroadcast))
                {
                        broadcastTime = parsedBroadcast;
                }
                else
                {
                    // split the broadcast string into words
                    broadcastWords = airingAnime.Broadcast.Split(' ');

                    if(broadcastWords.Length < 2)
                    {
                        continue;
                    }

                    // set the first word of the broadcast string as a day of the week
                    dayOfTheWeek = broadcastWords[0];

                    // parse time from the 2nd broadcast string word
                    if(!DateTime.TryParse(broadcastWords[2], out time))
                    {
                        continue;
                    }

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
                }

                // save JST broadcast time parsed from the Broadcast string
                airingAnime.JSTBroadcastTime = broadcastTime;

                // save JST broadcast time converted to your computer's local time to the model's property
                airingAnime.ConvertJstBroadcastTimeToSelectedTimeZone(TimeZoneInfo.Local);
            }

            // return list of airing animes with parsed data saved in LocalBroadcastTime properties
            return detailedAnimeList;
        }

        // tries to get the detailed anime information about anime with the given mal id, retries after given delay until the internet connection is working
        public async Task<Anime> TryToGetAnimeInfo(long malId, int millisecondsDelay)
        {
            Anime output = null;

            // if there is no response from API wait for the given time and retry
            while (output == null)
            {
                try
                {
                    // get detailed anime info from the jikan API
                    output = await JikanWrapper.GetAnime(malId);
                }
                catch (System.Net.Http.HttpRequestException)
                {
                    throw new NoInternetConnectionException("No internet connection");
                }
                catch (JikanDotNet.Exceptions.JikanRequestException)
                {
                    await Task.Delay(millisecondsDelay);
                }
                finally
                {
                    await Task.Delay(millisecondsDelay);
                }
            }
            return output;
        }
    }
}