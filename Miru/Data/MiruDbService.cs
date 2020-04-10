using JikanDotNet;
using Miru.Models;
using Miru.ViewModels;
using MyInternetConnectionLibrary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Miru.Data
{
    // contains the business logic that uses the local db
    public class MiruDbService : IMiruDbService
    {
        // constructor
        public MiruDbService(ICurrentSeasonModel currentSeasonModel, ICurrentUserAnimeListModel currentUserAnimeListModel, IJikan jikanWrapper)
        {
            // TODO: check if you can somehow set view model context in the constructor
            // dependency injection
            CurrentSeason = currentSeasonModel;
            CurrentUserAnimeList = currentUserAnimeListModel;
            JikanWrapper = jikanWrapper;

            // if there is no local senpai data file get the JSON from senpai.moe
            GetSenpaiData();
        }

        private IJikan JikanWrapper { get; }

        // stores view model's context
        public IShellViewModel ViewModelContext { get; set; }

        // stores data model of the current anime season
        public ICurrentSeasonModel CurrentSeason { get; }

        // stores data model of the currently synced user's anime list
        public ICurrentUserAnimeListModel CurrentUserAnimeList { get; }

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
                    ViewModelContext.MalUserName = ViewModelContext.TypedInUsername = db.SyncedMyAnimeListUsers.FirstOrDefault().Username;

                    // get the user's list of the airing animes from the db
                    var airingAnimeList = db.MiruAiringAnimeModels.ToList();

                    // set airing anime list entries for each day of the week
                    ViewModelContext.SortedAnimeListEntries.SortAiringAnime(airingAnimeList, AnimeListType.AiringAndWatching);
                }
            }
        }

        // clears anime models and synced users tables
        public void ClearDb()
        {
            using (var db = new MiruDbContext())
            {
                db.Database.ExecuteSqlCommand("TRUNCATE TABLE [MiruAiringAnimeModels]");
                db.Database.ExecuteSqlCommand("TRUNCATE TABLE [SyncedMyAnimeListUsers]");
            }
        }

        public void ClearLocalImageCache()
        {
            DirectoryInfo di = new DirectoryInfo(Constants.ImageCacheFolderPath);
            foreach (FileInfo file in di.EnumerateFiles())
            {
                file.Delete();
            }
        }

        // updates senpai.json data file which is used as a backup for anime airing time
        public void UpdateSenpaiData()
        {
            if (File.Exists(Constants.SenpaiFilePath))
            {
                File.Delete(Constants.SenpaiFilePath);
                GetSenpaiData();
            }
            else
            {
                GetSenpaiData();
            }
        }

        // gets the data from senpai.moe in a JSON form
        public void GetSenpaiData()
        {
            if (File.Exists(Constants.SenpaiFilePath))
            {
                return;
            }

            // serialize JSON directly to a file
            using (StreamWriter file = File.CreateText(Constants.SenpaiFilePath))
            {
                // get only MALID and airing_date json properties
                var deserializedSenpaiData = JsonConvert.DeserializeObject<SenpaiEntryModel>(InternetConnection.client.GetStringAsync(Constants.SenpaiDataSourceURL).Result);
                file.Write(JsonConvert.SerializeObject(deserializedSenpaiData, Formatting.Indented));
            }
        }

        // changes data for the displayed anime list to match time zone and anime list type passed as parameters
        public void ChangeDisplayedAnimeList(AnimeListType animeListType, TimeZoneInfo selectedTimeZone, AnimeType selectedAnimeType, string animeNameFilter)
        {
            using (var db = new MiruDbContext())
            {
                // get the user's list of the airing animes from the db
                var airingAnimeList = db.MiruAiringAnimeModels.ToList();

                // filter list of the airing animes depending on type
                switch (selectedAnimeType)
                {
                    case AnimeType.Both:
                        airingAnimeList.RemoveAll(x => x.Type != "TV" && x.Type != "ONA");
                        break;

                    case AnimeType.TV:
                        airingAnimeList.RemoveAll(x => x.Type != "TV");
                        break;

                    case AnimeType.ONA:
                        airingAnimeList.RemoveAll(x => x.Type != "ONA");
                        break;
                }

                if (!string.IsNullOrWhiteSpace(animeNameFilter))
                {
                    airingAnimeList.RemoveAll(x => !(x.Title.IndexOf(animeNameFilter, StringComparison.OrdinalIgnoreCase) >= 0));
                }

                foreach (var airingAnime in airingAnimeList)
                {
                    // save JST broadcast time converted to the selected timezone as local broadcast time
                    airingAnime.LocalBroadcastTime = ConvertJstBroadcastTimeToSelectedTimeZone(airingAnime.JSTBroadcastTime.Value, selectedTimeZone);
                }

                // set airing anime list entries for each day of the week
                ViewModelContext.SortedAnimeListEntries.SortAiringAnime(airingAnimeList, animeListType);
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
                ViewModelContext.UpdateAppStatus(MiruAppStatus.Busy, "Getting detailed user anime list data...");

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
                    ViewModelContext.UpdateAppStatus(MiruAppStatus.Busy, "Getting detailed current season anime data...");

                    // update user anime list with season anime detailed data if it fails return false
                    if (!await GetDetailedSeasonAnimeListInfo(detailedAnimeList))
                    {
                        return false;
                    }
                }
                ViewModelContext.UpdateAppStatus(MiruAppStatus.Busy, "Parse day and time from the broadcast string...");

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
            DirectoryInfo di = new DirectoryInfo(Constants.ImageCacheFolderPath);
            if (!di.Exists)
            {
                di.Create();
            }

            // get anime data from the db
            var detailedUserAnimeList = db.MiruAiringAnimeModels.ToList();

            // set IsOnWatchingList flag to false for all anime models in the db
            detailedUserAnimeList.ForEach(x => { x.IsOnWatchingList = false; x.WatchedEpisodes = 0; });

            // get mal ids of the anime models that were in the db
            var malIdsFromDb = new HashSet<long>(detailedUserAnimeList.Select(x => x.MalId));

            using (WebClient client = new WebClient())
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
                        string localImagePath = Path.Combine(Constants.ImageCacheFolderPath, $"{ animeInfo.MalId }.jpg");
                        // add airing anime created from the animeInfo data to the airingAnimes list
                        detailedUserAnimeList.Add(new MiruAiringAnimeModel
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
        public async Task<bool> GetDetailedSeasonAnimeListInfo(List<MiruAiringAnimeModel> detailedUserAnimeList)
        {
            // set of all anime ids from the db
            HashSet<long> airingAnimesMalIDs = new HashSet<long>(detailedUserAnimeList.Select(x => x.MalId));

            // list of anime entries in the current season
            var currentSeasonList = CurrentSeason.SeasonData.SeasonEntries.ToList();

            // remove anime entries with types other than 'TV' from the list
            currentSeasonList.RemoveAll(x => x.Type != "TV" && x.Type != "ONA");

            // remove anime entries marked as 'for kids' from the list
            currentSeasonList.RemoveAll(x => x.Kids == true);

            using (WebClient client = new WebClient())
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
                    if (airingAnimesMalIDs.Contains(seasonEntry.MalId))
                    {
                        var modelToBeUpdated = detailedUserAnimeList.FirstOrDefault(x => x.MalId == seasonEntry.MalId);
                        modelToBeUpdated.CurrentlyAiring = true;
                    }
                    else
                    {
                        string localImagePath = Path.Combine(Constants.ImageCacheFolderPath, $"{ animeInfo.MalId }.jpg");

                        // add airing anime created from the animeInfo data to the miruAnimeModelsList
                        detailedUserAnimeList.Add(new MiruAiringAnimeModel
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
                        client.DownloadFile(animeInfo.ImageURL, localImagePath);
                    }
                } 
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

            // deserialize data from senpai as a backup source of anime broadcast time
            var senpaiEntries = JsonConvert.DeserializeObject<SenpaiEntryModel>(File.ReadAllText(Constants.SenpaiFilePath));
            var senpaiIDs = senpaiEntries.Items.Select(x => x.MALID);

            // remove airing animes without specified broadcast time (like OVAs)
            detailedAnimeList.RemoveAll(x => !string.IsNullOrWhiteSpace(x.Broadcast) && (x.Broadcast.Contains("Unknown") || x.Broadcast.Contains("Not scheduled")));

            // for each airingAnime parse time and day of the week from the broadcast string
            foreach (var airingAnime in detailedAnimeList)
            {
                if (DateTime.TryParse(airingAnime.Broadcast, out DateTime parsedBroadcast))
                {
                    if (senpaiIDs.Contains(airingAnime.MalId))
                    {
                        airingAnime.Broadcast = senpaiEntries.Items.First(x => x.MALID == airingAnime.MalId).airdate;
                        broadcastTime = DateTime.Parse(airingAnime.Broadcast);
                    }
                    else
                    {
                        broadcastTime = parsedBroadcast;
                    }
                }
                else
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
                }

                // save JST broadcast time parsed from the Broadcast string
                airingAnime.JSTBroadcastTime = broadcastTime;

                // save JST broadcast time converted to your computer's local time to the model's property
                airingAnime.LocalBroadcastTime = ConvertJstBroadcastTimeToSelectedTimeZone(broadcastTime, TimeZoneInfo.Local);
            }

            // return list of airing animes with parsed data saved in LocalBroadcastTime properties
            return detailedAnimeList;
        }

        private DateTime ConvertJstBroadcastTimeToSelectedTimeZone(DateTime broadcastTime, TimeZoneInfo selectedTimeZone)
        {
            // covert JST to UTC
            var broadcastTimeInSelectedTimeZone = TimeZoneInfo.ConvertTimeToUtc(broadcastTime, TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time"));

            // get the UTC offset for the selected time zone
            var utcOffset = selectedTimeZone.GetUtcOffset(DateTime.UtcNow);

            // return Japanese broadcast time converted to selected time zone
            return broadcastTimeInSelectedTimeZone.Add(utcOffset);
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