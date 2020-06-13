using JikanDotNet;
using MiruLibrary.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace Miru.Data
{
    // app's default db context
    public class MiruDbContext : DbContext
    {
        // specify database name
        public MiruDbContext() : base("MiruDatabase")
        {
        }

        // set db tables
        public DbSet<SyncedMyAnimeListUser> SyncedMyAnimeListUsers { get; set; }
        public DbSet<MiruAnimeModel> MiruAnimeModels { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // set MalId as primary key using fluid API
            // and set DatabaseGeneratedOption.None to prevent EF from changing MalId to generated numbers
            modelBuilder.Entity<AnimeListEntry>()
                .HasKey(a => a.MalId)
                .Property(p => p.MalId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
        }
    }
}