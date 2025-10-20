namespace eParty.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_category : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Category",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 30),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Food", "CategoryId", c => c.Int(nullable: false));
            CreateIndex("dbo.Food", "CategoryId");
            AddForeignKey("dbo.Food", "CategoryId", "dbo.Category", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Food", "CategoryId", "dbo.Category");
            DropIndex("dbo.Food", new[] { "CategoryId" });
            DropColumn("dbo.Food", "CategoryId");
            DropTable("dbo.Category");
        }
    }
}
