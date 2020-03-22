using JikanDotNet;
using System.Threading.Tasks;

namespace Miru.Models
{
    public interface ICurrentSeasonModel
    {
        Season SeasonData { get; }

        Task<bool> GetCurrentSeasonList(int requestRetryDelayInMs);
    }
}