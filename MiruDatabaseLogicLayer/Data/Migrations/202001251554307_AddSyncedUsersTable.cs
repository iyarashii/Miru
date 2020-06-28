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