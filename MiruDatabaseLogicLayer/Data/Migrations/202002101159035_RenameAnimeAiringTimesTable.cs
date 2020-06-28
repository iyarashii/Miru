namespace MiruDatabaseLogicLayer.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class RenameAnimeAiringTimesTable : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.AnimeAiringTimes", newName: "MiruAiringAnimeModels");
        }

        public override void Down()
        {
            RenameTable(name: "dbo.MiruAiringAnimeModels", newName: "AnimeAiringTimes");
        }
    }
}