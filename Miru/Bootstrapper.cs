using Autofac;
using Caliburn.Micro.Autofac;
using Miru.Data;
using Miru.Models;
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
                // local API endpoint
                .WithParameter(new TypedParameter(typeof(string), "http://localhost:9001/public/v3/"))
                // public API endpoint
                //.WithParameters(
                //new[]
                //{
                //    //new NamedParameter("surpressException", false), 
                //    //new NamedParameter("useHttps", true) 
                //})
                .SingleInstance();
            builder.RegisterType<ProcessProxy>().As<IProcessProxy>();
            builder.RegisterType<ClipboardWrapper>().As<IClipboardWrapper>();
            builder.RegisterType<ToastNotifierWrapper>().As<IToastNotifierWrapper>();
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