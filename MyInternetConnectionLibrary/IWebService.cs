// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

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