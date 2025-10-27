namespace eParty.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class pad : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.News", "Image", c => c.String(maxLength: 200));
            DropColumn("dbo.News", "ImageUrl");
        }
        
        public override void Down()
        {
            AddColumn("dbo.News", "ImageUrl", c => c.String(maxLength: 200));
            DropColumn("dbo.News", "Image");
        }
    }
}
