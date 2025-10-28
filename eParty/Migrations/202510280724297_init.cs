namespace eParty.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.News", "CreatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.News", "Image", c => c.String());
            AddColumn("dbo.News", "ViewCount", c => c.Int(nullable: false));
            AddColumn("dbo.News", "IsPublished", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.News", "IsPublished");
            DropColumn("dbo.News", "ViewCount");
            DropColumn("dbo.News", "Image");
            DropColumn("dbo.News", "CreatedDate");
        }
    }
}
