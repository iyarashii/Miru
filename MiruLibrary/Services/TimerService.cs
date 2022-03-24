using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiruLibrary.Services
{
    public class TimerService : ITimerService
    {
        public async Task DelayTask(int millisecondsDelay)
        {
            await Task.Delay(millisecondsDelay);
        }
    }
}
