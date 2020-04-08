namespace cloudSiteMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLogger : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Loggers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DateTime = c.DateTime(nullable: false),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.User_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Loggers", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Loggers", new[] { "User_Id" });
            DropTable("dbo.Loggers");
        }
    }
}
