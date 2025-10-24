namespace eParty.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameUserToSystemUser : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.User", newName: "SystemUser");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.SystemUser", newName: "User");
        }
    }
}
