using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JikanDotNet;
using Miru.Data;
using Miru.Models;

namespace Miru.ViewModels
{
    public class ShellViewModel : Screen
    {
		private string _typedInUsername;
		private string _syncStatusText = "Not synced.";
		private string _appStatusText = "Miru -- Idle";


		// constructor
		public ShellViewModel()
		{
			using (var db = new MiruDbContext())
			{
				if (db.SyncedMyAnimeListUsers.Any())
				{
					SyncDate = db.SyncedMyAnimeListUsers.FirstOrDefault().SyncTime;
					SyncStatusText = db.SyncedMyAnimeListUsers.FirstOrDefault().Username;
				}
			}
		}

        #region properties
        UserAnimeList CurrentUserAnimeList { get; set; }
		DateTime SyncDate { get; set; }
		public string AppStatusText
		{
			get { return _appStatusText; }
			set 
			{ 
				_appStatusText = $"Miru -- { value }";
				NotifyOfPropertyChange(() => AppStatusText);
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
				_syncStatusText = $"Synced to the { value }'s anime list. On { SyncDate }";
				NotifyOfPropertyChange(() => SyncStatusText);
			}
		}
        #endregion

        public async Task SyncUserAnimeList()
		{
			AppStatusText = "Syncing...";
			var getUserAnimeListTask = Constants.jikan.GetUserAnimeList(TypedInUsername, UserAnimeListExtension.Watching);
			CurrentUserAnimeList = await getUserAnimeListTask;
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
				//SyncDate = db.SyncedMyAnimeListUsers.FirstOrDefault().SyncTime;
			}
			SyncStatusText = TypedInUsername;
			AppStatusText = "Idle";
		}

		public async Task GetAiringAnimeBroadcastTimes(MiruDbContext db, ICollection<AnimeListEntry> animeListEntries)
		{
			if (db.AnimeAiringTimes.Any())
			{
				db.Database.ExecuteSqlCommand("TRUNCATE TABLE [AnimeAiringTimes]");
			}

			ICollection<AnimeAiringTime> animeAiringTimes = new List<AnimeAiringTime>();
			Anime animeInfo;
			foreach (var animeListEntry in animeListEntries.Where(a => a.AiringStatus == AiringStatus.Airing))
			{
				animeInfo = await Constants.jikan.GetAnime(animeListEntry.MalId);

				while (animeInfo == null)
				{
					await Task.Delay(500);
					animeInfo = await Constants.jikan.GetAnime(animeListEntry.MalId);
				}

				animeAiringTimes.Add(new AnimeAiringTime { MalId = animeInfo.MalId, Broadcast = animeInfo.Broadcast, Title = animeInfo.Title });
			}
			db.AnimeAiringTimes.AddRange(animeAiringTimes);
			await db.SaveChangesAsync();
		}

	}
}
