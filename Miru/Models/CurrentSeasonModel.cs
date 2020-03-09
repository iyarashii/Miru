using JikanDotNet;
using System.Threading.Tasks;

namespace Miru.Models
{
    public class CurrentSeasonModel
    {
        // stores data model of the current anime season
        public Season SeasonData { get; private set; }

        // get current anime season data
        public async Task<bool> GetCurrentSeasonList(int requestRetryDelayInMs)
        {
            // get current season
            try
            {
                SeasonData = await Constants.jikan.GetSeason();
            }
            catch (System.Net.Http.HttpRequestException)
            {
                return false;
            }

            // if there is no response from API wait for a specified time and retry
            while (SeasonData == null)
            {
                await Task.Delay(requestRetryDelayInMs);
                try
                {
                    SeasonData = await Constants.jikan.GetSeason();
                }
                catch (System.Net.Http.HttpRequestException)
                {
                    return false;
                }
            }
            return true;
        }
    }
}