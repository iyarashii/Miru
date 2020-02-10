using System;
using System.Collections.Generic;
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
}
