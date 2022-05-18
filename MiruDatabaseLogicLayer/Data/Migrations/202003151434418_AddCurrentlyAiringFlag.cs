// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

namespace MiruDatabaseLogicLayer.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddCurrentlyAiringFlag : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MiruAiringAnimeModels", "CurrentlyAiring", c => c.Boolean(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.MiruAiringAnimeModels", "CurrentlyAiring");
        }
    }
}