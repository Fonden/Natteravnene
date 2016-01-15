namespace IN.Natteravnene.dk.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SponsorSequence : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sponsors", "Sequence", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Sponsors", "Sequence");
        }
    }
}
