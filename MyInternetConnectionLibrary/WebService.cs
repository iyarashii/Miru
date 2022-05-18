// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using JikanDotNet;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MyInternetConnectionLibrary
{
    public class WebService : IWebService
    {
        // initializing a read-only instance of HttpClient to give the app access to HTTP GET requests
        public HttpClient Client { get; } = new HttpClient();

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
                catch (HttpRequestException)
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
