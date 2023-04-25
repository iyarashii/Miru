﻿// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using Caliburn.Micro;
using MiruDatabaseLogicLayer;
using MiruLibrary;
using MiruLibrary.Models;
using MiruLibrary.Services;
using ModernWpf;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Miru.ViewModels
{
    public class ShellViewModel : Screen, IShellViewModel
    {
        // private fields that are used with properties in this class
        private string _typedInUsername;
        private string _appStatusText;
        private ISortedAnimeListsViewModel _sortedAnimeLists;
        private MiruAppStatus _appStatus;
        private ApplicationTheme _currentApplicationTheme;
        private SolidColorBrush _daysOfTheWeekBrush;
        private bool _canChangeDisplayedAnimeList;
        private AnimeType _selectedDisplayedAnimeType;
        private string _malUserName;
        private string _userAnimeListURL;
        private string _currentAnimeNameFilter;
        private double _animeImageSizeInPixels;
        private double _watchingStatusHighlightOpacity;
        private double _syncProgress;
        private int _totalProgressCount;
        private int _currentProgressCount;

        // fields with default values for properties with setter logic
        private AnimeListType _selectedDisplayedAnimeList = AnimeListType.Watching;
        private TimeZoneInfo _selectedTimeZone = TimeZoneInfo.Local;

        private void ConfigureDbService()
        {
            // subscribe to the events
            DbService.UpdateSyncDate += new EventHandler<DateTime>(UpdateSyncDate);
            DbService.UpdateCurrentUsername += new EventHandler<string>(UpdateUsername);
            DbService.UpdateAnimeListEntriesUI += new MiruDbService.SortedAnimeListEventHandler(SortedAnimeLists.SetAnimeSortedByAirDayOfWeekAndFilteredByGivenAnimeListType);
            DbService.UpdateAppStatusUI += new MiruDbService.UpdateAppStatusEventHandler(UpdateAppStatus);
            DbService.UpdateSyncProgress += new EventHandler<int>(UpdateSyncProgress);
        }

        private void LoadUserSettings(UserSettings userSettings)
        {
            AnimeImageSizeInPixels = userSettings.AnimeImageSize;
            _selectedDisplayedAnimeList = userSettings.DisplayedAnimeListType;
            SelectedDisplayedAnimeType = userSettings.DisplayedAnimeType;
            GetDroppedAnimeData = userSettings.GetDroppedAnimeData;
            WatchingStatusHighlightOpacity = userSettings.WatchingStatusHighlightOpacity;
        }

        private void ConfigureAppColorTheme()
        {
            // apply correct colors to the days of the week depending on windows theme during runtime
            UpdateBrushColors();

            // set app theme to prevent the app to react to windows theme change while the app is running
            ThemeManager.Current.ApplicationTheme = CurrentApplicationTheme;
        }

        public bool CheckSqlLocalDbInstallationPresence()
        {
            string[] installedVersions;
            var sqlLocalDbRegistryKey = SystemService.OpenLocalMachineSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server Local DB\Installed Versions\");
            if (sqlLocalDbRegistryKey != null)
            {
                installedVersions = sqlLocalDbRegistryKey.GetSubKeyNames();
                foreach (var version in installedVersions)
                {
                    if (string.IsNullOrWhiteSpace(version)) continue;
                    if (!float.TryParse(version, NumberStyles.Float, CultureInfo.InvariantCulture, out float parsedVersion)) break;

                    // 13.0 is SQL LocalDB 2016 version
                    if (parsedVersion >= 13.0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        // constructor
        public ShellViewModel(ISortedAnimeListsViewModel sortedAnimeLists,
                              IMiruDbService miruDbService,
                              IFileSystemService fileSystemService,
                              ISimpleContentDialog contentDialog,
                              IToastNotifierWrapper toastNotifierWrapper,
                              UserSettings userSettings,
                              ISystemService systemService)
        {
            #region dependency injection

            _sortedAnimeLists = sortedAnimeLists;
            DbService = miruDbService;
            FileSystemService = fileSystemService;
            ContentDialog = contentDialog;
            ToastNotifierWrapper = toastNotifierWrapper;
            SystemService = systemService;

            #endregion dependency injection

            if (CheckSqlLocalDbInstallationPresence())
            {
                ConfigureDbService();

                // load synced data from the db
                DbService.LoadLastSyncedData();

                LoadUserSettings(userSettings);

                // set default app status
                UpdateAppStatus(MiruAppStatus.Idle);
            }

            ConfigureAppColorTheme();
        }

        #region properties

        public double AnimeImageSizeInPixels
        {
            get { return _animeImageSizeInPixels; }
            set
            {
                if (double.IsNaN(value))
                {
                    _animeImageSizeInPixels = 134.0;
                }
                else
                {
                    _animeImageSizeInPixels = value;
                }
                NotifyOfPropertyChange(() => AnimeImageSizeInPixels);
            }
        }

        public double WatchingStatusHighlightOpacity
        {
            get { return _watchingStatusHighlightOpacity; }
            set
            {
                if (double.IsNaN(value))
                {
                    _watchingStatusHighlightOpacity = 0.66;
                }
                else
                {
                    _watchingStatusHighlightOpacity = value;
                }
                NotifyOfPropertyChange(() => WatchingStatusHighlightOpacity);
            }
        }

        public bool GetDroppedAnimeData { get; set; }
        public ISystemService SystemService { get; }

        public IToastNotifierWrapper ToastNotifierWrapper { get; }

        // content dialog instance
        public ISimpleContentDialog ContentDialog { get; }

        // stores MiruDbService's instance that contains most of the business logic
        public IMiruDbService DbService { get; }
        public IFileSystemService FileSystemService { get; }

        // stores collection of the time zones used by the system
        public ReadOnlyCollection<TimeZoneInfo> TimeZones { get; } = TimeZoneInfo.GetSystemTimeZones();

        // stores last sync date
        public DateTime SyncDate { get; set; }

        // stores dark mode theme toggle value
        public bool IsDarkModeOn { get; set; }

        // stores currently selected anime list display type
        public AnimeListType SelectedDisplayedAnimeList
        {
            get { return _selectedDisplayedAnimeList; }
            set
            {
                _selectedDisplayedAnimeList = value;

                // update displayed animes
                DbService.ChangeDisplayedAnimeList(value, SelectedTimeZone, SelectedDisplayedAnimeType, CurrentAnimeNameFilter);
                NotifyOfPropertyChange(() => SelectedDisplayedAnimeList);
            }
        }

        // stores currently selected anime type "TV", "ONA" etc.
        public AnimeType SelectedDisplayedAnimeType
        {
            get { return _selectedDisplayedAnimeType; }
            set
            {
                _selectedDisplayedAnimeType = value;

                DbService.ChangeDisplayedAnimeList(SelectedDisplayedAnimeList, SelectedTimeZone, value, CurrentAnimeNameFilter);
                NotifyOfPropertyChange(() => SelectedDisplayedAnimeType);
            }
        }

        // stores currently selected time zone
        public TimeZoneInfo SelectedTimeZone
        {
            get { return _selectedTimeZone; }
            set
            {
                _selectedTimeZone = value;

                // update displayed animes
                DbService.ChangeDisplayedAnimeList(SelectedDisplayedAnimeList, value, SelectedDisplayedAnimeType, CurrentAnimeNameFilter);
                NotifyOfPropertyChange(() => SelectedTimeZone);
            }
        }

        public string CurrentAnimeNameFilter
        {
            get { return _currentAnimeNameFilter; }
            set
            {
                _currentAnimeNameFilter = value;
                DbService.ChangeDisplayedAnimeList(SelectedDisplayedAnimeList, SelectedTimeZone, SelectedDisplayedAnimeType, value);
                NotifyOfPropertyChange(() => CurrentAnimeNameFilter);
            }
        }

        // stores color for the days of the week column headers
        public SolidColorBrush DaysOfTheWeekBrush
        {
            get { return _daysOfTheWeekBrush; }
            set
            {
                _daysOfTheWeekBrush = value;
                NotifyOfPropertyChange(() => DaysOfTheWeekBrush);
            }
        }

        // stores currently used UI theme
        public ApplicationTheme CurrentApplicationTheme
        {
            get { return _currentApplicationTheme; }
            set
            {
                _currentApplicationTheme = value;
                IsDarkModeOn = (_currentApplicationTheme == ApplicationTheme.Dark);
                NotifyOfPropertyChange(() => IsDarkModeOn);
                NotifyOfPropertyChange(() => CurrentApplicationTheme);
            }
        }

        // stores anime models sorted for each day of the week
        public ISortedAnimeListsViewModel SortedAnimeLists
        {
            get { return _sortedAnimeLists; }
            set
            {
                _sortedAnimeLists = value;
                NotifyOfPropertyChange(() => SortedAnimeLists);
            }
        }

        // stores app status text which is displayed as app window name
        public string AppStatusText
        {
            get { return _appStatusText; }
            private set
            {
                _appStatusText = value;
                NotifyOfPropertyChange(() => AppStatus);
                NotifyOfPropertyChange(() => AppStatusText);
            }
        }

        // stores app status and sets correct display text for the app window
        public MiruAppStatus AppStatus
        {
            get { return _appStatus; }
            private set
            {
                _appStatus = value;
                switch (value)
                {
                    case MiruAppStatus.Idle:
                        AppStatusText = "Idle";
                        CanChangeDisplayedAnimeList = true;
                        break;

                    case MiruAppStatus.Busy:
                        AppStatusText = "Busy...";
                        CanChangeDisplayedAnimeList = false;
                        break;

                    case MiruAppStatus.InternetConnectionProblems:
                        AppStatusText = "Problems with internet connection!";
                        CanChangeDisplayedAnimeList = true;
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

        // stores text that says the username of the last synced user
        public string MalUserName
        {
            get { return _malUserName; }
            set
            {
                _malUserName = value;
                NotifyOfPropertyChange(() => MalUserName);
                NotifyOfPropertyChange(() => SyncDate);
                NotifyOfPropertyChange(() => IsSynced);
                UserAnimeListURL = value;
            }
        }

        // tells whether there is synced user data
        public bool IsSynced
        {
            get
            {
                return !string.IsNullOrEmpty(MalUserName);
            }
        }

        // stores current user anime list URL
        public string UserAnimeListURL
        {
            get { return _userAnimeListURL; }
            set
            {
                _userAnimeListURL = $@"https://myanimelist.net/animelist/{value}";
                NotifyOfPropertyChange(() => UserAnimeListURL);
            }
        }

        // guard property for the buttons, comboboxes and the text field
        public bool CanChangeDisplayedAnimeList
        {
            get { return _canChangeDisplayedAnimeList; }
            set
            {
                _canChangeDisplayedAnimeList = value;
                NotifyOfPropertyChange(() => CanChangeDisplayedAnimeList);
            }
        }

        public double SyncProgress
        {
            get => _syncProgress;
            set
            {
                _syncProgress = value;
                NotifyOfPropertyChange(() => SyncProgress);
            }                
        }

        public int CurrentProgressCount
        {
            get => _currentProgressCount;
            set
            {
                _currentProgressCount = value;
                NotifyOfPropertyChange(() => CurrentProgressCount);
            }
        }

        public int TotalProgressCount
        {
            get => _totalProgressCount;
            set
            {
                _totalProgressCount = value;
                NotifyOfPropertyChange(() => TotalProgressCount);
            }
        }
        #endregion properties

        #region event handlers and guard methods

        public void UpdateSyncProgress(object sender, int currentCount)
        {
            int totalCount = 0;
            if((sender as string) == nameof(DbService.GetDetailedUserAnimeList))
            {
                totalCount = DbService.CurrentUserAnimeList.UserAnimeListData.Anime.Count;
            }
            else if((sender as string) == "GetDetailedSeasonAnimeListInfo")
            {
                totalCount = DbService.CurrentSeason.GetFilteredSeasonList().Count;
            }
            SyncProgress = (double)currentCount / totalCount * 100;
            CurrentProgressCount = currentCount;
            TotalProgressCount = totalCount;
        }

        public void UpdateSyncDate(object sender, DateTime value)
        {
            SyncDate = value;
        }

        public void UpdateUsername(object sender, string name)
        {
            if (string.IsNullOrEmpty(TypedInUsername))
            {
                MalUserName = TypedInUsername = name;
            }
            else
            {
                MalUserName = name;
            }
        }

        // called by dark mode toggle switch
        public void ChangeTheme()
        {
            ThemeManager.Current.ApplicationTheme = CurrentApplicationTheme == ApplicationTheme.Dark ? ApplicationTheme.Light : ApplicationTheme.Dark;
            UpdateBrushColors();
        }

        // fired on theme change
        public void UpdateBrushColors()
        {
            CurrentApplicationTheme = ThemeManager.Current.ActualApplicationTheme;
            if (CurrentApplicationTheme == ApplicationTheme.Dark)
            {
                DaysOfTheWeekBrush = Brushes.SeaGreen;
            }
            else if (CurrentApplicationTheme == ApplicationTheme.Light)
            {
                DaysOfTheWeekBrush = Brushes.Lime;
            }
            else
            {
                DaysOfTheWeekBrush = Brushes.Red;
            }
        }

        // checks whether sync button should be enabled (wired up by caliburn micro)
        public bool CanSyncUserAnimeList(string typedInUsername, MiruAppStatus appStatus, bool syncSeasonList)
        {
            if (string.IsNullOrWhiteSpace(typedInUsername) ||
                typedInUsername.Length < 2 || typedInUsername.Any(char.IsWhiteSpace) ||
                typedInUsername.Length > 16 ||
                (appStatus != MiruAppStatus.Idle && appStatus != MiruAppStatus.InternetConnectionProblems))
            {
                return false;
            }
            return true;
        }

        public async Task<bool> GetUserAnimeList()
        {
            // get user's watching status anime list
            UpdateAppStatus(MiruAppStatus.Busy, "Getting current user anime list...");

            var getCurrentUserAnimeListResult = await DbService.CurrentUserAnimeList.GetCurrentUserAnimeList(TypedInUsername);

            if (!getCurrentUserAnimeListResult.Success)
            {
                UpdateAppStatus(MiruAppStatus.Idle, getCurrentUserAnimeListResult.ErrorMessage);
                return false;
            }
            return true;
        }

        public async Task<bool> GetUserDroppedAnimeList()
        {
            // get user's dropped status anime list
            UpdateAppStatus(MiruAppStatus.Busy, "Getting current user dropped anime list...");

            var getCurrentUserDroppedAnimeListResult =
                await DbService.CurrentUserAnimeList.GetCurrentUserDroppedAnimeList(TypedInUsername);

            if (!getCurrentUserDroppedAnimeListResult.Success)
            {
                UpdateAppStatus(MiruAppStatus.Idle, getCurrentUserDroppedAnimeListResult.ErrorMessage);
                return false;
            }
            return true;
        }

        public async Task<bool> GetCurrentSeason()
        {
            // get current season
            UpdateAppStatus(MiruAppStatus.Busy, "Getting current season anime list...");

            if (!await DbService.CurrentSeason.GetCurrentSeasonList(2000))
            {
                UpdateAppStatus(MiruAppStatus.InternetConnectionProblems);
                return false;
            }
            return true;
        }

        public async Task SaveUserInfo(string username)
        {
            UpdateAppStatus(MiruAppStatus.Busy, "Saving user data to the db...");
            await DbService.SaveSyncedUserData(username);
        }

        public async Task<bool> SaveAnimeListData(bool isSeasonSyncOn)
        {
            if (!await DbService.SaveDetailedAnimeListData(isSeasonSyncOn))
            {
                UpdateAppStatus(MiruAppStatus.InternetConnectionProblems);
                return false;
            }
            return true;
        }

        public void UpdateUiAfterDataSync()
        {
            // update displayed username and sync date
            MalUserName = TypedInUsername;

            // display sorted animes from user's watching anime list
            SelectedDisplayedAnimeList = AnimeListType.Watching;
            SelectedTimeZone = TimeZoneInfo.Local;

            // update app status
            UpdateAppStatus(MiruAppStatus.Idle);
        }

        /// <summary>
        /// Performs synchronization to the typed-in user's anime list on a button click (wired up via caliburn micro).
        /// </summary>
        /// <returns></returns>
        public async Task SyncUserAnimeList(string typedInUsername, MiruAppStatus appStatus, bool seasonSyncOn)
        {
            // get user's list of dropped animes
            if (GetDroppedAnimeData && !await GetUserDroppedAnimeList()) return;

            // get user's watching status anime list
            if (!await GetUserAnimeList()) return;

            // get current season
            if (seasonSyncOn && !await GetCurrentSeason()) return;

            // save user data to the db
            await SaveUserInfo(typedInUsername);

            // save api data to the database
            if (!await SaveAnimeListData(seasonSyncOn)) return;

            // update displayed username and sync date
            UpdateUiAfterDataSync();
        }

        // event handler for the Clear local data button
        public async Task OpenClearLocalDataDialog()
        {
            ContentDialog.Config("Clear local data?",
                                 content: "Clears local database and image cache.");

            UpdateAppStatus(MiruAppStatus.Busy);

            // display confirmation pop-up window
            var result = await ContentDialog.ShowAsync();

            if (result == ModernWpf.Controls.ContentDialogResult.Primary)
            {
                ClearAppData();
            }

            UpdateAppStatus(MiruAppStatus.Idle);
        }

        public void ClearAppData()
        {
            UpdateAppStatus(MiruAppStatus.Busy, "Clearing data...");
            DbService.ClearDb();
            FileSystemService.ClearImageCache();
            MalUserName = string.Empty;
            TypedInUsername = string.Empty;
            CurrentAnimeNameFilter = string.Empty;
        }

        // event handler for "Update data from senpai" button
        public async Task UpdateSenpaiData()
        {
            ContentDialog.Config("Update data from senpai.moe?");

            UpdateAppStatus(MiruAppStatus.Busy);

            // display confirmation pop-up window
            var result = await ContentDialog.ShowAsync();

            if (result == ModernWpf.Controls.ContentDialogResult.Primary)
            {
                UpdateAppStatus(MiruAppStatus.Busy, "Updating data from senpai...");
                FileSystemService.UpdateSenpaiData();
            }

            UpdateAppStatus(MiruAppStatus.Idle);
        }

        // opens MAL anime page
        public Process OpenAnimeURL(string URL)
        {
            return SystemService.StartProcess(URL);
        }

        // saves anime title to the clipboard and shows notification describing this action
        public void CopyAnimeTitleToClipboard(string animeTitle)
        {
            System.Windows.Clipboard.SetDataObject(animeTitle);
            var animeTitleCopiedMessage = PrepareAnimeTitleCopiedNotification(animeTitle);
            ToastNotifierWrapper.DisplayToastNotification(animeTitleCopiedMessage);
        }

        public string PrepareAnimeTitleCopiedNotification(string animeTitle)
        {
            return $"'{animeTitle}' copied to the clipboard!";
        }

        public void UpdateAppStatus(MiruAppStatus newAppStatus, string detailedAppStatusDescription = null)
        {
            if (AppStatus != newAppStatus)
            {
                AppStatus = newAppStatus;
            }
            if (detailedAppStatusDescription != null)
            {
                AppStatusText = detailedAppStatusDescription;
            }
        }

        // event handler for the Clear local data button
        public async Task OpenNoLocalDbInfoDialog()
        {
            var link = @"https://docs.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb?view=sql-server-ver15";

            ContentDialog.Config("No SQL Server Express LocalDB found", "Close", "", secondaryButtonText: "Open LocalDB Download Page",
                                 content: $"Please install SQL Server Express LocalDB 2016 or newer for this app to work!");

            UpdateAppStatus(MiruAppStatus.Busy);

            // display confirmation pop-up window
            var result = await ContentDialog.ShowAsync();
            if (result == ModernWpf.Controls.ContentDialogResult.Primary)
            {
                SystemService.ExitEnvironment(0);
            }
            if (result == ModernWpf.Controls.ContentDialogResult.Secondary)
            {
                SystemService.StartProcess(link);
                SystemService.ExitEnvironment(0);
            }
        }

        // event handler for right click on anime name
        public async Task OpenCopySongDataDialog(string title, string opThemes, string edThemes)
        {
            ContentDialog.Config(
                $"Copy {title}'s OP or ED?", 
                primaryButtonText: "OP", 
                secondaryButtonText: "ED", 
                closeButtonText: "Cancel",
                content: $"{opThemes}\n{edThemes}");

            UpdateAppStatus(MiruAppStatus.Busy);

            // display pop-up window
            var result = await ContentDialog.ShowAsync();

            switch (result)
            {
                case ModernWpf.Controls.ContentDialogResult.Primary:
                    var opTitleMatches = Regex.Matches(opThemes, @"(?<="")(.*)(?="")");
                    var opArtistMatches = Regex.Matches(opThemes, @"(?<=by\s)(.*)(?=\s)");
                    string opOutput = null;
                    for (int i = 0; i < opTitleMatches.Count; i++)
                    {
                        opOutput += $"{opTitleMatches[i].Value} {opArtistMatches[i].Value}\n";
                    }
                    CopyAnimeTitleToClipboard(opOutput);
                    break;
                case ModernWpf.Controls.ContentDialogResult.Secondary:
                    var edTitleMatches = Regex.Matches(edThemes, @"(?<="")(.*)(?="")");
                    var edArtistMatches = Regex.Matches(edThemes, @"(?<=by\s)(.*)(?=\s)");
                    string edOutput = null;
                    for (int i = 0; i < edTitleMatches.Count; i++)
                    {
                        var edArtistName = 
                            edArtistMatches[i].Value.Contains(" (ep") ? 
                            edArtistMatches[i].Value.Remove(edArtistMatches[i].Value.IndexOf(" (ep"))
                            : edArtistMatches[i].Value;
                        edOutput += $"{edTitleMatches[i].Value} {edArtistName}\n";
                    }
                    CopyAnimeTitleToClipboard(edOutput);
                    break;
            }

            UpdateAppStatus(MiruAppStatus.Idle);
        }

        #endregion event handlers and guard methods
    }
}