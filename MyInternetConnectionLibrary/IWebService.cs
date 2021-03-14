using System;

namespace MyInternetConnectionLibrary
{
    public interface IWebService
    {
        Func<IWebClientWrapper> CreateWebClient { get; }
    }
}