namespace eParty.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitSchema : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FoodIngredient",
                c => new
                    {
                        FoodId = c.Int(nullable: false),
                        IngredientId = c.Int(nullable: false),
                        Amount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.FoodId, t.IngredientId })
                .ForeignKey("dbo.Food", t => t.FoodId)
                .ForeignKey("dbo.Ingredient", t => t.IngredientId)
                .Index(t => t.FoodId)
                .Index(t => t.IngredientId);
            
            CreateTable(
                "dbo.Food",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50),
                        Description = c.String(),
                        Unit = c.Int(nullable: false),
                        Cost = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MenuDetail",
                c => new
                    {
                        MenuId = c.Int(nullable: false),
                        FoodId = c.Int(nullable: false),
                        Amount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.MenuId, t.FoodId })
                .ForeignKey("dbo.Food", t => t.FoodId)
                .ForeignKey("dbo.Menu", t => t.MenuId)
                .Index(t => t.MenuId)
                .Index(t => t.FoodId);
            
            CreateTable(
                "dbo.Menu",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50),
                        Description = c.String(),
                        Cost = c.Int(nullable: false),
                        Status = c.String(maxLength: 20),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Party",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50),
                        Type = c.String(maxLength: 20),
                        Status = c.String(maxLength: 20),
                        Cost = c.Int(nullable: false),
                        BeginTime = c.DateTime(),
                        EndTime = c.DateTime(),
                        Description = c.String(),
                        Slots = c.Int(nullable: false),
                        Address = c.String(maxLength: 100),
                        Latitude = c.Double(),
                        Longitude = c.Double(),
                        UserUsername = c.String(maxLength: 50),
                        MenuId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Menu", t => t.MenuId)
                .ForeignKey("dbo.User", t => t.UserUsername)
                .Index(t => t.UserUsername)
                .Index(t => t.MenuId);
            
            CreateTable(
                "dbo.PriceHistory",
                c => new
                    {
                        PartyId = c.Int(nullable: false),
                        FoodId = c.Int(nullable: false),
                        Cost = c.Int(nullable: false),
                        Amount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.PartyId, t.FoodId })
                .ForeignKey("dbo.Food", t => t.FoodId)
                .ForeignKey("dbo.Party", t => t.PartyId)
                .Index(t => t.PartyId)
                .Index(t => t.FoodId);
            
            CreateTable(
                "dbo.StaffParty",
                c => new
                    {
                        StaffUsername = c.String(nullable: false, maxLength: 50),
                        PartyId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.StaffUsername, t.PartyId })
                .ForeignKey("dbo.Party", t => t.PartyId)
                .ForeignKey("dbo.User", t => t.StaffUsername, cascadeDelete: true)
                .Index(t => t.StaffUsername)
                .Index(t => t.PartyId);
            
            CreateTable(
                "dbo.User",
                c => new
                    {
                        Username = c.String(nullable: false, maxLength: 50),
                        Password = c.String(nullable: false, maxLength: 50),
                        FirstName = c.String(maxLength: 50),
                        LastName = c.String(maxLength: 50),
                        Email = c.String(maxLength: 50),
                        PhoneNumber = c.String(maxLength: 50),
                        Role = c.String(maxLength: 20),
                        Salary = c.Int(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Username);
            
            CreateTable(
                "dbo.Ingredient",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Provider",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IngredientId = c.Int(nullable: false),
                        Name = c.String(maxLength: 50),
                        Description = c.String(),
                        PhoneNumber = c.String(maxLength: 15),
                        Address = c.String(maxLength: 100),
                        Latitude = c.Double(),
                        Longitude = c.Double(),
                        Cost = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Ingredient", t => t.IngredientId)
                .Index(t => t.IngredientId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FoodIngredient", "IngredientId", "dbo.Ingredient");
            DropForeignKey("dbo.Provider", "IngredientId", "dbo.Ingredient");
            DropForeignKey("dbo.FoodIngredient", "FoodId", "dbo.Food");
            DropForeignKey("dbo.MenuDetail", "MenuId", "dbo.Menu");
            DropForeignKey("dbo.Party", "UserUsername", "dbo.User");
            DropForeignKey("dbo.StaffParty", "StaffUsername", "dbo.User");
            DropForeignKey("dbo.StaffParty", "PartyId", "dbo.Party");
            DropForeignKey("dbo.PriceHistory", "PartyId", "dbo.Party");
            DropForeignKey("dbo.PriceHistory", "FoodId", "dbo.Food");
            DropForeignKey("dbo.Party", "MenuId", "dbo.Menu");
            DropForeignKey("dbo.MenuDetail", "FoodId", "dbo.Food");
            DropIndex("dbo.Provider", new[] { "IngredientId" });
            DropIndex("dbo.StaffParty", new[] { "PartyId" });
            DropIndex("dbo.StaffParty", new[] { "StaffUsername" });
            DropIndex("dbo.PriceHistory", new[] { "FoodId" });
            DropIndex("dbo.PriceHistory", new[] { "PartyId" });
            DropIndex("dbo.Party", new[] { "MenuId" });
            DropIndex("dbo.Party", new[] { "UserUsername" });
            DropIndex("dbo.MenuDetail", new[] { "FoodId" });
            DropIndex("dbo.MenuDetail", new[] { "MenuId" });
            DropIndex("dbo.FoodIngredient", new[] { "IngredientId" });
            DropIndex("dbo.FoodIngredient", new[] { "FoodId" });
            DropTable("dbo.Provider");
            DropTable("dbo.Ingredient");
            DropTable("dbo.User");
            DropTable("dbo.StaffParty");
            DropTable("dbo.PriceHistory");
            DropTable("dbo.Party");
            DropTable("dbo.Menu");
            DropTable("dbo.MenuDetail");
            DropTable("dbo.Food");
            DropTable("dbo.FoodIngredient");
        }
    }
}
