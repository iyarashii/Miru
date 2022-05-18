// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using System;

namespace MiruLibrary.Models
{
    public interface ISyncedMyAnimeListUser
    {
        DateTime SyncTime { get; set; }
        string Username { get; set; }
    }
}