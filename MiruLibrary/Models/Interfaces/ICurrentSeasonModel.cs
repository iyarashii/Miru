using JikanDotNet;
using System.Threading.Tasks;

namespace MiruLibrary.Models
{
    public interface ICurrentSeasonModel
    {
        Season SeasonData { get; }

        Task<bool> GetCurrentSeasonList(int requestRetryDelayInMs);
    }
}