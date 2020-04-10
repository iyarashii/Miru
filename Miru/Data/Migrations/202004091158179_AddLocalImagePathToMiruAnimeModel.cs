namespace Miru.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLocalImagePathToMiruAnimeModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MiruAiringAnimeModels", "LocalImagePath", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MiruAiringAnimeModels", "LocalImagePath");
        }
    }
}
