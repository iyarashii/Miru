// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

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
using MiruLibrary.Services;

namespace Miru
{
    public class Bootstrapper : AutofacBootstrapper<IShellViewModel>
    {
        public Bootstrapper()
        {
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
            builder.RegisterType<TimerService>().As<ITimerService>();
            builder.RegisterType<UserDataService>().As<IUserDataService>();
            // path to the config file is passed here
            builder.RegisterModule(new SettingsModule());
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

            var sqLocalDbPresent = Container.Resolve<ShellViewModel>().CheckSqlLocalDbInstallationPresence();
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
            Container.Resolve<UserSettings>().AnimeImageSize = Container.Resolve<IShellViewModel>().AnimeImageSizeInPixels;
            Container.Resolve<UserSettings>().DisplayedAnimeListType = Container.Resolve<IShellViewModel>().SelectedDisplayedAnimeList;
            Container.Resolve<UserSettings>().DisplayedAnimeType = Container.Resolve<IShellViewModel>().SelectedDisplayedAnimeType;
            Container.Resolve<ISettingsWriter>().Write(Container.Resolve<UserSettings>());
        }
    }
}