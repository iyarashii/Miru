namespace MiruDatabaseLogicLayer.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddCurrentlyAiringFlag : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MiruAiringAnimeModels", "CurrentlyAiring", c => c.Boolean(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.MiruAiringAnimeModels", "CurrentlyAiring");
        }
    }
}