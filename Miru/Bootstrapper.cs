using Caliburn.Micro;
using Miru.ViewModels;
using System.Windows;

namespace Miru
{
    public class Bootstrapper : BootstrapperBase
    {
        public Bootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            // set starting view for this app
            DisplayRootViewFor<ShellViewModel>();
        }
    }
}