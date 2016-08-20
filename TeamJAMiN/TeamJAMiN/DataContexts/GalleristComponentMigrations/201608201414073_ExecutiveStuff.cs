namespace TeamJAMiN.DataContexts.GalleristComponentMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExecutiveStuff : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GameTurns", "HasExecutiveAction", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.GameTurns", "HasExecutiveAction");
        }
    }
}
