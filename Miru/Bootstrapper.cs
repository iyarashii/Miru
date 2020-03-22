using Autofac;
using Caliburn.Micro.Autofac;
using Miru.Data;
using Miru.Models;
using Miru.ViewModels;
using System.Windows;

namespace Miru
{
    public class Bootstrapper : AutofacBootstrapper<ShellViewModel>
    {
        public Bootstrapper()
        {
            Initialize();
        }

        protected override void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterType<SortedAnimeListEntries>().As<ISortedAnimeListEntries>();
            builder.RegisterType<ShellViewModel>().As<IShellViewModel>();
            //builder.RegisterAssemblyTypes(Assembly.Load(nameof(Miru)))
            //    .Where(t => t.Name == nameof(MiruDbService))
            //    .As(t => t.GetInterfaces().FirstOrDefault(i => i.Name == "I" + t.Name));
            builder.RegisterType<MiruDbService>().As<IMiruDbService>();
            //var clearDatabaseDialog = new ClearDbContentDialog
            //{
            //    Title = "Clear the database?",
            //    PrimaryButtonText = "Yes",
            //    CloseButtonText = "No",
            //    DefaultButton = ModernWpf.Controls.ContentDialogButton.Primary,
            //};
            //builder.RegisterInstance(clearDatabaseDialog).As<IClearDbContentDialog>();
            builder.RegisterType<ContentDialogWrapper>().As<IContentDialogWrapper>();
            builder.RegisterType<CurrentSeasonModel>().As<ICurrentSeasonModel>();
            builder.RegisterType<CurrentUserAnimeListModel>().As<ICurrentUserAnimeListModel>();
            //builder.RegisterType<SyncedMyAnimeListUser>().AsSelf().InstancePerDependency();
        }

        protected override void ConfigureBootstrapper()
        {
            base.ConfigureBootstrapper();
            EnforceNamespaceConvention = false;
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            // set starting view for this app
            DisplayRootViewFor<ShellViewModel>();
        }
    }
}