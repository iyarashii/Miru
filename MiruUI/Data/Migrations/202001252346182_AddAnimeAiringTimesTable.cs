namespace Miru.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAnimeAiringTimesTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AnimeAiringTimes",
                c => new
                    {
                        MalId = c.Long(nullable: false),
                        Broadcast = c.String(),
                        Title = c.String(),
                    })
                .PrimaryKey(t => t.MalId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.AnimeAiringTimes");
        }
    }
}
