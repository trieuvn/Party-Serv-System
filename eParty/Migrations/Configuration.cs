namespace eParty.Migrations
{
    using eParty.Models; // <-- Namespace Models của bạn
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<eParty.Models.AppDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        // PHƯƠNG THỨC NÀY SẼ CHẠY KHI BẠN GÕ LỆNH "Update-Database"
        protected override void Seed(eParty.Models.AppDbContext context)
        {
            // Tạo RoleManager và UserManager để làm việc với database
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            // 1. TẠO VAI TRÒ "Admin" NẾU CHƯA TỒN TẠI
            if (!roleManager.RoleExists("Admin"))
            {
                var role = new IdentityRole();
                role.Name = "Admin";
                roleManager.Create(role);
            }

            // 2. TẠO NGƯỜI DÙNG ADMIN NẾU CHƯA TỒN TẠI
            var adminUser = userManager.FindByName("admin@gmail.com");
            if (adminUser == null)
            {
                // Tạo một người dùng mới
                adminUser = new ApplicationUser()
                {
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    EmailConfirmed = true // Email đã được xác nhận sẵn
                };
                // Tạo người dùng với mật khẩu "Admin@123"
                var result = userManager.Create(adminUser, "Admin@123");
                if (result.Succeeded)
                {
                    // 3. GÁN VAI TRÒ "Admin" CHO NGƯỜI DÙNG VỪA TẠO
                    userManager.AddToRole(adminUser.Id, "Admin");
                }
            }
        }
    }
}