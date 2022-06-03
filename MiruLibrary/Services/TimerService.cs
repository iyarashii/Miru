// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace MiruLibrary.Services
{
    [ExcludeFromCodeCoverage]
    public class TimerService : ITimerService
    {
        public async Task DelayTask(int millisecondsDelay)
        {
            await Task.Delay(millisecondsDelay);
        }
    }
}
