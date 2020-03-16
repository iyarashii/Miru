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
        InternetConnectionProblems,
        ClearingDatabase
    }

    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum AnimeListType
    {
        [Description("Airing & Watching")]
        AiringAndWatching,

        [Description("Watching")]
        Watching,

        [Description("Current Season")]
        Season
    }

    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum AnimeType
    {
        [Description("TV & ONA")]
        Both,
        [Description("TV")]
        TV,
        [Description("ONA")]
        ONA
    }
}