using JikanDotNet;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiruLibrary.Models
{
    public class CurrentSeasonModel : ICurrentSeasonModel
    {
        public CurrentSeasonModel(IJikan jikanWrapper)
        {
            JikanWrapper = jikanWrapper;
        }

        private IJikan JikanWrapper { get; }

        // stores data model of the current anime season
        public Season SeasonData { get; private set; }

        // get current anime season data
        public async Task<bool> GetCurrentSeasonList(int requestRetryDelayInMs)
        {
            SeasonData = null;

            // get current season
            // if there is no response from API wait for a specified time and retry
            while (SeasonData == null)
            {
                try
                {
                    SeasonData = await JikanWrapper.GetSeason();
                }
                catch (System.Net.Http.HttpRequestException)
                {
                    return false;
                }
                catch (JikanDotNet.Exceptions.JikanRequestException)
                {
                    await Task.Delay(requestRetryDelayInMs);
                }
                finally
                {
                    await Task.Delay(requestRetryDelayInMs);
                }
            }
            return true;
        }

        public List<AnimeSubEntry> GetFilteredSeasonList()
        {
            // list of anime entries in the current season
            var currentSeasonList = SeasonData.SeasonEntries.ToList();

            // remove anime entries with types other than 'TV' from the list
            currentSeasonList.RemoveAll(x => x.Type != "TV" && x.Type != "ONA");

            // remove anime entries marked as 'for kids' from the list
            currentSeasonList.RemoveAll(x => x.Kids == true);

            return currentSeasonList;
        }
    }
}