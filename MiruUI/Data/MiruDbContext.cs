using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JikanDotNet;
using System.ComponentModel.DataAnnotations.Schema;
using Miru.Models;

namespace Miru.Data
{
    public class MiruDbContext : DbContext
    {
        // specify database name
        public MiruDbContext() : base("MiruDatabase")
        {
        }

        public DbSet<AnimeListEntry> AnimeListEntries { get; set; }
        public DbSet<SyncedMyAnimeListUser> SyncedMyAnimeListUsers { get; set; }
        public DbSet<MiruAiringAnimeModel> MiruAiringAnimeModels { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // set MalId as primary key using fluid API 
            // and set DatabaseGeneratedOption.None to prevent EF from changing MalId to generated numbers
            modelBuilder.Entity<AnimeListEntry>()
                .HasKey(a => a.MalId)
                .Property(p => p.MalId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            //modelBuilder.Entity<Anime>()
            //    .HasKey(k => k.MalId)
            //    .Property(p => p.MalId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
        }
    }
}
