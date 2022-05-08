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
