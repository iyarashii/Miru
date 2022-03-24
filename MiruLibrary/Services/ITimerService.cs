using System.Threading.Tasks;

namespace MiruLibrary.Services
{
    public interface ITimerService
    {
        Task DelayTask(int millisecondsDelay);
    }
}