using eParty.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eParty.Areas.Admin.Controllers
{
    public class DashboardController : Controller
    {
        private AppDbContext db = new AppDbContext();
        public ActionResult Index()
        {
            int totalfoods = db.Foods.ToList().Sum(f => f.Cost);
            var list = new HomeViewModel
            {
                Providers = db.Providers.ToList(),
                Foods = db.Foods.ToList(),
                Ingredients = db.Ingredients.ToList(),
                TotalCost = totalfoods,
                Users= db.Users.ToList()
            };
            return View(list);
        }
    }
}