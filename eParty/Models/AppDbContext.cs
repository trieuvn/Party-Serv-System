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

        protected override void OnModelCreating(DbModelBuilder mb)
        {
            base.OnModelCreating(mb);
            mb.Conventions.Remove<PluralizingTableNameConvention>();

            // TPH cho Staff kế thừa User: EF tự thêm Discriminator

            // Composite Keys (đã đặt DataAnnotations ở model) – cấu hình quan hệ:
            mb.Entity<MenuDetail>()
              .HasRequired(md => md.Menu).WithMany(m => m.MenuDetails)
              .HasForeignKey(md => md.MenuId).WillCascadeOnDelete(false);

            mb.Entity<MenuDetail>()
              .HasRequired(md => md.Food).WithMany(f => f.MenuDetails)
              .HasForeignKey(md => md.FoodId).WillCascadeOnDelete(false);

            mb.Entity<FoodIngredient>()
              .HasRequired(fi => fi.Food).WithMany(f => f.FoodIngredients)
              .HasForeignKey(fi => fi.FoodId).WillCascadeOnDelete(false);

            mb.Entity<FoodIngredient>()
              .HasRequired(fi => fi.Ingredient).WithMany(i => i.FoodIngredients)
              .HasForeignKey(fi => fi.IngredientId).WillCascadeOnDelete(false);

            mb.Entity<PriceHistory>()
              .HasRequired(ph => ph.Party).WithMany(p => p.PriceHistories)
              .HasForeignKey(ph => ph.PartyId).WillCascadeOnDelete(false);

            mb.Entity<PriceHistory>()
              .HasRequired(ph => ph.Food).WithMany(f => f.PriceHistories)
              .HasForeignKey(ph => ph.FoodId).WillCascadeOnDelete(false);

            mb.Entity<StaffParty>()
              .HasRequired(sp => sp.Party).WithMany(p => p.StaffParties)
              .HasForeignKey(sp => sp.PartyId).WillCascadeOnDelete(false);

            mb.Entity<Provider>()
              .HasRequired(pv => pv.Ingredient).WithMany(i => i.Providers)
              .HasForeignKey(pv => pv.IngredientId).WillCascadeOnDelete(false);
        }
    }
}