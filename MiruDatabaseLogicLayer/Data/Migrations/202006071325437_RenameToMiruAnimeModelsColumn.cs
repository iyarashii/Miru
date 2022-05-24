﻿// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

namespace MiruDatabaseLogicLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public partial class RenameToMiruAnimeModelsColumn : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.MiruAiringAnimeModels", newName: "MiruAnimeModels");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.MiruAnimeModels", newName: "MiruAiringAnimeModels");
        }
    }
}
