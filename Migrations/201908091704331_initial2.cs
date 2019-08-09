namespace Student_management_.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ChangedUserIds", "LogingId", "dbo.Logings");
            DropIndex("dbo.ChangedUserIds", new[] { "LogingId" });
            CreateTable(
                "dbo.Logs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        Event = c.Int(nullable: false),
                        ChangesMadeToUserId = c.String(),
                        ChangeToTable = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            DropTable("dbo.ChangedUserIds");
            DropTable("dbo.Logings");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Logings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ChangedUserIds",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Event = c.Int(nullable: false),
                        LogingId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            DropTable("dbo.Logs");
            CreateIndex("dbo.ChangedUserIds", "LogingId");
            AddForeignKey("dbo.ChangedUserIds", "LogingId", "dbo.Logings", "Id", cascadeDelete: true);
        }
    }
}
