using MiruLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiruLibrary.Services
{
    public class UserDataService : IUserDataService
    {
        public UserDataService(ICurrentSeasonModel currentSeasonModel,
            ICurrentUserAnimeListModel currentUserAnimeListModel, ISyncedMyAnimeListUser syncedMyAnimeListUser)
        {
            CurrentSeasonModel = currentSeasonModel;
            CurrentUserAnimeListModel = currentUserAnimeListModel;
            SyncedMyAnimeListUser = syncedMyAnimeListUser;
        }

        public ICurrentSeasonModel CurrentSeasonModel { get; }
        public ICurrentUserAnimeListModel CurrentUserAnimeListModel { get; }
        public ISyncedMyAnimeListUser SyncedMyAnimeListUser { get; }
    }
}
