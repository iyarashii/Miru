namespace MiruDatabaseLogicLayer.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class UpdateMiruAiringAnimeModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MiruAiringAnimeModels", "JSTBroadcastTime", c => c.DateTime());
            AddColumn("dbo.MiruAiringAnimeModels", "IsOnWatchingList", c => c.Boolean(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.MiruAiringAnimeModels", "IsOnWatchingList");
            DropColumn("dbo.MiruAiringAnimeModels", "JSTBroadcastTime");
        }
    }
}