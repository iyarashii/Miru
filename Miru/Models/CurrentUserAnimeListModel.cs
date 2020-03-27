using JikanDotNet;
using System.Threading.Tasks;

namespace Miru.Models
{
    public class CurrentUserAnimeListModel : ICurrentUserAnimeListModel
    {
        public CurrentUserAnimeListModel(IJikan jikanWrapper)
        {
            JikanWrapper = jikanWrapper;
        }

        private IJikan JikanWrapper { get; }

        // stores anime list of the currently synced user
        public UserAnimeList UserAnimeListData { get; private set; }

        // get user's watching status anime list
        public async Task<bool> GetCurrentUserAnimeList(string malUsername, int requestRetryDelayInMs)
        {
            try
            {
                // get user's watching status anime list
                UserAnimeListData = await JikanWrapper.GetUserAnimeList(malUsername, UserAnimeListExtension.Watching);
            }
            catch (System.Net.Http.HttpRequestException)
            {
                return false;
            }

            // if there is no response from API wait for a specified time and retry
            while (UserAnimeListData == null)
            {
                await Task.Delay(requestRetryDelayInMs);
                try
                {
                    // get user's watching status anime list
                    UserAnimeListData = await JikanWrapper.GetUserAnimeList(malUsername, UserAnimeListExtension.Watching);
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