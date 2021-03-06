﻿using MiruDatabaseLogicLayer;
using MiruLibrary;
using ModernWpf;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace Miru.ViewModels
{
    public interface IShellViewModel
    {
        MiruAppStatus AppStatus { get; }
        string AppStatusText { get; }
        bool CanChangeDisplayedAnimeList { get; set; }
        ISimpleContentDialog ContentDialog { get; }
        ApplicationTheme CurrentApplicationTheme { get; set; }
        SolidColorBrush DaysOfTheWeekBrush { get; set; }
        IMiruDbService DbService { get; }
        bool IsDarkModeOn { get; set; }
        bool IsSynced { get; }
        string MalUserName { get; set; }
        AnimeListType SelectedDisplayedAnimeList { get; set; }
        AnimeType SelectedDisplayedAnimeType { get; set; }
        TimeZoneInfo SelectedTimeZone { get; set; }
        ISortedAnimeListsViewModel SortedAnimeLists { get; set; }
        DateTime SyncDate { get; set; }
        ReadOnlyCollection<TimeZoneInfo> TimeZones { get; }
        string TypedInUsername { get; set; }
        string UserAnimeListURL { get; set; }
        IToastNotifierWrapper ToastNotifierWrapper { get; }
        string CurrentAnimeNameFilter { get; set; }
        double AnimeImageSizeInPixels { get; set; }

        bool CanSyncUserAnimeList(string typedInUsername, MiruAppStatus appStatus, bool syncSeasonList);

        void ChangeTheme();

        Task OpenClearLocalDataDialog();

        void CopyAnimeTitleToClipboard(string animeTitle);

        void UpdateBrushColors();

        void OpenAnimeURL(string URL);

        Task SyncUserAnimeList(string typedInUsername, MiruAppStatus appStatus, bool seasonSyncOn);

        Task UpdateSenpaiData();
        void UpdateAppStatus(MiruAppStatus newAppStatus, string detailedAppStatusDescription = null);
        void ClearAppData();
        Task<bool> GetUserAnimeList();
        Task<bool> GetCurrentSeason();
        Task SaveUserInfo(string username);
        Task<bool> SaveAnimeListData(bool isSeasonSyncOn);
        void UpdateUiAfterDataSync();
    }
}