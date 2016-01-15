namespace IN.Natteravnene.dk.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateSponsorAddLink : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Links",
                c => new
                    {
                        LinkID = c.Guid(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        URL = c.String(),
                        Finish = c.DateTime(),
                        Lastchanged = c.DateTime(nullable: false),
                        Created = c.DateTime(nullable: false),
                        Association_AssociationID = c.Guid(),
                    })
                .PrimaryKey(t => t.LinkID)
                .ForeignKey("dbo.Associations", t => t.Association_AssociationID)
                .Index(t => t.Association_AssociationID);
            Sql("CREATE TRIGGER Links_updated_Lastchanged ON [Links] AFTER UPDATE AS BEGIN SET NOCOUNT ON; UPDATE [Links] Set [Lastchanged] = GetDate() where [LinkID] in (SELECT [LinkID] FROM Inserted) END;");
            
            AddColumn("dbo.Sponsors", "URL", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Links", "Association_AssociationID", "dbo.Associations");
            DropIndex("dbo.Links", new[] { "Association_AssociationID" });
            DropColumn("dbo.Sponsors", "URL");
            DropTable("dbo.Links");
        }
    }
}
