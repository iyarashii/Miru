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
		private string _typedInUsername;
		private string _syncStatusText = "Not synced.";
		private string _appStatusText;
		private SortedAnimeListEntries _sortedAnimeListEntries = new SortedAnimeListEntries();
		private MiruAppStatus _appStatus;


		// constructor
		public ShellViewModel()
		{
			AppStatus = MiruAppStatus.Loading;
			using (var db = new MiruDbContext())
			{
				if (db.SyncedMyAnimeListUsers.Any())
				{
					SyncDate = db.SyncedMyAnimeListUsers.FirstOrDefault().SyncTime;
					SyncStatusText = TypedInUsername = db.SyncedMyAnimeListUsers.FirstOrDefault().Username;
					var airingAnimeList = db.MiruAiringAnimeModels.ToList();
					
					SortedAnimeListEntries.MondayAiringAnimeList = airingAnimeList.Where(a => a.LocalBroadcastTime.Value.DayOfWeek == DayOfWeek.Monday).OrderBy(s => s.LocalBroadcastTime).ToList();
					SortedAnimeListEntries.TuesdayAiringAnimeList = airingAnimeList.Where(a => a.LocalBroadcastTime.Value.DayOfWeek == DayOfWeek.Tuesday).OrderBy(s => s.LocalBroadcastTime).ToList();
					SortedAnimeListEntries.WednesdayAiringAnimeList = airingAnimeList.Where(a => a.LocalBroadcastTime.Value.DayOfWeek == DayOfWeek.Wednesday).OrderBy(s => s.LocalBroadcastTime).ToList();
					SortedAnimeListEntries.ThursdayAiringAnimeList = airingAnimeList.Where(a => a.LocalBroadcastTime.Value.DayOfWeek == DayOfWeek.Thursday).OrderBy(s => s.LocalBroadcastTime).ToList();
					SortedAnimeListEntries.FridayAiringAnimeList = airingAnimeList.Where(a => a.LocalBroadcastTime.Value.DayOfWeek == DayOfWeek.Friday).OrderBy(s => s.LocalBroadcastTime).ToList();
					SortedAnimeListEntries.SaturdayAiringAnimeList = airingAnimeList.Where(a => a.LocalBroadcastTime.Value.DayOfWeek == DayOfWeek.Saturday).OrderBy(s => s.LocalBroadcastTime).ToList();
					SortedAnimeListEntries.SundayAiringAnimeList = airingAnimeList.Where(a => a.LocalBroadcastTime.Value.DayOfWeek == DayOfWeek.Sunday).OrderBy(s => s.LocalBroadcastTime).ToList();
				}
			}
			AppStatus = MiruAppStatus.Idle;
		}

        #region properties
        UserAnimeList CurrentUserAnimeList { get; set; }
		DateTime SyncDate { get; set; }

		public SortedAnimeListEntries SortedAnimeListEntries
		{
			get { return _sortedAnimeListEntries; }
			set 
			{ 
				_sortedAnimeListEntries = value;
				NotifyOfPropertyChange(() => SortedAnimeListEntries);
			}
		}

		public string AppStatusText
		{
			get { return _appStatusText; }
			set
			{
				_appStatusText = $"Miru -- { value }";
				NotifyOfPropertyChange(() => AppStatusText);
			}
		}

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
				//NotifyOfPropertyChange(() => SyncStatusText);
			}
		}


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
		
		// method that checks whether sync button should be enabled (wired up by caliburn micro)
		public bool CanSyncUserAnimeList(string typedInUsername)
		{
			if (string.IsNullOrWhiteSpace(typedInUsername) || typedInUsername.Length < 2 || typedInUsername.Length > 16)
			{
				return false;
			}
			return true;
		}


		public async Task SyncUserAnimeList(string typedInUsername)
		{
			//AppStatus = MiruAppStatus.CheckingInternetConnection;
			if (!await CheckInternetConnection()) return;

			//AppStatus = MiruAppStatus.Syncing;
			var getUserAnimeListTask = Constants.jikan.GetUserAnimeList(TypedInUsername, UserAnimeListExtension.Watching);
			CurrentUserAnimeList = await getUserAnimeListTask;
			while (CurrentUserAnimeList == null)
			{
				await Task.Delay(1000);
				if (!await CheckInternetConnection()) return;
				CurrentUserAnimeList = await Constants.jikan.GetUserAnimeList(TypedInUsername, UserAnimeListExtension.Watching);
			}
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

				db.AnimeListEntries.AddRange(CurrentUserAnimeList.Anime);

				// if syncedusers table is not empty then delete all rows
				if (db.SyncedMyAnimeListUsers.Any())
				{
					db.Database.ExecuteSqlCommand("TRUNCATE TABLE [SyncedMyAnimeListUsers]");
				}
				db.SyncedMyAnimeListUsers.Add(new SyncedMyAnimeListUser { Username = TypedInUsername, SyncTime = SyncDate = DateTime.Now });

				await db.SaveChangesAsync();


				await GetAiringAnimeBroadcastTimes(db, CurrentUserAnimeList.Anime);
			}

			SyncStatusText = TypedInUsername;
			//AppStatusText = "Idle";
			AppStatus = MiruAppStatus.Idle;
		}

		public async Task GetAiringAnimeBroadcastTimes(MiruDbContext db, ICollection<AnimeListEntry> animeListEntries)
		{
			if (db.MiruAiringAnimeModels.Any())
			{
				db.Database.ExecuteSqlCommand("TRUNCATE TABLE [AnimeAiringTimes]");
			}

			List<MiruAiringAnimeModel> animeAiringTimes = new List<MiruAiringAnimeModel>();
			Anime animeInfo;
			foreach (var animeListEntry in animeListEntries.Where(a => a.AiringStatus == AiringStatus.Airing))
			{
				animeInfo = await Constants.jikan.GetAnime(animeListEntry.MalId);

				while (animeInfo == null)
				{
					await Task.Delay(1100);
					if (!await CheckInternetConnection()) return;
					animeInfo = await Constants.jikan.GetAnime(animeListEntry.MalId);
				}

				animeAiringTimes.Add(new MiruAiringAnimeModel { MalId = animeInfo.MalId, Broadcast = animeInfo.Broadcast, 
					Title = animeInfo.Title, ImageURL = animeInfo.ImageURL, 
					TotalEpisodes = animeListEntry.TotalEpisodes, URL = animeListEntry.URL, WatchedEpisodes = animeListEntry.WatchedEpisodes });
			}

			// parse day and time from broadcast string
			animeAiringTimes = ParseTimeFromBroadcast(animeAiringTimes);

			db.MiruAiringAnimeModels.AddRange(animeAiringTimes);
			await db.SaveChangesAsync();
		}

		public List<MiruAiringAnimeModel> ParseTimeFromBroadcast(List<MiruAiringAnimeModel> animeAiringTimes)
		{
			// local variables
			List<MiruAiringAnimeModel> parsedData;
			string dayOfTheWeek;
			string[] broadcastWords;
			DateTime broadcastTime;
			DateTime time;
			DateTime localBroadcastTime;
			//ICollection<AnimeAiringTime> airingAnimeWithoutBroadcast = new List<AnimeAiringTime>();
			animeAiringTimes.RemoveAll(x => string.IsNullOrWhiteSpace(x.Broadcast));
			// for each animeentry model parse time and day of the week from broadcast string
			foreach (var animeAiringTime in animeAiringTimes)
			{
				broadcastWords = animeAiringTime.Broadcast.Split(' ');
				dayOfTheWeek = broadcastWords[0];
				time = DateTime.Parse(broadcastWords[2]);
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

				animeAiringTime.LocalBroadcastTime = localBroadcastTime;
			}
			parsedData = animeAiringTimes;
			SortedAnimeListEntries.MondayAiringAnimeList = parsedData.Where(a => a.LocalBroadcastTime.Value.DayOfWeek == DayOfWeek.Monday).OrderBy(s => s.LocalBroadcastTime).ToList();
			SortedAnimeListEntries.TuesdayAiringAnimeList = parsedData.Where(a => a.LocalBroadcastTime.Value.DayOfWeek == DayOfWeek.Tuesday).OrderBy(s => s.LocalBroadcastTime).ToList();
			SortedAnimeListEntries.WednesdayAiringAnimeList = parsedData.Where(a => a.LocalBroadcastTime.Value.DayOfWeek == DayOfWeek.Wednesday).OrderBy(s => s.LocalBroadcastTime).ToList();
			SortedAnimeListEntries.ThursdayAiringAnimeList = parsedData.Where(a => a.LocalBroadcastTime.Value.DayOfWeek == DayOfWeek.Thursday).OrderBy(s => s.LocalBroadcastTime).ToList();
			SortedAnimeListEntries.FridayAiringAnimeList = parsedData.Where(a => a.LocalBroadcastTime.Value.DayOfWeek == DayOfWeek.Friday).OrderBy(s => s.LocalBroadcastTime).ToList();
			SortedAnimeListEntries.SaturdayAiringAnimeList = parsedData.Where(a => a.LocalBroadcastTime.Value.DayOfWeek == DayOfWeek.Saturday).OrderBy(s => s.LocalBroadcastTime).ToList();
			SortedAnimeListEntries.SundayAiringAnimeList = parsedData.Where(a => a.LocalBroadcastTime.Value.DayOfWeek == DayOfWeek.Sunday).OrderBy(s => s.LocalBroadcastTime).ToList();
			return parsedData;
		}

		public void OpenAnimeURL(string URL)
		{
			Process.Start(URL);
		}

		public void CopyAnimeTitleToClipboard(string animeTitle)
		{
			string copyNotification = $"'{ animeTitle }' copied to the clipboard!";
			System.Windows.Clipboard.SetText(animeTitle);
			Constants.notifier.ShowInformation(copyNotification, Constants.messageOptions);
		}

		public async Task<bool> CheckInternetConnection()
		{
			AppStatus = MiruAppStatus.CheckingInternetConnection;
			await InternetConnection.CheckForInternetConnection(AppStatusText);
			AppStatus = InternetConnection.Connection ? MiruAppStatus.Syncing : MiruAppStatus.InternetConnectionProblems;
			return InternetConnection.Connection;
		}
	}
}
