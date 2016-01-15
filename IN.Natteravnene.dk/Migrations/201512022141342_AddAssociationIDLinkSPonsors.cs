namespace IN.Natteravnene.dk.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAssociationIDLinkSPonsors : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Links", "Association_AssociationID", "dbo.Associations");
            DropForeignKey("dbo.Sponsors", "Association_AssociationID", "dbo.Associations");
            DropIndex("dbo.Links", new[] { "Association_AssociationID" });
            DropIndex("dbo.Sponsors", new[] { "Association_AssociationID" });
            RenameColumn(table: "dbo.Links", name: "Association_AssociationID", newName: "AssociationID");
            RenameColumn(table: "dbo.Sponsors", name: "Association_AssociationID", newName: "AssociationID");
            AlterColumn("dbo.Links", "AssociationID", c => c.Guid(nullable: false));
            AlterColumn("dbo.Sponsors", "AssociationID", c => c.Guid(nullable: false));
            CreateIndex("dbo.Links", "AssociationID");
            CreateIndex("dbo.Sponsors", "AssociationID");
            AddForeignKey("dbo.Links", "AssociationID", "dbo.Associations", "AssociationID", cascadeDelete: true);
            AddForeignKey("dbo.Sponsors", "AssociationID", "dbo.Associations", "AssociationID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Sponsors", "AssociationID", "dbo.Associations");
            DropForeignKey("dbo.Links", "AssociationID", "dbo.Associations");
            DropIndex("dbo.Sponsors", new[] { "AssociationID" });
            DropIndex("dbo.Links", new[] { "AssociationID" });
            AlterColumn("dbo.Sponsors", "AssociationID", c => c.Guid());
            AlterColumn("dbo.Links", "AssociationID", c => c.Guid());
            RenameColumn(table: "dbo.Sponsors", name: "AssociationID", newName: "Association_AssociationID");
            RenameColumn(table: "dbo.Links", name: "AssociationID", newName: "Association_AssociationID");
            CreateIndex("dbo.Sponsors", "Association_AssociationID");
            CreateIndex("dbo.Links", "Association_AssociationID");
            AddForeignKey("dbo.Sponsors", "Association_AssociationID", "dbo.Associations", "AssociationID");
            AddForeignKey("dbo.Links", "Association_AssociationID", "dbo.Associations", "AssociationID");
        }
    }
}
