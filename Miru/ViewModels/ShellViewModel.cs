using Caliburn.Micro;
using Miru.Data;
using ModernWpf;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using ToastNotifications.Messages;

namespace Miru.ViewModels
{
    public class ShellViewModel : Screen, IShellViewModel
    {
        // private fields that are used with properties in this class
        private string _typedInUsername;
        private string _appStatusText;
        private ISortedAnimeListEntries _sortedAnimeListEntries;
        private MiruAppStatus _appStatus;
        private ApplicationTheme _currentApplicationTheme;
        private SolidColorBrush _daysOfTheWeekBrush;
        private AnimeListType _selectedDisplayedAnimeList;
        private TimeZoneInfo _selectedTimeZone;
        private bool _canChangeDisplayedAnimeList;
        private AnimeType _selectedDisplayedAnimeType;
        private string _malUserName;
        private string _userAnimeListURL;

        // constructor
        public ShellViewModel(ISortedAnimeListEntries sortedAnimeListEntries, IMiruDbService miruDbService, IContentDialogWrapper contentDialog)
        {
            // dependency injection
            _sortedAnimeListEntries = sortedAnimeListEntries;

            // assign db service to the injected instance
            DbService = miruDbService;

            ContentDialog = contentDialog;

            // set db service viewmodel context to this view model
            DbService.ViewModelContext = this;

            // set system's local time zone as initially selected time zone
            SelectedTimeZone = TimeZoneInfo.Local;

            // apply correct colors to the days of the week depending on windows theme during runtime
            UpdateBrushColors();

            // set app theme to prevent the app to react to windows theme change while the app is running
            ThemeManager.Current.ApplicationTheme = CurrentApplicationTheme;

            // load synced data from the db
            DbService.LoadLastSyncedData();

            // set default app status
            UpdateAppStatus(MiruAppStatus.Idle);
        }

        #region properties

        public IContentDialogWrapper ContentDialog { get; }

        // stores MiruDbService's instance that contains most of the business logic
        public IMiruDbService DbService { get; }

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
                DbService.ChangeDisplayedAnimeList(value, SelectedTimeZone, SelectedDisplayedAnimeType);
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

                DbService.ChangeDisplayedAnimeList(SelectedDisplayedAnimeList, SelectedTimeZone, value);
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
                DbService.ChangeDisplayedAnimeList(SelectedDisplayedAnimeList, value, SelectedDisplayedAnimeType);
                NotifyOfPropertyChange(() => SelectedTimeZone);
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
        public ISortedAnimeListEntries SortedAnimeListEntries
        {
            get { return _sortedAnimeListEntries; }
            set
            {
                _sortedAnimeListEntries = value;
                NotifyOfPropertyChange(() => SortedAnimeListEntries);
            }
        }

        // stores app status text which is displayed as app window name
        public string AppStatusText
        {
            get { return _appStatusText; }
            private set
            {
                _appStatusText = $"Miru -- { value }";
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
                _userAnimeListURL = $@"https://myanimelist.net/animelist/{ value }";
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

        #endregion properties

        #region event handlers and guard methods

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

        /// <summary>
        /// Performs synchronization to the typed-in user's anime list on a button click (wired up via caliburn micro).
        /// </summary>
        /// <param name="typedInUsername"></param>
        /// <returns></returns>
        public async Task SyncUserAnimeList(string typedInUsername, MiruAppStatus appStatus, bool seasonSyncOn)
        {
            // get user's watching status anime list
            UpdateAppStatus(MiruAppStatus.Busy, "Getting current user anime list...");

            var getCurrentUserAnimeListResult = await DbService.CurrentUserAnimeList.GetCurrentUserAnimeList(TypedInUsername);

            if (!getCurrentUserAnimeListResult.Success)
            {
                UpdateAppStatus(MiruAppStatus.Idle, getCurrentUserAnimeListResult.ErrorMessage);
                return;
            }

            // get current season
            if (seasonSyncOn)
            {
                UpdateAppStatus(MiruAppStatus.Busy, "Getting current season anime list...");

                if (!await DbService.CurrentSeason.GetCurrentSeasonList(2000))
                {
                    UpdateAppStatus(MiruAppStatus.InternetConnectionProblems);
                    return;
                }
            }

            // save user data to the db
            UpdateAppStatus(MiruAppStatus.Busy, "Saving user data to the db...");
            await DbService.SaveSyncedUserData();

            // save api data to the database
            if (!await DbService.SaveDetailedAnimeListData(seasonSyncOn))
            {
                UpdateAppStatus(MiruAppStatus.InternetConnectionProblems);
                return;
            }

            // update displayed username and sync date
            MalUserName = TypedInUsername;

            // display sorted animes from user's watching anime list
            SelectedDisplayedAnimeList = AnimeListType.AiringAndWatching;
            SelectedTimeZone = TimeZoneInfo.Local;

            // update app status
            UpdateAppStatus(MiruAppStatus.Idle);
        }

        // event handler for the Clear db button
        public async Task ClearDatabase()
        {
            ContentDialog.Config("Clear the database?");

            UpdateAppStatus(MiruAppStatus.Busy, "Waiting for user action...");

            // display confirmation pop-up window
            var result = await ContentDialog.ShowAsync();

            if (result == ModernWpf.Controls.ContentDialogResult.Primary)
            {
                UpdateAppStatus(MiruAppStatus.Busy, "Clearing database...");
                DbService.ClearDb();
                MalUserName = string.Empty;
                TypedInUsername = string.Empty;
                DbService.ChangeDisplayedAnimeList(SelectedDisplayedAnimeList, SelectedTimeZone, SelectedDisplayedAnimeType);
            }

            UpdateAppStatus(MiruAppStatus.Idle);
        }

        // event handler for "Update data from senpai" button
        public async Task UpdateSenpaiData()
        {
            ContentDialog.Config("Update data from senpai.moe?");

            UpdateAppStatus(MiruAppStatus.Busy, "Waiting for user action...");

            // display confirmation pop-up window
            var result = await ContentDialog.ShowAsync();

            if (result == ModernWpf.Controls.ContentDialogResult.Primary)
            {
                UpdateAppStatus(MiruAppStatus.Busy, "Updating data from senpai...");
                DbService.UpdateSenpaiData();
            }

            UpdateAppStatus(MiruAppStatus.Idle);
        }

        // opens MAL anime page
        public void OpenAnimeURL(string URL)
        {
            Process.Start(URL);
        }

        // saves anime title to the clipboard and shows notification describing this action
        public void CopyAnimeTitleToClipboard(string animeTitle)
        {
            string copyNotification = $"'{ animeTitle }' copied to the clipboard!";
            System.Windows.Clipboard.SetText(animeTitle);
            Constants.ToastNotifier.ShowInformation(copyNotification, Constants.MessageOptions);
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

        #endregion event handlers and guard methods
    }
}