namespace MiruDatabaseLogicLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameToMiruAnimeModelsColumn : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.MiruAiringAnimeModels", newName: "MiruAnimeModels");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.MiruAnimeModels", newName: "MiruAiringAnimeModels");
        }
    }
}
