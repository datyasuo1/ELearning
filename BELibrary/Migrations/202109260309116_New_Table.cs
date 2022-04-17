namespace BELibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class New_Table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Galleries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ImageUrl = c.String(),
                        Type = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TeacherProfiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Username = c.String(maxLength: 50),
                        Hobby = c.String(),
                        Description = c.String(),
                        FacebookLink = c.String(),
                        TwitterLink = c.String(),
                        SkypeLink = c.String(),
                        Experiences = c.String(),
                        Address = c.String(),
                        ContactPhone = c.String(),
                        ContactEmail = c.String(),
                        ContactLocation = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TeacherProfiles");
            DropTable("dbo.Galleries");
        }
    }
}
