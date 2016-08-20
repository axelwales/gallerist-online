namespace TeamJAMiN.DataContexts.GalleristComponentMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SellAction : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GameContracts", "IsComplete", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.GameContracts", "IsComplete");
        }
    }
}
