namespace MiruDatabaseLogicLayer.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class ModifyAnimeAiringTimeModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AnimeAiringTimes", "URL", c => c.String());
            AddColumn("dbo.AnimeAiringTimes", "ImageURL", c => c.String());
            AddColumn("dbo.AnimeAiringTimes", "LocalBroadcastTime", c => c.DateTime());
            AddColumn("dbo.AnimeAiringTimes", "WatchedEpisodes", c => c.Int());
            AddColumn("dbo.AnimeAiringTimes", "TotalEpisodes", c => c.Int());
        }

        public override void Down()
        {
            DropColumn("dbo.AnimeAiringTimes", "TotalEpisodes");
            DropColumn("dbo.AnimeAiringTimes", "WatchedEpisodes");
            DropColumn("dbo.AnimeAiringTimes", "LocalBroadcastTime");
            DropColumn("dbo.AnimeAiringTimes", "ImageURL");
            DropColumn("dbo.AnimeAiringTimes", "URL");
        }
    }
}