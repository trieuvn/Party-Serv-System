namespace eParty.Migrations
{
    using eParty.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<eParty.Models.AppDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false; // an toàn hơn
        }

        protected override void Seed(AppDbContext db)
        {
            // ---------- USERS & STAFF ----------
            db.Users.AddOrUpdate(u => u.Username,
                new Staff { Username = "admin", Password = "123456", Role = "Admin", FirstName = "Sang", LastName = "Tran", Salary = 1500 },
                new Staff { Username = "staff1", Password = "123456", Role = "Staff", FirstName = "Linh", LastName = "Pham", Salary = 800 },
                new Staff { Username = "staff2", Password = "123456", Role = "Staff", FirstName = "Huy", LastName = "Le", Salary = 850 },
                new Staff { Username = "staff3", Password = "123456", Role = "Staff", FirstName = "Nga", LastName = "Vu", Salary = 820 },
                new User { Username = "user1", Password = "123456", Role = "User", FirstName = "An", LastName = "Nguyen" },
                new User { Username = "user2", Password = "123456", Role = "User", FirstName = "Binh", LastName = "Tran" }
            );
            db.SaveChanges();

            // ---------- INGREDIENTS ----------
            var ingNames = new[] { "Sugar","Flour","Egg","Milk","Butter","Salt","Pepper",
                           "Chicken","Beef","Rice","Tomato","Lettuce","Onion","Garlic","Oil" };
            foreach (var n in ingNames)
                db.Ingredients.AddOrUpdate(i => i.Name, new Ingredient { Name = n, Description = n + " ingredient" });
            db.SaveChanges();

            // ---------- PROVIDERS (nhiều provider cho 1 ingredient) ----------
            var ing = db.Ingredients.ToDictionary(i => i.Name, i => i.Id);
            db.Providers.AddOrUpdate(x => new { x.Name, x.Ingredient },
                new Provider { Name = "ABC Foods", Ingredient = ing["Chicken"], Cost = 40, PhoneNumber = "0901000001", Address = "Q1 HCM" },
                new Provider { Name = "XYZ Farms", Ingredient = ing["Beef"], Cost = 75, PhoneNumber = "0901000002", Address = "Q3 HCM" },
                new Provider { Name = "Dairy VN", Ingredient = ing["Milk"], Cost = 18, PhoneNumber = "0901000003", Address = "Thu Duc" },
                new Provider { Name = "Bakers Co", Ingredient = ing["Flour"], Cost = 12, PhoneNumber = "0901000004", Address = "Q5 HCM" },
                new Provider { Name = "Sweet Co", Ingredient = ing["Sugar"], Cost = 10, PhoneNumber = "0901000005", Address = "Tan Binh" },
                new Provider { Name = "Veggie Hub", Ingredient = ing["Tomato"], Cost = 9, PhoneNumber = "0901000006", Address = "Go Vap" },
                new Provider { Name = "Veggie Hub", Ingredient = ing["Lettuce"], Cost = 7, PhoneNumber = "0901000006", Address = "Go Vap" }
            );
            db.SaveChanges();

            // ---------- FOODS ----------
            db.Foods.AddOrUpdate(f => f.Name,
                new Food { Name = "Bread", Unit = 1, Cost = 10, Description = "Fresh bread" },
                new Food { Name = "Cake", Unit = 1, Cost = 50, Description = "Sweet cake" },
                new Food { Name = "Fried Chicken", Unit = 1, Cost = 65, Description = "Crispy chicken" },
                new Food { Name = "Beef Stew", Unit = 1, Cost = 90, Description = "Beef and veggies" },
                new Food { Name = "Salad", Unit = 1, Cost = 25, Description = "Mixed salad" },
                new Food { Name = "Rice Bowl", Unit = 1, Cost = 20, Description = "Steamed rice" },
                new Food { Name = "Tomato Soup", Unit = 1, Cost = 30, Description = "Soup with tomato" },
                new Food { Name = "Omelette", Unit = 1, Cost = 22, Description = "Egg omelette" }
            );
            db.SaveChanges();

            var food = db.Foods.ToDictionary(f => f.Name, f => f.Id);

            // ---------- FOOD <-> INGREDIENT ----------
            var fi = new (string Food, string Ing, int Amount)[] {
        ("Bread","Flour",2), ("Bread","Sugar",1), ("Bread","Salt",1), ("Bread","Oil",1),
        ("Cake","Flour",2), ("Cake","Sugar",2), ("Cake","Egg",3), ("Cake","Butter",2), ("Cake","Milk",1),
        ("Fried Chicken","Chicken",1), ("Fried Chicken","Oil",2), ("Fried Chicken","Salt",1), ("Fried Chicken","Pepper",1),
        ("Beef Stew","Beef",2), ("Beef Stew","Onion",1), ("Beef Stew","Garlic",1), ("Beef Stew","Tomato",2),
        ("Salad","Lettuce",2), ("Salad","Tomato",1), ("Salad","Onion",1),
        ("Rice Bowl","Rice",2), ("Rice Bowl","Salt",1),
        ("Tomato Soup","Tomato",3), ("Tomato Soup","Onion",1), ("Tomato Soup","Salt",1),
        ("Omelette","Egg",2), ("Omelette","Milk",1), ("Omelette","Salt",1)
    };
            foreach (var x in fi)
                db.FoodIngredients.AddOrUpdate(k => new { k.Food, k.Ingredient },
                    new FoodIngredient { Food = food[x.Food], Ingredient = ing[x.Ing], Amount = x.Amount });
            db.SaveChanges();

            // ---------- MENUS ----------
            db.Menus.AddOrUpdate(m => m.Name,
                new Menu { Name = "Basic Menu", Description = "Bread, Rice, Salad", Cost = 80, Status = "Active" },
                new Menu { Name = "Premium Menu", Description = "Chicken, Beef, Cake", Cost = 180, Status = "Active" },
                new Menu { Name = "Vegan Menu", Description = "Salad, Tomato Soup", Cost = 90, Status = "Active" },
                new Menu { Name = "Kids Menu", Description = "Omelette, Bread", Cost = 60, Status = "Active" }
            );
            db.SaveChanges();

            var menu = db.Menus.ToDictionary(m => m.Name, m => m.Id);

            // ---------- MENU DETAILS ----------
            var md = new (string Menu, string Food, int Amount)[] {
        ("Basic Menu","Bread",10), ("Basic Menu","Rice Bowl",20), ("Basic Menu","Salad",10),
        ("Premium Menu","Fried Chicken",15), ("Premium Menu","Beef Stew",12), ("Premium Menu","Cake",8),
        ("Vegan Menu","Salad",20), ("Vegan Menu","Tomato Soup",15), ("Vegan Menu","Bread",10),
        ("Kids Menu","Omelette",15), ("Kids Menu","Bread",15), ("Kids Menu","Cake",6)
    };
            foreach (var x in md)
                db.MenuDetails.AddOrUpdate(k => new { k.Menu, k.Food },
                    new MenuDetail { Menu = menu[x.Menu], Food = food[x.Food], Amount = x.Amount });
            db.SaveChanges();

            // ---------- PARTIES ----------
            db.Parties.AddOrUpdate(p => p.Name,
                new Party
                {
                    Name = "Birthday",
                    Type = "Private",
                    Status = "Open",
                    Cost = 200,
                    Slots = 20,
                    Address = "Q1, HCM",
                    User = "admin",
                    Menu = menu["Kids Menu"],
                    CreatedDate = DateTime.Now
                },
                new Party
                {
                    Name = "Wedding",
                    Type = "Ceremony",
                    Status = "Planning",
                    Cost = 1200,
                    Slots = 150,
                    Address = "Thu Duc",
                    User = "user1",
                    Menu = menu["Premium Menu"],
                    CreatedDate = DateTime.Now
                },
                new Party
                {
                    Name = "Company Year End",
                    Type = "Corporate",
                    Status = "Open",
                    Cost = 800,
                    Slots = 80,
                    Address = "District 7",
                    User = "user2",
                    Menu = menu["Basic Menu"],
                    CreatedDate = DateTime.Now
                },
                new Party
                {
                    Name = "Picnic",
                    Type = "Outdoor",
                    Status = "Open",
                    Cost = 300,
                    Slots = 30,
                    Address = "Cu Chi",
                    User = "admin",
                    Menu = menu["Vegan Menu"],
                    CreatedDate = DateTime.Now
                }
            );
            db.SaveChanges();

            var party = db.Parties.ToDictionary(p => p.Name, p => p.Id);

            // ---------- STAFF <-> PARTY ----------
            var sp = new (string Staff, string Party)[] {
        ("admin","Birthday"), ("staff1","Birthday"),
        ("staff2","Wedding"), ("staff3","Wedding"),
        ("staff1","Company Year End"), ("staff2","Company Year End"),
        ("staff3","Picnic")
    };
            foreach (var x in sp)
                db.StaffParties.AddOrUpdate(k => new { k.Staff, k.Party },
                    new StaffParty { Staff = x.Staff, Party = party[x.Party] });
            db.SaveChanges();

            // ---------- PRICE HISTORY ----------
            var ph = new[] {
        new { Party="Birthday",          Food="Omelette",    Cost=20, Amount=15 },
        new { Party="Birthday",          Food="Bread",       Cost=8,  Amount=15 },
        new { Party="Birthday",          Food="Cake",        Cost=45, Amount=6  },
        new { Party="Wedding",           Food="Fried Chicken", Cost=60, Amount=15 },
        new { Party="Wedding",           Food="Beef Stew",     Cost=85, Amount=12 },
        new { Party="Wedding",           Food="Cake",          Cost=48, Amount=8  },
        new { Party="Company Year End",  Food="Bread",       Cost=9,  Amount=10 },
        new { Party="Company Year End",  Food="Rice Bowl",   Cost=18, Amount=20 },
        new { Party="Company Year End",  Food="Salad",       Cost=22, Amount=10 },
        new { Party="Picnic",            Food="Salad",       Cost=23, Amount=20 },
        new { Party="Picnic",            Food="Tomato Soup", Cost=28, Amount=15 },
        new { Party="Picnic",            Food="Bread",       Cost=9,  Amount=10 }
    };
            foreach (var x in ph)
                db.PriceHistories.AddOrUpdate(k => new { k.Party, k.Food },
                    new PriceHistory { Party = party[x.Party], Food = food[x.Food], Cost = x.Cost, Amount = x.Amount });
            db.SaveChanges();

            // ---------- DISCOUNT & USERDISCOUNT ----------
            db.Discounts.AddOrUpdate(d => d.CouponCode,
                new Discount { Value = 10, CouponCode = "WELCOME10", CreatedDate = DateTime.Now, ExpiredDate = DateTime.Now.AddMonths(3), IsValid = true },
                new Discount { Value = 20, CouponCode = "VIP20", CreatedDate = DateTime.Now, ExpiredDate = DateTime.Now.AddMonths(1), IsValid = true }
            );
            db.SaveChanges();
            var disc = db.Discounts.ToDictionary(d => d.CouponCode, d => d.Id);
            db.UserDiscounts.AddOrUpdate(k => new { k.User, k.Discount },
                new UserDiscount { User = "admin", Discount = disc["WELCOME10"], Amount = 1 }
            );

            // ---------- NEWS / COMMENT ----------
            db.News.AddOrUpdate(n => n.Subject,
                new News { Name = "Notice 1", Subject = "New Menu Launched", Description = "Try our premium menu", Status = "Active", User = "admin" }
            );
            db.SaveChanges();
            var newsId = db.News.Where(n => n.Subject == "New Menu Launched").Select(n => n.Id).First();
            db.Comments.AddOrUpdate(k => new { k.User, k.News },
                new Comment { User = "user1", News = newsId, Stars = 5, Description = "Great!" }
            );

            // ---------- RATE ----------
            var birthdayId = party["Birthday"];
            db.Rates.AddOrUpdate(k => new { k.User, k.Party },
                new Rate { User = "user1", Party = birthdayId, Stars = 5, Comment = "Nice party" }
            );

            // ---------- POSTER / PARTNER ----------
            db.Posters.AddOrUpdate(p => p.Name,
                new Poster { Name = "Poster1", Image = "/content/img/poster1.jpg" }
            );
            db.Partners.AddOrUpdate(p => p.Name,
                new Partner { Name = "EcoLayers", Subject = "Green Partner", Description = "Sustainable partner" }
            );
        }
    }
}