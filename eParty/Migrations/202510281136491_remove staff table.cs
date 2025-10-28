namespace eParty.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removestafftable : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.SystemUser", "Discriminator");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SystemUser", "Discriminator", c => c.String(nullable: false, maxLength: 128));
        }
    }
}
