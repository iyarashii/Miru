// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

namespace MiruDatabaseLogicLayer.Migrations
{
    using System.Data.Entity.Migrations;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public partial class AddDroppedFlag : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MiruAnimeModels", "Dropped", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MiruAnimeModels", "Dropped");
        }
    }
}
