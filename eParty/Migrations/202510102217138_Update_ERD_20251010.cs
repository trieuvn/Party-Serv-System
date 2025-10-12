namespace eParty.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_ERD_20251010 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.FoodIngredient", name: "FoodId", newName: "Food");
            RenameColumn(table: "dbo.MenuDetail", name: "FoodId", newName: "Food");
            RenameColumn(table: "dbo.PriceHistory", name: "FoodId", newName: "Food");
            RenameColumn(table: "dbo.MenuDetail", name: "MenuId", newName: "Menu");
            RenameColumn(table: "dbo.Party", name: "MenuId", newName: "Menu");
            RenameColumn(table: "dbo.PriceHistory", name: "PartyId", newName: "Party");
            RenameColumn(table: "dbo.StaffParty", name: "PartyId", newName: "Party");
            RenameColumn(table: "dbo.Party", name: "UserUsername", newName: "User");
            RenameColumn(table: "dbo.StaffParty", name: "StaffUsername", newName: "Staff");
            RenameColumn(table: "dbo.FoodIngredient", name: "IngredientId", newName: "Ingredient");
            RenameColumn(table: "dbo.Provider", name: "IngredientId", newName: "Ingredient");
            RenameIndex(table: "dbo.Party", name: "IX_UserUsername", newName: "IX_User");
            RenameIndex(table: "dbo.Party", name: "IX_MenuId", newName: "IX_Menu");
            RenameIndex(table: "dbo.MenuDetail", name: "IX_MenuId", newName: "IX_Menu");
            RenameIndex(table: "dbo.MenuDetail", name: "IX_FoodId", newName: "IX_Food");
            RenameIndex(table: "dbo.FoodIngredient", name: "IX_FoodId", newName: "IX_Food");
            RenameIndex(table: "dbo.FoodIngredient", name: "IX_IngredientId", newName: "IX_Ingredient");
            RenameIndex(table: "dbo.Provider", name: "IX_IngredientId", newName: "IX_Ingredient");
            RenameIndex(table: "dbo.PriceHistory", name: "IX_PartyId", newName: "IX_Party");
            RenameIndex(table: "dbo.PriceHistory", name: "IX_FoodId", newName: "IX_Food");
            RenameIndex(table: "dbo.StaffParty", name: "IX_StaffUsername", newName: "IX_Staff");
            RenameIndex(table: "dbo.StaffParty", name: "IX_PartyId", newName: "IX_Party");
            CreateTable(
                "dbo.Comment",
                c => new
                    {
                        User = c.String(nullable: false, maxLength: 50),
                        News = c.Int(nullable: false),
                        Stars = c.Int(nullable: false),
                        Description = c.String(),
                    })
                .PrimaryKey(t => new { t.User, t.News })
                .ForeignKey("dbo.User", t => t.User, cascadeDelete: true)
                .ForeignKey("dbo.News", t => t.News, cascadeDelete: true)
                .Index(t => t.User)
                .Index(t => t.News);
            
            CreateTable(
                "dbo.News",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50),
                        Subject = c.String(maxLength: 100),
                        Description = c.String(),
                        Status = c.String(maxLength: 20),
                        User = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.User)
                .Index(t => t.User);
            
            CreateTable(
                "dbo.Rate",
                c => new
                    {
                        User = c.String(nullable: false, maxLength: 50),
                        Party = c.Int(nullable: false),
                        Stars = c.Int(nullable: false),
                        Comment = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => new { t.User, t.Party })
                .ForeignKey("dbo.Party", t => t.Party)
                .ForeignKey("dbo.User", t => t.User, cascadeDelete: true)
                .Index(t => t.User)
                .Index(t => t.Party);
            
            CreateTable(
                "dbo.UserDiscount",
                c => new
                    {
                        User = c.String(nullable: false, maxLength: 50),
                        Discount = c.Int(nullable: false),
                        Amount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.User, t.Discount })
                .ForeignKey("dbo.Discount", t => t.Discount, cascadeDelete: true)
                .ForeignKey("dbo.User", t => t.User, cascadeDelete: true)
                .Index(t => t.User)
                .Index(t => t.Discount);
            
            CreateTable(
                "dbo.Discount",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Value = c.Int(nullable: false),
                        CreatedDate = c.DateTime(),
                        ExpiredDate = c.DateTime(),
                        CouponCode = c.String(maxLength: 10),
                        IsValid = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Partner",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50),
                        Subject = c.String(maxLength: 50),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Poster",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50),
                        Image = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Food", "Image", c => c.String());
            AddColumn("dbo.Food", "Discount", c => c.Int());
            AddColumn("dbo.Menu", "Image", c => c.String());
            AddColumn("dbo.Menu", "Discount", c => c.Int());
            AddColumn("dbo.Party", "Image", c => c.String());
            AddColumn("dbo.Party", "CreatedDate", c => c.DateTime());
            AddColumn("dbo.User", "Avatar", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Comment", "News", "dbo.News");
            DropForeignKey("dbo.UserDiscount", "User", "dbo.User");
            DropForeignKey("dbo.UserDiscount", "Discount", "dbo.Discount");
            DropForeignKey("dbo.Rate", "User", "dbo.User");
            DropForeignKey("dbo.Rate", "Party", "dbo.Party");
            DropForeignKey("dbo.News", "User", "dbo.User");
            DropForeignKey("dbo.Comment", "User", "dbo.User");
            DropIndex("dbo.UserDiscount", new[] { "Discount" });
            DropIndex("dbo.UserDiscount", new[] { "User" });
            DropIndex("dbo.Rate", new[] { "Party" });
            DropIndex("dbo.Rate", new[] { "User" });
            DropIndex("dbo.News", new[] { "User" });
            DropIndex("dbo.Comment", new[] { "News" });
            DropIndex("dbo.Comment", new[] { "User" });
            DropColumn("dbo.User", "Avatar");
            DropColumn("dbo.Party", "CreatedDate");
            DropColumn("dbo.Party", "Image");
            DropColumn("dbo.Menu", "Discount");
            DropColumn("dbo.Menu", "Image");
            DropColumn("dbo.Food", "Discount");
            DropColumn("dbo.Food", "Image");
            DropTable("dbo.Poster");
            DropTable("dbo.Partner");
            DropTable("dbo.Discount");
            DropTable("dbo.UserDiscount");
            DropTable("dbo.Rate");
            DropTable("dbo.News");
            DropTable("dbo.Comment");
            RenameIndex(table: "dbo.StaffParty", name: "IX_Party", newName: "IX_PartyId");
            RenameIndex(table: "dbo.StaffParty", name: "IX_Staff", newName: "IX_StaffUsername");
            RenameIndex(table: "dbo.PriceHistory", name: "IX_Food", newName: "IX_FoodId");
            RenameIndex(table: "dbo.PriceHistory", name: "IX_Party", newName: "IX_PartyId");
            RenameIndex(table: "dbo.Provider", name: "IX_Ingredient", newName: "IX_IngredientId");
            RenameIndex(table: "dbo.FoodIngredient", name: "IX_Ingredient", newName: "IX_IngredientId");
            RenameIndex(table: "dbo.FoodIngredient", name: "IX_Food", newName: "IX_FoodId");
            RenameIndex(table: "dbo.MenuDetail", name: "IX_Food", newName: "IX_FoodId");
            RenameIndex(table: "dbo.MenuDetail", name: "IX_Menu", newName: "IX_MenuId");
            RenameIndex(table: "dbo.Party", name: "IX_Menu", newName: "IX_MenuId");
            RenameIndex(table: "dbo.Party", name: "IX_User", newName: "IX_UserUsername");
            RenameColumn(table: "dbo.Provider", name: "Ingredient", newName: "IngredientId");
            RenameColumn(table: "dbo.FoodIngredient", name: "Ingredient", newName: "IngredientId");
            RenameColumn(table: "dbo.StaffParty", name: "Staff", newName: "StaffUsername");
            RenameColumn(table: "dbo.Party", name: "User", newName: "UserUsername");
            RenameColumn(table: "dbo.StaffParty", name: "Party", newName: "PartyId");
            RenameColumn(table: "dbo.PriceHistory", name: "Party", newName: "PartyId");
            RenameColumn(table: "dbo.Party", name: "Menu", newName: "MenuId");
            RenameColumn(table: "dbo.MenuDetail", name: "Menu", newName: "MenuId");
            RenameColumn(table: "dbo.PriceHistory", name: "Food", newName: "FoodId");
            RenameColumn(table: "dbo.MenuDetail", name: "Food", newName: "FoodId");
            RenameColumn(table: "dbo.FoodIngredient", name: "Food", newName: "FoodId");
        }
    }
}
