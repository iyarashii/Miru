using JikanDotNet;
using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace MiruLibrary.Models
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
        public async Task<(bool Success, string ErrorMessage)> GetCurrentUserAnimeList(string malUsername)
        {
            try
            {
                // get user's watching status anime list
                UserAnimeListData = await JikanWrapper.GetUserAnimeList(malUsername, UserAnimeListExtension.Watching);
            }
            catch (System.Net.Http.HttpRequestException)
            {
                return (false, "Problems with internet connection!");
            }
            catch (JikanDotNet.Exceptions.JikanRequestException)
            {
                return (false, $"Could not find the user \"{ malUsername }\". Please make sure you typed in the name correctly.");
            }

            return (true, string.Empty);
        }
    }
}