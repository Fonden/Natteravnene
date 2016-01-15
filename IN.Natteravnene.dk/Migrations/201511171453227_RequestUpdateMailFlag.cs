namespace IN.Natteravnene.dk.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RequestUpdateMailFlag : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Leads", "RequestUpdateMail", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Leads", "RequestUpdateMail");
        }
    }
}
