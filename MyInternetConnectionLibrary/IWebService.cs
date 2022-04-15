using JikanDotNet;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MyInternetConnectionLibrary
{
    public interface IWebService
    {
        Func<IWebClientWrapper> CreateWebClient { get; }
        HttpClient Client { get; }

        Task<Anime> TryToGetAnimeInfo(long malId, int millisecondsDelay, IJikan jikanWrapper);
    }
}