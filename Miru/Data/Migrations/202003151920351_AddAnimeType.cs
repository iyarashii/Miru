namespace Miru.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAnimeType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MiruAiringAnimeModels", "Type", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MiruAiringAnimeModels", "Type");
        }
    }
}
