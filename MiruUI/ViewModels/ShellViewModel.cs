using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JikanDotNet;
using Miru.Data;
using Miru.Models;
using System.Globalization;
using System.Diagnostics;
using ToastNotifications.Messages;
using MyInternetConnectionLibrary;

namespace Miru.ViewModels
{
    public class ShellViewModel : Screen
    {
		// private fields that are used with properties in this class
		private string _typedInUsername;
		private string _syncStatusText = "Not synced.";
		private string _appStatusText;
		private SortedAnimeListEntries _sortedAnimeListEntries = new SortedAnimeListEntries();
		private MiruAppStatus _appStatus;


		// constructor
		public ShellViewModel()
		{
			// open temporary connection to the database
			using (var db = new MiruDbContext())
			{
				// if SyncedMyAnimeListUsers table is not empty
				if (db.SyncedMyAnimeListUsers.Any())
				{
					// set SyncDate prop to the sync time of the last synchronization
					SyncDate = db.SyncedMyAnimeListUsers.FirstOrDefault().SyncTime;

					// set SyncStatusText and TypedInUsername props to the username of the last synchronized user
					SyncStatusText = TypedInUsername = db.SyncedMyAnimeListUsers.FirstOrDefault().Username;

					// get the user's list of the airing animes from the db
					var airingAnimeList = db.MiruAiringAnimeModels.ToList();

					// set airing anime list entries for each day of the week
					SortAiringAnime(airingAnimeList);
				}
			}

			// set default app status
			AppStatus = MiruAppStatus.Idle;
		}

        #region properties

		// stores anime list of the currently synced user
        UserAnimeList CurrentUserAnimeList { get; set; }

		// stores last sync date
		DateTime SyncDate { get; set; }

		// stores anime models sorted for each day of the week
		public SortedAnimeListEntries SortedAnimeListEntries
		{
			get { return _sortedAnimeListEntries; }
			set 
			{ 
				_sortedAnimeListEntries = value;
				NotifyOfPropertyChange(() => SortedAnimeListEntries);
			}
		}

		// stores app status text which is displayed as app window name
		public string AppStatusText
		{
			get { return _appStatusText; }
			set
			{
				_appStatusText = $"Miru -- { value }";
				NotifyOfPropertyChange(() => AppStatusText);
			}
		}

		// stores app status and sets correct display text for the app window
		public MiruAppStatus AppStatus
		{
			get { return _appStatus; }
			set 
			{ 
				_appStatus = value;
				switch (value)
				{
					case MiruAppStatus.Idle:
						AppStatusText = "Idle";
						break;
					case MiruAppStatus.CheckingInternetConnection:
						AppStatusText = "Checking internet connection...";
						break;
					case MiruAppStatus.Syncing:
						AppStatusText = "Syncing...";
						break;
					case MiruAppStatus.InternetConnectionProblems:
						AppStatusText = "Problems with internet connection!";
						break;
					case MiruAppStatus.Loading:
						AppStatusText = "Loading data from the last synchronization...";
						break;
				}
			}
		}

		// stores username that is typed in the textbox at the moment
		public string TypedInUsername
		{
			get 
			{ 
				
				return _typedInUsername; 
			}
			set
			{
				_typedInUsername = value;
				NotifyOfPropertyChange(() => TypedInUsername);
			}
		}

		// stores text that says the username of the last synced user and time of the sync
		public string SyncStatusText
		{
			get { return _syncStatusText; }
			set 
			{ 
				_syncStatusText = $"Synced to the { value }'s anime list on { SyncDate }";
				NotifyOfPropertyChange(() => SyncStatusText);
			}
		}
        #endregion
		
		// checks whether sync button should be enabled (wired up by caliburn micro)
		public bool CanSyncUserAnimeList(string typedInUsername)
		{
			if (string.IsNullOrWhiteSpace(typedInUsername) || typedInUsername.Length < 2 || typedInUsername.Length > 16)
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// Performs synchronization to the typed-in user's anime list on a button click (wired up via caliburn micro).
		/// </summary>
		/// <param name="typedInUsername"></param>
		/// <returns></returns>
		public async Task SyncUserAnimeList(string typedInUsername)
		{
			// stop method execution if there is a problem with internet connection
			if (!await CheckInternetConnection()) return;

			// get user's watching status anime list
			CurrentUserAnimeList = await Constants.jikan.GetUserAnimeList(TypedInUsername, UserAnimeListExtension.Watching);

			// if there is no response from API wait 1 second and retry
			while (CurrentUserAnimeList == null)
			{
				await Task.Delay(1000);
				if (!await CheckInternetConnection()) return;
				CurrentUserAnimeList = await Constants.jikan.GetUserAnimeList(TypedInUsername, UserAnimeListExtension.Watching);
			}

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
				db.SyncedMyAnimeListUsers.Add(new SyncedMyAnimeListUser { Username = TypedInUsername, SyncTime = SyncDate = DateTime.Now });

				// save changes to the database
				await db.SaveChangesAsync();

				// get the anime broadcast times, convert them to the GMT+1 timezone and save them to the database
				await GetAiringAnimeBroadcastTimes(db, CurrentUserAnimeList.Anime);
			}

			// update displayed username and sync date 
			SyncStatusText = TypedInUsername;

			// update app status
			AppStatus = MiruAppStatus.Idle;
		}

