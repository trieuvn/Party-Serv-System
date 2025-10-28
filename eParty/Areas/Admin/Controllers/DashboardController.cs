using eParty.Areas.Admin.Models;
using eParty.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eParty.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")] 
    public class DashboardController : Controller
    {
        private AppDbContext db = new AppDbContext();
        public ActionResult Index()
        {
            String total_cost = db.Parties.ToList().Sum(f => f.Cost).ToString("n", CultureInfo.GetCultureInfo("vi-VN"));
            var list = new DashboardViewModel
            {
                Providers = db.Providers.ToList(),
                Foods = db.Foods.ToList(),
                Ingredients = db.Ingredients.ToList(),
                TotalCost = total_cost,
                RoleUsers= db.SystemUsers.Where(p=>p.Role == "User").ToList()
            };
            return View(list);
        }
    }
}