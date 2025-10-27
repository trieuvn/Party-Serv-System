namespace eParty.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class news : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.News", "Image", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.News", "Image", c => c.String(maxLength: 200));
        }
    }
}
