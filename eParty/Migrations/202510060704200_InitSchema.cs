namespace eParty.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class InitSchema : DbMigration
    {
        public override void Up()
        {
            // ======= CORE: USER (TPH cho Staff) =======
            CreateTable(
                "dbo.User",
                c => new
                {
                    Username = c.String(nullable: false, maxLength: 50),
                    Password = c.String(nullable: false, maxLength: 50),
                    FirstName = c.String(maxLength: 50),
                    LastName = c.String(maxLength: 50),
                    Avatar = c.String(),             // nvarchar(MAX)
                    Email = c.String(maxLength: 50),
                    PhoneNumber = c.String(maxLength: 50),
                    Role = c.String(maxLength: 20),
                    // TPH inheritance
                    Discriminator = c.String(nullable: false, maxLength: 128),
                    Salary = c.Int(),               // chỉ dùng khi Discriminator = "Staff"
                })
                .PrimaryKey(t => t.Username);

            // ======= DISCOUNT =======
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
                "dbo.UserDiscount",
                c => new
                {
                    User = c.String(nullable: false, maxLength: 50),
                    Discount = c.Int(nullable: false),
                    Amount = c.Int(nullable: false),
                })
                .PrimaryKey(t => new { t.User, t.Discount })
                .ForeignKey("dbo.User", t => t.User, cascadeDelete: false)
                .ForeignKey("dbo.Discount", t => t.Discount, cascadeDelete: false)
                .Index(t => t.User)
                .Index(t => t.Discount);

            // ======= MENU / FOOD =======
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
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.MenuDetail",
                c => new
                {
                    Menu = c.Int(nullable: false),
                    Food = c.Int(nullable: false),
                    Amount = c.Int(nullable: false),
                })
                .PrimaryKey(t => new { t.Menu, t.Food })
                .ForeignKey("dbo.Menu", t => t.Menu, cascadeDelete: false)
                .ForeignKey("dbo.Food", t => t.Food, cascadeDelete: false)
                .Index(t => t.Menu)
                .Index(t => t.Food);

            // ======= PARTY =======
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
                    User = c.String(maxLength: 50), // FK -> User.Username
                    Menu = c.Int(),                 // FK -> Menu.Id
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.User, cascadeDelete: false)
                .ForeignKey("dbo.Menu", t => t.Menu, cascadeDelete: false)
                .Index(t => t.User)
                .Index(t => t.Menu);

            // ======= PRICE HISTORY (Party <-> Food) =======
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
                .ForeignKey("dbo.Party", t => t.Party, cascadeDelete: false)
                .ForeignKey("dbo.Food", t => t.Food, cascadeDelete: false)
                .Index(t => t.Party)
                .Index(t => t.Food);

            // ======= STAFF <-> PARTY (join) =======
            CreateTable(
                "dbo.StaffParty",
                c => new
                {
                    Staff = c.String(nullable: false, maxLength: 50), // FK -> User (Staff TPH)
                    Party = c.Int(nullable: false),                    // FK -> Party
                })
                .PrimaryKey(t => new { t.Staff, t.Party })
                .ForeignKey("dbo.User", t => t.Staff, cascadeDelete: false)
                .ForeignKey("dbo.Party", t => t.Party, cascadeDelete: false)
                .Index(t => t.Staff)
                .Index(t => t.Party);

            // ======= INGREDIENT, PROVIDER, FOODINGREDIENT =======
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
                    Ingredient = c.Int(nullable: false), // FK -> Ingredient
                    Name = c.String(maxLength: 50),
                    Description = c.String(),
                    PhoneNumber = c.String(maxLength: 15),
                    Address = c.String(maxLength: 100),
                    Latitude = c.Double(),
                    Longitude = c.Double(),
                    Cost = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Ingredient", t => t.Ingredient, cascadeDelete: false)
                .Index(t => t.Ingredient);

            CreateTable(
                "dbo.FoodIngredient",
                c => new
                {
                    Food = c.Int(nullable: false), // FK -> Food
                    Ingredient = c.Int(nullable: false), // FK -> Ingredient
                    Amount = c.Int(nullable: false),
                })
                .PrimaryKey(t => new { t.Food, t.Ingredient })
                .ForeignKey("dbo.Food", t => t.Food, cascadeDelete: false)
                .ForeignKey("dbo.Ingredient", t => t.Ingredient, cascadeDelete: false)
                .Index(t => t.Food)
                .Index(t => t.Ingredient);

            // ======= RATE (User <-> Party) =======
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
                .ForeignKey("dbo.User", t => t.User, cascadeDelete: false)
                .ForeignKey("dbo.Party", t => t.Party, cascadeDelete: false)
                .Index(t => t.User)
                .Index(t => t.Party);

            // ======= NEWS & COMMENT =======
            CreateTable(
                "dbo.News",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(maxLength: 50),
                    Subject = c.String(maxLength: 100),
                    Description = c.String(),
                    Status = c.String(maxLength: 20),
                    User = c.String(maxLength: 50), // author -> User
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.User, cascadeDelete: false)
                .Index(t => t.User);

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
                .ForeignKey("dbo.User", t => t.User, cascadeDelete: false)
                .ForeignKey("dbo.News", t => t.News, cascadeDelete: false)
                .Index(t => t.User)
                .Index(t => t.News);

            // ======= POSTER & PARTNER =======
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
                "dbo.Partner",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(maxLength: 50),
                    Subject = c.String(maxLength: 50),
                    Description = c.String(),
                })
                .PrimaryKey(t => t.Id);
        }

        public override void Down()
        {
            // Drop FKs & Indexes in reverse order
            DropTable("dbo.Partner");
            DropTable("dbo.Poster");

            DropForeignKey("dbo.Comment", "News", "dbo.News");
            DropForeignKey("dbo.Comment", "User", "dbo.User");
            DropIndex("dbo.Comment", new[] { "News" });
            DropIndex("dbo.Comment", new[] { "User" });
            DropTable("dbo.Comment");

            DropForeignKey("dbo.News", "User", "dbo.User");
            DropIndex("dbo.News", new[] { "User" });
            DropTable("dbo.News");

            DropForeignKey("dbo.Rate", "Party", "dbo.Party");
            DropForeignKey("dbo.Rate", "User", "dbo.User");
            DropIndex("dbo.Rate", new[] { "Party" });
            DropIndex("dbo.Rate", new[] { "User" });
            DropTable("dbo.Rate");

            DropForeignKey("dbo.FoodIngredient", "Ingredient", "dbo.Ingredient");
            DropForeignKey("dbo.FoodIngredient", "Food", "dbo.Food");
            DropIndex("dbo.FoodIngredient", new[] { "Ingredient" });
            DropIndex("dbo.FoodIngredient", new[] { "Food" });
            DropTable("dbo.FoodIngredient");

            DropForeignKey("dbo.Provider", "Ingredient", "dbo.Ingredient");
            DropIndex("dbo.Provider", new[] { "Ingredient" });
            DropTable("dbo.Provider");

            DropTable("dbo.Ingredient");

            DropForeignKey("dbo.StaffParty", "Party", "dbo.Party");
            DropForeignKey("dbo.StaffParty", "Staff", "dbo.User");
            DropIndex("dbo.StaffParty", new[] { "Party" });
            DropIndex("dbo.StaffParty", new[] { "Staff" });
            DropTable("dbo.StaffParty");

            DropForeignKey("dbo.PriceHistory", "Food", "dbo.Food");
            DropForeignKey("dbo.PriceHistory", "Party", "dbo.Party");
            DropIndex("dbo.PriceHistory", new[] { "Food" });
            DropIndex("dbo.PriceHistory", new[] { "Party" });
            DropTable("dbo.PriceHistory");

            DropForeignKey("dbo.Party", "Menu", "dbo.Menu");
            DropForeignKey("dbo.Party", "User", "dbo.User");
            DropIndex("dbo.Party", new[] { "Menu" });
            DropIndex("dbo.Party", new[] { "User" });
            DropTable("dbo.Party");

            DropForeignKey("dbo.MenuDetail", "Food", "dbo.Food");
            DropForeignKey("dbo.MenuDetail", "Menu", "dbo.Menu");
            DropIndex("dbo.MenuDetail", new[] { "Food" });
            DropIndex("dbo.MenuDetail", new[] { "Menu" });
            DropTable("dbo.MenuDetail");

            DropTable("dbo.Food");
            DropTable("dbo.Menu");

            DropForeignKey("dbo.UserDiscount", "Discount", "dbo.Discount");
            DropForeignKey("dbo.UserDiscount", "User", "dbo.User");
            DropIndex("dbo.UserDiscount", new[] { "Discount" });
            DropIndex("dbo.UserDiscount", new[] { "User" });
            DropTable("dbo.UserDiscount");

            DropTable("dbo.Discount");
            DropTable("dbo.User");
        }
    }

}