// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/Miru/blob/master/LICENSE for full license details.

namespace MiruDatabaseLogicLayer.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddSyncedUsersTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SyncedMyAnimeListUsers",
                c => new
                {
                    Username = c.String(nullable: false, maxLength: 16),
                    SyncTime = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.Username);
        }

        public override void Down()
        {
            DropTable("dbo.SyncedMyAnimeListUsers");
        }
    }
}