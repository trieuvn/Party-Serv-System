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
            var ingNames = new[]
            {
        "Sugar","Flour","Egg","Milk","Butter","Salt","Pepper",
        "Chicken","Beef","Rice","Tomato","Lettuce","Onion","Garlic","Oil"
    };
            foreach (var n in ingNames)
                db.Ingredients.AddOrUpdate(i => i.Name, new Ingredient { Name = n, Description = n + " ingredient" });
            db.SaveChanges();

            // ---------- PROVIDERS (nhiều provider cho 1 ingredient) ----------
            var providers = new[]
            {
        new Provider { Name="ABC Foods",  IngredientId=db.Ingredients.Single(i=>i.Name=="Chicken").Id, Cost=40, PhoneNumber="0901 000 001", Address="Q1 HCM" },
        new Provider { Name="XYZ Farms",  IngredientId=db.Ingredients.Single(i=>i.Name=="Beef").Id,     Cost=75, PhoneNumber="0901 000 002", Address="Q3 HCM" },
        new Provider { Name="Dairy VN",   IngredientId=db.Ingredients.Single(i=>i.Name=="Milk").Id,     Cost=18, PhoneNumber="0901 000 003", Address="Thu Duc" },
        new Provider { Name="Bakers Co",  IngredientId=db.Ingredients.Single(i=>i.Name=="Flour").Id,    Cost=12, PhoneNumber="0901 000 004", Address="Q5 HCM" },
        new Provider { Name="Sweet Co",   IngredientId=db.Ingredients.Single(i=>i.Name=="Sugar").Id,    Cost=10, PhoneNumber="0901 000 005", Address="Tan Binh" },
        new Provider { Name="Veggie Hub", IngredientId=db.Ingredients.Single(i=>i.Name=="Tomato").Id,   Cost=9,  PhoneNumber="0901 000 006", Address="Go Vap" },
        new Provider { Name="Veggie Hub", IngredientId=db.Ingredients.Single(i=>i.Name=="Lettuce").Id,  Cost=7,  PhoneNumber="0901 000 006", Address="Go Vap" }
    };
            foreach (var p in providers)
                db.Providers.AddOrUpdate(x => new { x.Name, x.IngredientId }, p);
            db.SaveChanges();

            // ---------- FOODS ----------
            var foods = new[]
            {
        new Food { Name="Bread",        Unit=1, Cost=10, Description="Fresh bread" },
        new Food { Name="Cake",         Unit=1, Cost=50, Description="Sweet cake" },
        new Food { Name="Fried Chicken",Unit=1, Cost=65, Description="Crispy chicken" },
        new Food { Name="Beef Stew",    Unit=1, Cost=90, Description="Beef and veggies" },
        new Food { Name="Salad",        Unit=1, Cost=25, Description="Mixed salad" },
        new Food { Name="Rice Bowl",    Unit=1, Cost=20, Description="Steamed rice" },
        new Food { Name="Tomato Soup",  Unit=1, Cost=30, Description="Soup with tomato" },
        new Food { Name="Omelette",     Unit=1, Cost=22, Description="Egg omelette" }
    };
            foreach (var f in foods) db.Foods.AddOrUpdate(x => x.Name, f);
            db.SaveChanges();

            var dictFood = db.Foods.ToDictionary(f => f.Name, f => f.Id);
            var dictIng = db.Ingredients.ToDictionary(i => i.Name, i => i.Id);

            // ---------- FOOD <-> INGREDIENT ----------
            var fi = new (string Food, string Ing, int Amount)[]
            {
        ("Bread","Flour",2), ("Bread","Sugar",1), ("Bread","Salt",1), ("Bread","Oil",1),
        ("Cake","Flour",2), ("Cake","Sugar",2), ("Cake","Egg",3), ("Cake","Butter",2), ("Cake","Milk",1),
        ("Fried Chicken","Chicken",1), ("Fried Chicken","Oil",2), ("Fried Chicken","Salt",1), ("Fried Chicken","Pepper",1),
        ("Beef Stew","Beef",2), ("Beef Stew","Onion",1), ("Beef Stew","Garlic",1), ("Beef Stew","Tomato",2),
        ("Salad","Lettuce",2), ("Salad","Tomato",1), ("Salad","Onion",1),
        ("Rice Bowl","Rice",2), ("Rice Bowl","Salt",1),
        ("Tomato Soup","Tomato",3), ("Tomato Soup","Onion",1), ("Tomato Soup","Salt",1),
        ("Omelette","Egg",2), ("Omelette","Milk",1), ("Omelette","Salt",1)
            };
            foreach (var (food, ing, amount) in fi)
                db.FoodIngredients.AddOrUpdate(
                    k => new { k.FoodId, k.IngredientId },
                    new FoodIngredient { FoodId = dictFood[food], IngredientId = dictIng[ing], Amount = amount }
                );
            db.SaveChanges();

            // ---------- MENUS ----------
            db.Menus.AddOrUpdate(m => m.Name,
                new Menu { Name = "Basic Menu", Description = "Bread, Rice, Salad", Cost = 80, Status = "Active" },
                new Menu { Name = "Premium Menu", Description = "Chicken, Beef, Cake", Cost = 180, Status = "Active" },
                new Menu { Name = "Vegan Menu", Description = "Salad, Tomato Soup", Cost = 90, Status = "Active" },
                new Menu { Name = "Kids Menu", Description = "Omelette, Bread", Cost = 60, Status = "Active" }
            );
            db.SaveChanges();

            var dictMenu = db.Menus.ToDictionary(m => m.Name, m => m.Id);

            // ---------- MENU DETAILS ----------
            var md = new (string Menu, string Food, int Amount)[]
            {
        ("Basic Menu","Bread",10), ("Basic Menu","Rice Bowl",20), ("Basic Menu","Salad",10),
        ("Premium Menu","Fried Chicken",15), ("Premium Menu","Beef Stew",12), ("Premium Menu","Cake",8),
        ("Vegan Menu","Salad",20), ("Vegan Menu","Tomato Soup",15), ("Vegan Menu","Bread",10),
        ("Kids Menu","Omelette",15), ("Kids Menu","Bread",15), ("Kids Menu","Cake",6)
            };
            foreach (var (menu, food, amount) in md)
                db.MenuDetails.AddOrUpdate(
                    k => new { k.MenuId, k.FoodId },
                    new MenuDetail { MenuId = dictMenu[menu], FoodId = dictFood[food], Amount = amount }
                );
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
                    UserUsername = "admin",
                    MenuId = dictMenu["Kids Menu"]
                },
                new Party
                {
                    Name = "Wedding",
                    Type = "Ceremony",
                    Status = "Planning",
                    Cost = 1200,
                    Slots = 150,
                    Address = "Thu Duc City",
                    UserUsername = "user1",
                    MenuId = dictMenu["Premium Menu"]
                },
                new Party
                {
                    Name = "Company Year End",
                    Type = "Corporate",
                    Status = "Open",
                    Cost = 800,
                    Slots = 80,
                    Address = "District 7",
                    UserUsername = "user2",
                    MenuId = dictMenu["Basic Menu"]
                },
                new Party
                {
                    Name = "Picnic",
                    Type = "Outdoor",
                    Status = "Open",
                    Cost = 300,
                    Slots = 30,
                    Address = "Cu Chi",
                    UserUsername = "admin",
                    MenuId = dictMenu["Vegan Menu"]
                }
            );
            db.SaveChanges();

            var dictParty = db.Parties.ToDictionary(p => p.Name, p => p.Id);

            // ---------- STAFF <-> PARTY ----------
            var sp = new (string Staff, string Party)[]
            {
        ("admin","Birthday"), ("staff1","Birthday"),
        ("staff2","Wedding"), ("staff3","Wedding"),
        ("staff1","Company Year End"), ("staff2","Company Year End"),
        ("staff3","Picnic")
            };
            foreach (var (staff, party) in sp)
                db.StaffParties.AddOrUpdate(
                    k => new { k.StaffUsername, k.PartyId },
                    new StaffParty { StaffUsername = staff, PartyId = dictParty[party] }
                );
            db.SaveChanges();

            // ---------- PRICE HISTORY (mỗi party-đồ ăn một giá) ----------
            var ph = new[]
            {
        // Birthday (Kids Menu)
        new { Party="Birthday", Food="Omelette",   Cost=20, Amount=15 },
        new { Party="Birthday", Food="Bread",      Cost=8,  Amount=15 },
        new { Party="Birthday", Food="Cake",       Cost=45, Amount=6 },

        // Wedding (Premium)
        new { Party="Wedding",  Food="Fried Chicken", Cost=60, Amount=15 },
        new { Party="Wedding",  Food="Beef Stew",     Cost=85, Amount=12 },
        new { Party="Wedding",  Food="Cake",          Cost=48, Amount=8  },

        // Company Year End (Basic)
        new { Party="Company Year End", Food="Bread",     Cost=9,  Amount=10 },
        new { Party="Company Year End", Food="Rice Bowl", Cost=18, Amount=20 },
        new { Party="Company Year End", Food="Salad",     Cost=22, Amount=10 },

        // Picnic (Vegan)
        new { Party="Picnic", Food="Salad",       Cost=23, Amount=20 },
        new { Party="Picnic", Food="Tomato Soup", Cost=28, Amount=15 },
        new { Party="Picnic", Food="Bread",       Cost=9,  Amount=10 }
    };
            foreach (var x in ph)
                db.PriceHistories.AddOrUpdate(
                    k => new { k.PartyId, k.FoodId },
                    new PriceHistory { PartyId = dictParty[x.Party], FoodId = dictFood[x.Food], Cost = x.Cost, Amount = x.Amount }
                );
            db.SaveChanges();
        }
    }
}
