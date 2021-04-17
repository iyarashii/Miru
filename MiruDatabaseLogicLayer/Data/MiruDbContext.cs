using JikanDotNet;
using MiruLibrary.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace MiruDatabaseLogicLayer
{
    // app's default db context
    public class MiruDbContext : DbContext, IMiruDbContext
    {
        // specify database name
        public MiruDbContext() : base("MiruDatabase")
        {
        }

        // set db tables
        public virtual DbSet<SyncedMyAnimeListUser> SyncedMyAnimeListUsers { get; set; }
        public virtual DbSet<MiruAnimeModel> MiruAnimeModels { get; set; }

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