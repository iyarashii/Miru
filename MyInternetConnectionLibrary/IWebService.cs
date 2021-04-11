using JikanDotNet;
using System;
using System.Threading.Tasks;

namespace MyInternetConnectionLibrary
{
    public interface IWebService
    {
        Func<IWebClientWrapper> CreateWebClient { get; }

        Task<Anime> TryToGetAnimeInfo(long malId, int millisecondsDelay, IJikan jikanWrapper);
    }
}