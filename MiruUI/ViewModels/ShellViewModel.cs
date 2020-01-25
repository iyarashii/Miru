using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JikanDotNet;
using Miru.Data;

namespace Miru.ViewModels
{
    public class ShellViewModel : Screen
    {
		private string _userName;
		private string _syncStatusText = "Not synced.";

		UserAnimeList CurrentUserAnimeList { get; set; }

		public string UserName
		{
			get 
			{ 
				
				return _userName; 
			}
			set
			{
				_userName = value;
				NotifyOfPropertyChange(() => UserName);
				NotifyOfPropertyChange(() => SyncStatusText);
			}
		}


		public string SyncStatusText
		{
			get { return _syncStatusText; }
			set 
			{ 
				_syncStatusText = value;
				NotifyOfPropertyChange(() => SyncStatusText);
			}
		}
			
		//public string SyncStatusText { get; set; } = "Not synced.";
		//{
		//	get 
		//	{
		//		if (string.IsNullOrWhiteSpace(_userName))
		//		{
		//			return "Not synced";
		//		}
		//		return $"Synced to the { _userName }'s anime list."; 
		//	}
		//}

		public async Task SyncUserAnimeList()
		{
			var getUserAnimeListTask = Constants.jikan.GetUserAnimeList(UserName, UserAnimeListExtension.Watching);
			CurrentUserAnimeList = await getUserAnimeListTask;
			SyncStatusText = $"Synced to the { _userName }'s anime list.";
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
				await db.SaveChangesAsync();
			}
		}

	}
}
