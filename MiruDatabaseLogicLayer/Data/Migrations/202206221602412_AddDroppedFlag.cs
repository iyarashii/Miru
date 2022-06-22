namespace MiruDatabaseLogicLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
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
