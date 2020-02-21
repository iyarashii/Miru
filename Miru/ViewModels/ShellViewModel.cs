using Caliburn.Micro;
using JikanDotNet;
using Miru.Data;
using Miru.Models;
using ModernWpf;
using MyInternetConnectionLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using ToastNotifications.Messages;

namespace Miru.ViewModels
{
    public class ShellViewModel : Screen
    {
        // private fields that are used with properties in this class
        private string _typedInUsername;
        private string _syncStatusText = "Not synced.";
        private string _appStatusText;
        private SortedAnimeListEntries _sortedAnimeListEntries = new SortedAnimeListEntries();
        private MiruAppStatus _appStatus;
        private ApplicationTheme _currentApplicationTheme;
        private SolidColorBrush _daysOfTheWeekBrush;
        private AnimeListType _selectedDisplayedAnimeList;
        private bool _canChangeDisplayedAnimeList;

        // constructor
        public ShellViewModel()
        {
            // create new instance of shellmodel
            ShellModel = new ShellModel(this);

            // apply correct colors to the days of the week depending on windows theme during runtime
            OnThemeChange();

            // set app theme to prevent the app to react to windows theme change while the app is running
            ThemeManager.Current.ApplicationTheme = CurrentApplicationTheme;

            // load synced data from the db
            ShellModel.LoadLastSyncedData();
            
            // set default app status
            AppStatus = MiruAppStatus.Idle;
        }

        #region properties

        public ShellModel ShellModel { get; set; }

        // stores last sync date
        public DateTime SyncDate { get; set; }

        public bool IsDarkModeOn { get; set; }

        public AnimeListType SelectedDisplayedAnimeList
        {
            get { return _selectedDisplayedAnimeList; }
            set
            {
                _selectedDisplayedAnimeList = value;
                ShellModel.ChangeDisplayedAnimeList(value);
                NotifyOfPropertyChange(() => SelectedDisplayedAnimeList);
            }
        }

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
                IsDarkModeOn = _currentApplicationTheme == ApplicationTheme.Dark ? true : false;
                NotifyOfPropertyChange(() => IsDarkModeOn);
                NotifyOfPropertyChange(() => CurrentApplicationTheme);
            }
        }

        // stores anime models sorted for each day of the week
        public SortedAnimeListEntries SortedAnimeListEntries
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
            set
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
            set
            {
                _appStatus = value;
                switch (value)
                {
                    case MiruAppStatus.Idle:
                        AppStatusText = "Idle";
                        CanChangeDisplayedAnimeList = true;
                        break;

                    case MiruAppStatus.CheckingInternetConnection:
                        AppStatusText = "Checking internet connection...";
                        CanChangeDisplayedAnimeList = false;
                        break;

                    case MiruAppStatus.Syncing:
                        AppStatusText = "Syncing...";
                        CanChangeDisplayedAnimeList = false;
                        break;

                    case MiruAppStatus.InternetConnectionProblems:
                        AppStatusText = "Problems with internet connection!";
                        CanChangeDisplayedAnimeList = false;
                        break;

                    case MiruAppStatus.Loading:
                        AppStatusText = "Loading data from the last synchronization...";
                        CanChangeDisplayedAnimeList = false;
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

        // stores text that says the username of the last synced user and time of the sync
        public string SyncStatusText
        {
            get { return _syncStatusText; }
            set
            {
                _syncStatusText = $"Synced to the { value }'s anime list on { SyncDate }";
                NotifyOfPropertyChange(() => SyncStatusText);
            }
        }

        // guard property for DisplayedAnimeList combobox
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
            OnThemeChange();
        }

        // fired on theme change
        public void OnThemeChange()
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
            if (string.IsNullOrWhiteSpace(typedInUsername) || typedInUsername.Length < 2 || typedInUsername.Length > 16 || appStatus != MiruAppStatus.Idle)
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
        public async Task SyncUserAnimeList(string typedInUsername, MiruAppStatus appStatus, bool syncSeasonList)
        {
            // stop method execution if there is a problem with internet connection
            if (!await InternetConnectionViewModel.CheckAppInternetConnectionStatus(this)) return;

            // get user's watching status anime list
            if (await ShellModel.GetCurrentUserAnimeList() == false) return;

            if (syncSeasonList)
            {
                // get current season
                if (await ShellModel.GetCurrentSeasonList() == false) return;
            }

            // save api data to the database
            if( await ShellModel.SaveSyncData(syncSeasonList) == false) return;

            // update displayed username and sync date
            SyncStatusText = TypedInUsername;

            // display sorted animes from user's watching anime list
            SelectedDisplayedAnimeList = AnimeListType.Watching;

            // update app status
            AppStatus = MiruAppStatus.Idle;
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
            Constants.notifier.ShowInformation(copyNotification, Constants.messageOptions);
        }

        #endregion
    }
}