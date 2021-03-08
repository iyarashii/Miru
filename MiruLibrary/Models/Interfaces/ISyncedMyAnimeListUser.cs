using System;

namespace MiruLibrary.Models
{
    public interface ISyncedMyAnimeListUser
    {
        DateTime SyncTime { get; set; }
        string Username { get; set; }
    }
}