namespace MiruDatabaseLogicLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsOnSenpaiField : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MiruAnimeModels", "IsOnSenpai", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MiruAnimeModels", "IsOnSenpai");
        }
    }
}
