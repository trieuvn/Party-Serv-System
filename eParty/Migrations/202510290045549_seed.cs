namespace eParty.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class seed : DbMigration
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
            
            CreateTable(
                "dbo.Food",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50),
                        Description = c.String(),
                        Unit = c.Int(nullable: false),
                        Cost = c.Int(nullable: false),
                        Image = c.String(),
                        Discount = c.Int(),
                        CategoryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Category", t => t.CategoryId, cascadeDelete: true)
                .Index(t => t.CategoryId);
            
            CreateTable(
                "dbo.FoodIngredient",
                c => new
                    {
                        Food = c.Int(nullable: false),
                        Ingredient = c.Int(nullable: false),
                        Amount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Food, t.Ingredient })
                .ForeignKey("dbo.Food", t => t.Food)
                .ForeignKey("dbo.Ingredient", t => t.Ingredient)
                .Index(t => t.Food)
                .Index(t => t.Ingredient);
            
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
                        Ingredient = c.Int(nullable: false),
                        Name = c.String(maxLength: 50),
                        Description = c.String(),
                        PhoneNumber = c.String(maxLength: 15),
                        Address = c.String(maxLength: 100),
                        Latitude = c.Double(),
                        Longitude = c.Double(),
                        Cost = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Ingredient", t => t.Ingredient)
                .Index(t => t.Ingredient);
            
            CreateTable(
                "dbo.MenuDetail",
                c => new
                    {
                        Menu = c.Int(nullable: false),
                        Food = c.Int(nullable: false),
                        Amount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Menu, t.Food })
                .ForeignKey("dbo.Food", t => t.Food)
                .ForeignKey("dbo.Menu", t => t.Menu)
                .Index(t => t.Menu)
                .Index(t => t.Food);
            
            CreateTable(
                "dbo.Menu",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50),
                        Description = c.String(),
                        Cost = c.Int(nullable: false),
                        Status = c.String(maxLength: 20),
                        Image = c.String(),
                        Discount = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Party",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50),
                        Image = c.String(),
                        Type = c.String(maxLength: 20),
                        Status = c.String(maxLength: 20),
                        Cost = c.Int(nullable: false),
                        BeginTime = c.DateTime(),
                        EndTime = c.DateTime(),
                        CreatedDate = c.DateTime(),
                        Description = c.String(),
                        Slots = c.Int(nullable: false),
                        Address = c.String(maxLength: 100),
                        Latitude = c.Double(),
                        Longitude = c.Double(),
                        User = c.String(maxLength: 50),
                        Menu = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Menu", t => t.Menu)
                .ForeignKey("dbo.SystemUser", t => t.User)
                .Index(t => t.User)
                .Index(t => t.Menu);
            
            CreateTable(
                "dbo.SystemUser",
                c => new
                    {
                        Username = c.String(nullable: false, maxLength: 50),
                        Password = c.String(nullable: false, maxLength: 50),
                        FirstName = c.String(maxLength: 50),
                        LastName = c.String(maxLength: 50),
                        Avatar = c.String(),
                        Email = c.String(maxLength: 50),
                        PhoneNumber = c.String(maxLength: 50),
                        Role = c.String(maxLength: 20),
                        Salary = c.Int(),
                    })
                .PrimaryKey(t => t.Username);
            
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
                .ForeignKey("dbo.News", t => t.News, cascadeDelete: true)
                .ForeignKey("dbo.SystemUser", t => t.User, cascadeDelete: true)
                .Index(t => t.User)
                .Index(t => t.News);
            
            CreateTable(
                "dbo.News",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CreatedDate = c.DateTime(nullable: false),
                        Image = c.String(),
                        ViewCount = c.Int(nullable: false),
                        IsPublished = c.Boolean(nullable: false),
                        Name = c.String(maxLength: 50),
                        Subject = c.String(maxLength: 100),
                        Description = c.String(),
                        Status = c.String(maxLength: 20),
                        User = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SystemUser", t => t.User)
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
                .ForeignKey("dbo.SystemUser", t => t.User, cascadeDelete: true)
                .Index(t => t.User)
                .Index(t => t.Party);
            
            CreateTable(
                "dbo.StaffParty",
                c => new
                    {
                        Staff = c.String(nullable: false, maxLength: 50),
                        Party = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Staff, t.Party })
                .ForeignKey("dbo.Party", t => t.Party)
                .ForeignKey("dbo.SystemUser", t => t.Staff, cascadeDelete: true)
                .Index(t => t.Staff)
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
                .ForeignKey("dbo.SystemUser", t => t.User, cascadeDelete: true)
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
                "dbo.PriceHistory",
                c => new
                    {
                        Party = c.Int(nullable: false),
                        Food = c.Int(nullable: false),
                        Cost = c.Int(nullable: false),
                        Amount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Party, t.Food })
                .ForeignKey("dbo.Food", t => t.Food)
                .ForeignKey("dbo.Party", t => t.Party)
                .Index(t => t.Party)
                .Index(t => t.Food);
            
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
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.MenuDetail", "Menu", "dbo.Menu");
            DropForeignKey("dbo.PriceHistory", "Party", "dbo.Party");
            DropForeignKey("dbo.PriceHistory", "Food", "dbo.Food");
            DropForeignKey("dbo.UserDiscount", "User", "dbo.SystemUser");
            DropForeignKey("dbo.UserDiscount", "Discount", "dbo.Discount");
            DropForeignKey("dbo.StaffParty", "Staff", "dbo.SystemUser");
            DropForeignKey("dbo.StaffParty", "Party", "dbo.Party");
            DropForeignKey("dbo.Rate", "User", "dbo.SystemUser");
            DropForeignKey("dbo.Rate", "Party", "dbo.Party");
            DropForeignKey("dbo.Party", "User", "dbo.SystemUser");
            DropForeignKey("dbo.Comment", "User", "dbo.SystemUser");
            DropForeignKey("dbo.Comment", "News", "dbo.News");
            DropForeignKey("dbo.News", "User", "dbo.SystemUser");
            DropForeignKey("dbo.Party", "Menu", "dbo.Menu");
            DropForeignKey("dbo.MenuDetail", "Food", "dbo.Food");
            DropForeignKey("dbo.FoodIngredient", "Ingredient", "dbo.Ingredient");
            DropForeignKey("dbo.Provider", "Ingredient", "dbo.Ingredient");
            DropForeignKey("dbo.FoodIngredient", "Food", "dbo.Food");
            DropForeignKey("dbo.Food", "CategoryId", "dbo.Category");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.PriceHistory", new[] { "Food" });
            DropIndex("dbo.PriceHistory", new[] { "Party" });
            DropIndex("dbo.UserDiscount", new[] { "Discount" });
            DropIndex("dbo.UserDiscount", new[] { "User" });
            DropIndex("dbo.StaffParty", new[] { "Party" });
            DropIndex("dbo.StaffParty", new[] { "Staff" });
            DropIndex("dbo.Rate", new[] { "Party" });
            DropIndex("dbo.Rate", new[] { "User" });
            DropIndex("dbo.News", new[] { "User" });
            DropIndex("dbo.Comment", new[] { "News" });
            DropIndex("dbo.Comment", new[] { "User" });
            DropIndex("dbo.Party", new[] { "Menu" });
            DropIndex("dbo.Party", new[] { "User" });
            DropIndex("dbo.MenuDetail", new[] { "Food" });
            DropIndex("dbo.MenuDetail", new[] { "Menu" });
            DropIndex("dbo.Provider", new[] { "Ingredient" });
            DropIndex("dbo.FoodIngredient", new[] { "Ingredient" });
            DropIndex("dbo.FoodIngredient", new[] { "Food" });
            DropIndex("dbo.Food", new[] { "CategoryId" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Poster");
            DropTable("dbo.Partner");
            DropTable("dbo.PriceHistory");
            DropTable("dbo.Discount");
            DropTable("dbo.UserDiscount");
            DropTable("dbo.StaffParty");
            DropTable("dbo.Rate");
            DropTable("dbo.News");
            DropTable("dbo.Comment");
            DropTable("dbo.SystemUser");
            DropTable("dbo.Party");
            DropTable("dbo.Menu");
            DropTable("dbo.MenuDetail");
            DropTable("dbo.Provider");
            DropTable("dbo.Ingredient");
            DropTable("dbo.FoodIngredient");
            DropTable("dbo.Food");
            DropTable("dbo.Category");
        }
    }
}
