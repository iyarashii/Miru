// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using JikanDotNet;
using System;
using System.Linq;
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
        public UserAnimeList UserDroppedAnimeListData { get; private set; }

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

        public async Task<(bool Success, string ErrorMessage)> GetCurrentUserDroppedAnimeList(string malUsername)
        {
            int maxPageSize = 300, page = 1;
            try
            {
                // get user's dropped status anime list
                UserDroppedAnimeListData = await JikanWrapper
                    .GetUserAnimeList(malUsername, UserAnimeListExtension.Dropped, page++);

                while (UserDroppedAnimeListData.Anime.Count > 0 && 
                       UserDroppedAnimeListData.Anime.Count % maxPageSize == 0)
                {
                    var nextDroppedAnimeListPage = await JikanWrapper
                        .GetUserAnimeList(malUsername, UserAnimeListExtension.Dropped, page++);
                    if (nextDroppedAnimeListPage.Anime.Count == 0) break;
                    UserDroppedAnimeListData.Anime = UserDroppedAnimeListData.Anime
                                                                             .Concat(nextDroppedAnimeListPage.Anime)
                                                                             .ToArray();
                }
            }
            catch (Exception)
            {
                return (false, "Problem with getting user's dropped anime list!");
            }

            return (true, string.Empty);
        }
    }
}