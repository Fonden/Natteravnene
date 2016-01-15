namespace IN.Natteravnene.dk.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLeads : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Leads",
                c => new
                    {
                        LeadID = c.Guid(nullable: false, identity: true),
                        FirstName = c.String(nullable: false, maxLength: 40),
                        FamilyName = c.String(nullable: false, maxLength: 50),
                        Address = c.String(maxLength: 70),
                        Zip = c.String(maxLength: 15),
                        City = c.String(maxLength: 50),
                        Email = c.String(),
                        Mobile = c.String(maxLength: 15),
                        Phone = c.String(maxLength: 15),
                        Status = c.Int(nullable: false),
                        AssociationID = c.Guid(nullable: true),
                        Lastchanged = c.DateTime(nullable: false, defaultValueSql: "GETDATE()"),
                        Created = c.DateTime(nullable: false, defaultValueSql: "GETDATE()"),
                    })
                .PrimaryKey(t => t.LeadID)
                .ForeignKey("dbo.Associations", t => t.AssociationID, cascadeDelete: false)
                .Index(t => t.AssociationID);


            Sql("CREATE TRIGGER Leads_updated_Lastchanged ON [Leads] AFTER UPDATE AS BEGIN SET NOCOUNT ON; UPDATE [Leads] Set [Lastchanged] = GetDate() where [LeadID] in (SELECT [LeadID] FROM Inserted) END;");
            
        }
        
        public override void Down()
        {
            Sql("DROP TRIGGER [Leads_updated_Lastchanged]");
            DropForeignKey("dbo.Leads", "AssociationID", "dbo.Associations");
            DropIndex("dbo.Leads", new[] { "AssociationID" });
            DropTable("dbo.Leads");
        }
    }
}
