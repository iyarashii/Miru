namespace MiruDatabaseLogicLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAgeRating : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MiruAnimeModels", "AgeRating", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MiruAnimeModels", "AgeRating");
        }
    }
}
