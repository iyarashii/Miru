using JikanDotNet;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyInternetConnectionLibrary
{
    public class WebService : IWebService
    {
        public WebService(Func<IWebClientWrapper> createWebClient)
        {
            CreateWebClient = createWebClient;
        }

        public Func<IWebClientWrapper> CreateWebClient { get; private set; }

        // tries to get the detailed anime information about anime with the given mal id, retries after given delay until the internet connection is working
        public async Task<Anime> TryToGetAnimeInfo(long malId, int millisecondsDelay, IJikan jikanWrapper)
        {
            Anime output = null;

            // if there is no response from API wait for the given time and retry
            while (output == null)
            {
                try
                {
                    // get detailed anime info from the jikan API
                    output = await jikanWrapper.GetAnime(malId);
                }
                catch (System.Net.Http.HttpRequestException)
                {
                    throw new NoInternetConnectionException("No internet connection");
                }
                catch (JikanDotNet.Exceptions.JikanRequestException)
                {
                    await Task.Delay(millisecondsDelay);
                }
                finally
                {
                    await Task.Delay(millisecondsDelay);
                }
            }
            return output;
        }
    }
}
