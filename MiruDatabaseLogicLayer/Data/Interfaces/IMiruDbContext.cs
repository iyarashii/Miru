// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using MiruLibrary.Models;
using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace MiruDatabaseLogicLayer
{
    public interface IMiruDbContext : IDisposable
    {
        DbSet<MiruAnimeModel> MiruAnimeModels { get; set; }
        DbSet<SyncedMyAnimeListUser> SyncedMyAnimeListUsers { get; set; }
        Database Database { get; }
        Task<int> SaveChangesAsync();
        int ExecuteSqlCommand(string sql, params object[] parameters);
    }
}