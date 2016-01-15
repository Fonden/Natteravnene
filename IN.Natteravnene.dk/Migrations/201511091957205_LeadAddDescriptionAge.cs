namespace IN.Natteravnene.dk.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LeadAddDescriptionAge : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Leads", "Age", c => c.Int(nullable: false));
            AddColumn("dbo.Leads", "Comments", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Leads", "Comments");
            DropColumn("dbo.Leads", "Age");
        }
    }
}
