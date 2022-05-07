using MiruLibrary.Models;

namespace MiruLibrary.Services
{
    public interface IUserDataService
    {
        ICurrentSeasonModel CurrentSeasonModel { get; }
        ICurrentUserAnimeListModel CurrentUserAnimeListModel { get; }
        ISyncedMyAnimeListUser SyncedMyAnimeListUser { get; }
    }
}