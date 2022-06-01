// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

using JikanDotNet;
using MiruLibrary.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Diagnostics.CodeAnalysis;

namespace MiruDatabaseLogicLayer
{
    // app's default db context
    [ExcludeFromCodeCoverage]
    public class MiruDbContext : DbContext, IMiruDbContext
    {
        // specify database name
        public MiruDbContext() : base("MiruDatabase")
        {
        }

        // for tests
        public int ExecuteSqlCommand(string sql, params object[] parameters)
        {
            return Database.ExecuteSqlCommand(sql, parameters);
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