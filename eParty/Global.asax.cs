using eParty.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace eParty
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //Database.SetInitializer(new MigrateDatabaseToLatestVersion<AppDbContext, Migrations.Configuration>());

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            CreateRoles();
        }
        private void CreateRoles()
        {
            AppDbContext context = new AppDbContext();
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            // Kiểm tra và tạo vai trò "Admin" 
            if (!roleManager.RoleExists("Admin"))
            {
                var role = new IdentityRole(); role.Name = "Admin";
                roleManager.Create(role);
            }
            // Kiểm tra và tạo vai trò "User" 
            if (!roleManager.RoleExists("User"))
            {
                var role = new IdentityRole(); role.Name = "User"; roleManager.Create(role);
            }
            if (!roleManager.RoleExists("Staff"))
            {
                var role = new IdentityRole(); role.Name = "Staff"; roleManager.Create(role);
            }
        }
    }
}
