using Autofac;
using Caliburn.Micro.Autofac;
using Miru.Data;
using Miru.Models;
using Miru.ViewModels;
using System.Windows;
using JikanDotNet;

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
            builder.RegisterType<ContentDialogWrapper>().As<IContentDialogWrapper>();
            builder.RegisterType<CurrentSeasonModel>().As<ICurrentSeasonModel>();
            builder.RegisterType<CurrentUserAnimeListModel>().As<ICurrentUserAnimeListModel>();
            builder.RegisterType<Jikan>().As<IJikan>().SingleInstance();
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