using Miru.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