		/// <summary>
		/// Gets detailed anime info for each AnimeListEntry in the collection and saves it as 
		/// MiruAiringAnimeModels that contain the local broadcast time and the number of watched episodes by the user.
		/// </summary>
		/// <param name="db">Context of the database that is going to be updated.</param>
		/// <param name="animeListEntries">Collection of AnimeListEntries that are going to receive broadcast time data.</param>
		/// <returns></returns>
		public async Task GetAiringAnimeBroadcastTimes(MiruDbContext db, ICollection<AnimeListEntry> animeListEntries)
		{
			// clear MiruAiringAnimeModels table from any data
			if (db.MiruAiringAnimeModels.Any())
			{
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
					if (!await CheckInternetConnection()) return;
					animeInfo = await Constants.jikan.GetAnime(animeListEntry.MalId);
				}

				// add airing anime created from the animeInfo data to the airingAnimes list
				airingAnimes.Add(new MiruAiringAnimeModel { MalId = animeInfo.MalId, Broadcast = animeInfo.Broadcast, 
					Title = animeInfo.Title, ImageURL = animeInfo.ImageURL, 
					TotalEpisodes = animeListEntry.TotalEpisodes, URL = animeListEntry.URL, WatchedEpisodes = animeListEntry.WatchedEpisodes });
			}

			// parse day and time from broadcast string
			airingAnimes = ParseTimeFromBroadcast(airingAnimes);

			// add airingAnimes list to the MiruAiringAnimeModels table
			db.MiruAiringAnimeModels.AddRange(airingAnimes);

			// update database
			await db.SaveChangesAsync();
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

				// convert JST to GMT+1 time
				localBroadcastTime = broadcastTime.AddHours(-8);

				// save parsed date and time to the model's property
				airingAnime.LocalBroadcastTime = localBroadcastTime;
			}

			// set airing anime list entries for each day of the week
			SortAiringAnime(airingAnimes);

			// return list of airing animes with parsed data saved in LocalBroadcastTime properties
			return airingAnimes;
		}

		// opens MAL anime page
		public void OpenAnimeURL(string URL)
		{
			Process.Start(URL);
		}

		// saves anime title to the clipboard and shows notification describing this action
		public void CopyAnimeTitleToClipboard(string animeTitle)
		{
			string copyNotification = $"'{ animeTitle }' copied to the clipboard!";
			System.Windows.Clipboard.SetText(animeTitle);
			Constants.notifier.ShowInformation(copyNotification, Constants.messageOptions);
		}

		// checks internet connection and sets the app status accordingly
		public async Task<bool> CheckInternetConnection()
		{
			AppStatus = MiruAppStatus.CheckingInternetConnection;
			await InternetConnection.CheckForInternetConnection(AppStatusText);
			AppStatus = InternetConnection.Connection ? MiruAppStatus.Syncing : MiruAppStatus.InternetConnectionProblems;
			return InternetConnection.Connection;
		}

		// orders the airing anime list entries by the days
		public void SortAiringAnime(List<MiruAiringAnimeModel> airingAnimeModels)
		{
			SortedAnimeListEntries.MondayAiringAnimeList = airingAnimeModels.Where(a => a.LocalBroadcastTime.Value.DayOfWeek == DayOfWeek.Monday).OrderBy(s => s.LocalBroadcastTime).ToList();
			SortedAnimeListEntries.TuesdayAiringAnimeList = airingAnimeModels.Where(a => a.LocalBroadcastTime.Value.DayOfWeek == DayOfWeek.Tuesday).OrderBy(s => s.LocalBroadcastTime).ToList();
			SortedAnimeListEntries.WednesdayAiringAnimeList = airingAnimeModels.Where(a => a.LocalBroadcastTime.Value.DayOfWeek == DayOfWeek.Wednesday).OrderBy(s => s.LocalBroadcastTime).ToList();
			SortedAnimeListEntries.ThursdayAiringAnimeList = airingAnimeModels.Where(a => a.LocalBroadcastTime.Value.DayOfWeek == DayOfWeek.Thursday).OrderBy(s => s.LocalBroadcastTime).ToList();
			SortedAnimeListEntries.FridayAiringAnimeList = airingAnimeModels.Where(a => a.LocalBroadcastTime.Value.DayOfWeek == DayOfWeek.Friday).OrderBy(s => s.LocalBroadcastTime).ToList();
			SortedAnimeListEntries.SaturdayAiringAnimeList = airingAnimeModels.Where(a => a.LocalBroadcastTime.Value.DayOfWeek == DayOfWeek.Saturday).OrderBy(s => s.LocalBroadcastTime).ToList();
			SortedAnimeListEntries.SundayAiringAnimeList = airingAnimeModels.Where(a => a.LocalBroadcastTime.Value.DayOfWeek == DayOfWeek.Sunday).OrderBy(s => s.LocalBroadcastTime).ToList();
		}
	}
}
