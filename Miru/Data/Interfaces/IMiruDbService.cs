using JikanDotNet;
using Miru.Models;
using Miru.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Miru.Data
{
    public interface IMiruDbService
    {
        ICurrentSeasonModel CurrentSeason { get; }
        ICurrentUserAnimeListModel CurrentUserAnimeList { get; }
        IShellViewModel ViewModelContext { get; set; }

        void ChangeDisplayedAnimeList(AnimeListType animeListType, TimeZoneInfo selectedTimeZone, AnimeType selectedAnimeType);
        void ClearDb();
        Task<bool> GetDetailedSeasonAnimeListInfo(List<MiruAiringAnimeModel> detailedUserAnimeList);
        Task<List<MiruAiringAnimeModel>> GetDetailedUserAnimeList(MiruDbContext db, ICollection<AnimeListEntry> currentUserAnimeListEntries);
        void GetSenpaiData();
        void LoadLastSyncedData();
        List<MiruAiringAnimeModel> ParseTimeFromBroadcast(List<MiruAiringAnimeModel> detailedAnimeList);
        Task<bool> SaveDetailedAnimeListData(bool seasonSyncOn);
        Task SaveSyncedUserData();
        Task<Anime> TryToGetAnimeInfo(long malId, int millisecondsDelay);
        void UpdateSenpaiData();
    }
}