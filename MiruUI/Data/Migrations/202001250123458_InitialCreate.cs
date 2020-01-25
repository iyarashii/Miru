namespace Miru.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AnimeListEntries",
                c => new
                    {
                        MalId = c.Long(nullable: false),
                        Title = c.String(),
                        ImageURL = c.String(),
                        URL = c.String(),
                        VideoUrl = c.String(),
                        Type = c.String(),
                        WatchedEpisodes = c.Int(),
                        TotalEpisodes = c.Int(),
                        Score = c.Int(nullable: false),
                        HasEpisodeVideo = c.Boolean(),
                        HasPromoVideo = c.Boolean(),
                        HasVideo = c.Boolean(),
                        IsRewatching = c.Boolean(),
                        Rating = c.String(),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
                        WatchStartDate = c.DateTime(),
                        WatchEndDate = c.DateTime(),
                        Days = c.Int(),
                        Priority = c.String(),
                        AiringStatus = c.Int(nullable: false),
                        WatchingStatus = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.MalId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.AnimeListEntries");
        }
    }
}
