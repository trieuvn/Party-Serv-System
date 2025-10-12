using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace eParty.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("DefaultConnection") { }

        public DbSet<User> Users { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Party> Parties { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<MenuDetail> MenuDetails { get; set; }
        public DbSet<Food> Foods { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<FoodIngredient> FoodIngredients { get; set; }
        public DbSet<Provider> Providers { get; set; }
        public DbSet<PriceHistory> PriceHistories { get; set; }
        public DbSet<StaffParty> StaffParties { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<UserDiscount> UserDiscounts { get; set; }
        public DbSet<Rate> Rates { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Poster> Posters { get; set; }
        public DbSet<Partner> Partners { get; set; }

        protected override void OnModelCreating(DbModelBuilder mb)
        {
            base.OnModelCreating(mb);
            mb.Conventions.Remove<PluralizingTableNameConvention>();

            // Tắt cascade để tránh multiple cascade paths (nhiều bảng join)
            mb.Entity<MenuDetail>()
              .HasRequired(x => x.MenuRef).WithMany(m => m.MenuDetails)
              .HasForeignKey(x => x.Menu).WillCascadeOnDelete(false);
            mb.Entity<MenuDetail>()
              .HasRequired(x => x.FoodRef).WithMany(f => f.MenuDetails)
              .HasForeignKey(x => x.Food).WillCascadeOnDelete(false);

            mb.Entity<FoodIngredient>()
              .HasRequired(x => x.FoodRef).WithMany(f => f.FoodIngredients)
              .HasForeignKey(x => x.Food).WillCascadeOnDelete(false);
            mb.Entity<FoodIngredient>()
              .HasRequired(x => x.IngredientRef).WithMany(i => i.FoodIngredients)
              .HasForeignKey(x => x.Ingredient).WillCascadeOnDelete(false);

            mb.Entity<PriceHistory>()
              .HasRequired(x => x.PartyRef).WithMany(p => p.PriceHistories)
              .HasForeignKey(x => x.Party).WillCascadeOnDelete(false);
            mb.Entity<PriceHistory>()
              .HasRequired(x => x.FoodRef).WithMany(f => f.PriceHistories)
              .HasForeignKey(x => x.Food).WillCascadeOnDelete(false);

            mb.Entity<StaffParty>()
              .HasRequired(x => x.PartyRef).WithMany(p => p.StaffParties)
              .HasForeignKey(x => x.Party).WillCascadeOnDelete(false);

            mb.Entity<Provider>()
              .HasRequired(x => x.IngredientRef).WithMany(i => i.Providers)
              .HasForeignKey(x => x.Ingredient).WillCascadeOnDelete(false);

            mb.Entity<Rate>()
              .HasRequired(r => r.PartyRef).WithMany(p => p.Rates)
              .HasForeignKey(r => r.Party).WillCascadeOnDelete(false);
        }
    }
}