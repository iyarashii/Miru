using MyInternetConnectionLibrary;
using System.Threading.Tasks;

namespace Miru.ViewModels
{
    public static class InternetConnectionViewModel
    {
        // checks internet connection and sets the app status accordingly
        //public static async Task<bool> CheckAppInternetConnectionStatus(ShellViewModel viewModel)
        //{
        //    viewModel.AppStatus = MiruAppStatus.CheckingInternetConnection;
        //    await InternetConnection.CheckForInternetConnection(viewModel.AppStatusText);
        //    viewModel.AppStatus = InternetConnection.Connection ? MiruAppStatus.Syncing : MiruAppStatus.InternetConnectionProblems;
        //    return InternetConnection.Connection;
        //}
    }
}