namespace eParty.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hello : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.News", "CreatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.News", "ImageUrl", c => c.String(maxLength: 200));
            AddColumn("dbo.News", "ViewCount", c => c.Int(nullable: false));
            AddColumn("dbo.News", "IsPublished", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.News", "IsPublished");
            DropColumn("dbo.News", "ViewCount");
            DropColumn("dbo.News", "ImageUrl");
            DropColumn("dbo.News", "CreatedDate");
        }
    }
}
