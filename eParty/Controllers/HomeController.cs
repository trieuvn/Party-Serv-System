using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eParty.Models;
using System.Data.Entity;

namespace eParty.Controllers
{
    public class HomeController : Controller
    {
        private AppDbContext db = new AppDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Service()
        {
            return View();
        }

        public ActionResult Event()
        {
            return View();
        }

        public ActionResult Menu()
        {
            var categoriesWithFoods = db.Categories
                                        .Include(c => c.Foods)
                                        .ToList();

            var viewModel = new MenuViewModel
            {
                CategoriesWithFoods = categoriesWithFoods
            };

            return View(viewModel);
        }

        public ActionResult Book()
        {
            return View();
        }

        public ActionResult Team()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            if (email == "admin@gmail.com" && password == "123")
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
            else
            {
                ViewBag.ErrorMessage = "Email hoặc mật khẩu không chính xác.";
                return View(); 
            }
        }

        public ActionResult News()
        {
            return View();
        }
    }
}