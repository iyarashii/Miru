namespace MiruDatabaseLogicLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOpeningsAndEndings : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MiruAnimeModels", "OpeningThemes", c => c.String());
            AddColumn("dbo.MiruAnimeModels", "EndingThemes", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MiruAnimeModels", "EndingThemes");
            DropColumn("dbo.MiruAnimeModels", "OpeningThemes");
        }
    }
}
