namespace BELibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Property_Gallery_Table : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Galleries", "CreatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Galleries", "CreatedBy", c => c.String());
            AddColumn("dbo.Galleries", "ModifiedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Galleries", "ModifiedBy", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Galleries", "ModifiedBy");
            DropColumn("dbo.Galleries", "ModifiedDate");
            DropColumn("dbo.Galleries", "CreatedBy");
            DropColumn("dbo.Galleries", "CreatedDate");
        }
    }
}
