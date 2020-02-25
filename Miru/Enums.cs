using Miru.Views;
using System.ComponentModel;

// file used as a storage for enums used in this app
namespace Miru
{
    public enum MiruAppStatus
    {
        Loading,
        Idle,
        CheckingInternetConnection,
        Syncing,
        InternetConnectionProblems
    }

    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum AnimeListType
    {
        [Description("Watching")]
        Watching,

        [Description("Current Season")]
        Season
    }
}