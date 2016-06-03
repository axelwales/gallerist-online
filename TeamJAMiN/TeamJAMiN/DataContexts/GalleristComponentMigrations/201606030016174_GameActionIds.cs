namespace TeamJAMiN.DataContexts.GalleristComponentMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GameActionIds : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GameTurns", "NextActionId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.GameTurns", "NextActionId");
        }
    }
}
