using Autofac;
using Caliburn.Micro.Autofac;
using MiruDatabaseLogicLayer;
using MiruLibrary.Models;
using Miru.ViewModels;
using System.Windows;
using JikanDotNet;
using System;
using ToastNotifications.Core;

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
            builder.RegisterType<SortedAnimeListEntries>().As<ISortedAnimeListEntries>();
            builder.RegisterType<ShellViewModel>().As<IShellViewModel>();
            builder.RegisterType<MiruDbService>().As<IMiruDbService>();
            builder.RegisterType<SimpleContentDialog>().As<ISimpleContentDialog>();
            builder.RegisterType<CurrentSeasonModel>().As<ICurrentSeasonModel>();
            builder.RegisterType<CurrentUserAnimeListModel>().As<ICurrentUserAnimeListModel>();
            builder.RegisterType<Jikan>().As<IJikan>()
                // dev API endpoint
#if DEBUG
                .WithParameter(new TypedParameter(typeof(string), "http://localhost:9001/public/v3/"))
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
            builder.RegisterType<ProcessProxy>().As<IProcessProxy>();
            builder.RegisterType<ToastNotifierWrapper>().As<IToastNotifierWrapper>();
            builder.RegisterType<MiruAnimeModelProcessor>().As<IMiruAnimeModelProcessor>();
        }

        protected override void ConfigureBootstrapper()
        {
            base.ConfigureBootstrapper();
            EnforceNamespaceConvention = false;
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            // set starting view for this app
            DisplayRootViewFor<IShellViewModel>();
        }
    }
}