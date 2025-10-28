namespace eParty.Migrations
{
    using eParty.Models;
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

        protected override void Seed(eParty.Models.AppDbContext context)
        {
            // --- 1. Tạo Roles nếu chưa có ---
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            if (!roleManager.RoleExists("Admin"))
            {
                var role = new IdentityRole { Name = "Admin" };
                roleManager.Create(role);
            }

            if (!roleManager.RoleExists("Staff"))
            {
                var role = new IdentityRole { Name = "Staff" };
                roleManager.Create(role);
            }

            if (!roleManager.RoleExists("User"))
            {
                var role = new IdentityRole { Name = "User" };
                roleManager.Create(role);
            }

            // --- 2. Tạo User Manager ---
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            // *** QUAN TRỌNG: Cấu hình PasswordValidator giống IdentityConfig để kiểm tra ***
            // (Hoặc đơn giản là đảm bảo mật khẩu dưới đây đủ mạnh)
            userManager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };


            // --- MẬT KHẨU MẶC ĐỊNH MỚI (ĐÁP ỨNG YÊU CẦU) ---
            string defaultPassword = "Pass@123";
            // ---

            // --- 3. Tạo Admin User nếu chưa có ---
            var adminEmail = "admin@eparty.com";
            if (userManager.FindByEmail(adminEmail) == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    PhoneNumber = "0901112222",
                    PhoneNumberConfirmed = true
                };

                // Tạo user Identity với mật khẩu MỚI
                var result = userManager.Create(adminUser, defaultPassword); // <-- Sử dụng mật khẩu mới

                if (result.Succeeded)
                {
                    userManager.AddToRole(adminUser.Id, "Admin");

                    var systemAdmin = new SystemUser
                    {
                        Username = adminEmail,
                        Password = defaultPassword, // <-- Cập nhật mật khẩu mới (nếu cần lưu)
                        FirstName = "Admin",
                        LastName = "Account",
                        Email = adminEmail,
                        PhoneNumber = "0901112222",
                        Role = "Admin",
                        Salary = 20000000
                    };
                    context.SystemUsers.AddOrUpdate(u => u.Username, systemAdmin);
                    System.Diagnostics.Debug.WriteLine($"Admin User '{adminEmail}' created successfully.");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Error creating Admin User '{adminEmail}': " + string.Join(", ", result.Errors));
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Admin User '{adminEmail}' already exists.");
            }

            // --- 4. Tạo Staff User nếu chưa có ---
            var staffEmail = "staff1@eparty.com";
            if (userManager.FindByEmail(staffEmail) == null)
            {
                var staffUser = new ApplicationUser
                {
                    UserName = staffEmail,
                    Email = staffEmail,
                    EmailConfirmed = true,
                    PhoneNumber = "0912345678",
                    PhoneNumberConfirmed = true
                };

                var result = userManager.Create(staffUser, defaultPassword); // <-- Sử dụng mật khẩu mới

                if (result.Succeeded)
                {
                    userManager.AddToRole(staffUser.Id, "Staff");

                    var systemStaff = new SystemUser
                    {
                        Username = staffEmail,
                        Password = defaultPassword, // <-- Cập nhật mật khẩu mới (nếu cần lưu)
                        FirstName = "Staff",
                        LastName = "01",
                        Email = staffEmail,
                        PhoneNumber = "0912345678",
                        Role = "Staff",
                        Salary = 8000000
                    };
                    context.SystemUsers.AddOrUpdate(u => u.Username, systemStaff);
                    System.Diagnostics.Debug.WriteLine($"Staff User '{staffEmail}' created successfully.");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Error creating Staff User '{staffEmail}': " + string.Join(", ", result.Errors));
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Staff User '{staffEmail}' already exists.");
            }

            // --- 5. Lưu các thay đổi vào DB ---
            try
            {
                context.SaveChanges();
                System.Diagnostics.Debug.WriteLine("Seed changes saved successfully.");
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                System.Diagnostics.Debug.WriteLine("SaveChanges Error during Seed:");
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        System.Diagnostics.Debug.WriteLine($"  Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                    }
                }
                throw;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Generic SaveChanges Error during Seed: " + ex.ToString());
                throw;
            }

            // Seed Categories (giữ nguyên)
            context.Categories.AddOrUpdate(c => c.Name,
               new Category { Name = "Món Khai Vị" },
               new Category { Name = "Món Chính - Heo" },
               new Category { Name = "Món Chính - Gà" },
               new Category { Name = "Món Chính - Bò" },
               new Category { Name = "Món Chính - Hải Sản" },
               new Category { Name = "Món Lẩu" },
               new Category { Name = "Món Tráng Miệng" },
               new Category { Name = "Đồ Uống" }
           );
            context.SaveChanges();

        }
    }
}