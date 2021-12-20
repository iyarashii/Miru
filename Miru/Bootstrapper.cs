using Autofac;
using Caliburn.Micro.Autofac;
using MiruDatabaseLogicLayer;
using MiruLibrary.Models;
using Miru.ViewModels;
using System.Windows;
using JikanDotNet;
using System;
using MiruLibrary.Settings;
using MiruLibrary;
using System.IO.Abstractions;
using MyInternetConnectionLibrary;

namespace Miru
{
    public class Bootstrapper : AutofacBootstrapper<IShellViewModel>
    {
        //[DllImport("kernel32.dll")]
        //static extern IntPtr GetConsoleWindow();

        //[DllImport("user32.dll")]
        //static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        //const int SW_HIDE = 0;
        //const int SW_SHOW = 5;
        public Bootstrapper()
        {
            // this is example code of how to check and display message about localDB being present in console window

            //var handle = GetConsoleWindow();
            //ShowWindow(handle, SW_HIDE);
            //string[] installedVersions;
            //var sqlLocalDbIsInstalled = false;

            //// check if localDB is installed
            //var sqlLocalDbRegistry = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server Local DB\Installed Versions\");
            //if (sqlLocalDbRegistry != null)
            //{
            //    installedVersions = sqlLocalDbRegistry.GetSubKeyNames();
            //    foreach (var s in installedVersions)
            //    {
            //        if (string.IsNullOrEmpty(s))
            //            continue;
            //        if (float.Parse(s) >= 13.0)
            //        {
            //            sqlLocalDbIsInstalled = true;
            //            break;
            //        }
            //    }
            //}
            
            //if (!sqlLocalDbIsInstalled /*&& installedVersions.All(s => string.IsNullOrEmpty(s))*/)
            //{
            //    ShowWindow(handle, SW_SHOW);
            //    Console.WriteLine("LocalDB installation not detected!\nPlease install SQL Server Express LocalDB 2016 or newer!");
            //    Console.ReadKey();
            //    Environment.Exit(0);
            //}

            Initialize();
        }

        protected override void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterType<SortedAnimeListsViewModel>().As<ISortedAnimeListsViewModel>();
            builder.RegisterType<ShellViewModel>().As<IShellViewModel>().SingleInstance();
            builder.RegisterType<MiruDbService>().As<IMiruDbService>();
            builder.RegisterType<SimpleContentDialog>().As<ISimpleContentDialog>();
            builder.RegisterType<CurrentSeasonModel>().As<ICurrentSeasonModel>();
            builder.RegisterType<CurrentUserAnimeListModel>().As<ICurrentUserAnimeListModel>();
            builder.RegisterType<Jikan>().As<IJikan>()
                // dev API endpoint
#if DEBUG
                .WithParameter(new TypedParameter(typeof(string), "http://localhost:9000/v3/"))
                // prod API endpoint
#else
                .WithParameters(
                new[]
                {
                    new NamedParameter("surpressException", false),
                    new NamedParameter("useHttps", true)
                })
#endif
                .SingleInstance();
            builder.RegisterType<ToastNotifierWrapper>().As<IToastNotifierWrapper>();
            builder.RegisterType<MiruAnimeModelProcessor>().As<IMiruAnimeModelProcessor>();
            builder.RegisterType<MiruDbContext>().As<IMiruDbContext>();
            builder.RegisterType<FileSystem>().As<IFileSystem>();
            builder.RegisterType<FileSystemService>().As<IFileSystemService>();
            builder.RegisterType<SyncedMyAnimeListUser>().As<ISyncedMyAnimeListUser>();
            builder.RegisterType<WebClientWrapper>().As<IWebClientWrapper>();
            builder.RegisterType<WebService>().As<IWebService>();
            builder.RegisterType<MiruAnimeModel>();
            // path to the config file is passed here
            builder.RegisterModule(new SettingsModule(Constants.SettingsPath));
        }

        protected override void ConfigureBootstrapper()
        {
            base.ConfigureBootstrapper();
            EnforceNamespaceConvention = false;
        }

        protected override async void OnStartup(object sender, StartupEventArgs e)
        {
            // set starting view for this app
            DisplayRootViewFor<IShellViewModel>();

            var sqLocalDbPresent = Container.Resolve<ShellViewModel>().IsSqlLocalDbInstalled();
            if (!sqLocalDbPresent)
                await Container.Resolve<ShellViewModel>().OpenNoLocalDbInfoDialog();
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            SaveSettings();
            base.OnExit(sender, e);
        }

        private void SaveSettings()
        {
            //UserSettings userSettings = Container.Resolve<UserSettings>();
            //userSettings.AnimeImageSize = Container.Resolve<IShellViewModel>().AnimeImageSizeInPixels;

            Container.Resolve<UserSettings>().AnimeImageSize = Container.Resolve<IShellViewModel>().AnimeImageSizeInPixels;
            Container.Resolve<UserSettings>().DisplayedAnimeListType = Container.Resolve<IShellViewModel>().SelectedDisplayedAnimeList;
            Container.Resolve<UserSettings>().DisplayedAnimeType = Container.Resolve<IShellViewModel>().SelectedDisplayedAnimeType;
            //UserSettings userSettings = new UserSettings
            //{
            //    AnimeImageSize = Container.Resolve<IShellViewModel>().AnimeImageSizeInPixels
            //};

            Container.Resolve<ISettingsWriter>().Write(Container.Resolve<UserSettings>());
        }
    }
}