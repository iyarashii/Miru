// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using MiruLibrary.Models;

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
