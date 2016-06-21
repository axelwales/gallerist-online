namespace TeamJAMiN.DataContexts.GalleristComponentMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class datalist : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GameTurns", "PendingActions_NextActionId", c => c.Int(nullable: false));
            AddColumn("dbo.GameTurns", "PendingActions_Data", c => c.String());
            AddColumn("dbo.GameTurns", "CompletedActions_Data", c => c.String());
            DropColumn("dbo.GameTurns", "CompletedActionData");
            DropColumn("dbo.GameTurns", "PendingActionData");
            DropColumn("dbo.GameTurns", "NextActionId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.GameTurns", "NextActionId", c => c.Int(nullable: false));
            AddColumn("dbo.GameTurns", "PendingActionData", c => c.String());
            AddColumn("dbo.GameTurns", "CompletedActionData", c => c.String());
            DropColumn("dbo.GameTurns", "CompletedActions_Data");
            DropColumn("dbo.GameTurns", "PendingActions_Data");
            DropColumn("dbo.GameTurns", "PendingActions_NextActionId");
        }
    }
}
