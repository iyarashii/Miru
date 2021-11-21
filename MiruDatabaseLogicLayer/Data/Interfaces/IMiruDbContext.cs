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