using System;
using System.Collections.Generic;
using System.Text;

namespace MyInternetConnectionLibrary
{
    public class WebService : IWebService
    {
        public WebService(Func<IWebClientWrapper> createWebClient)
        {
            CreateWebClient = createWebClient;
        }

        public Func<IWebClientWrapper> CreateWebClient { get; private set; }
    }
}
