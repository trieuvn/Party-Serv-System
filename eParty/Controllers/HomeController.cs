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
            // Lấy tất cả các Category và đồng thời tải tất cả Food tương ứng
            var categoriesWithFoods = db.Categories
                                        .Include(c => c.Foods)
                                        .ToList();

            // Tạo ViewModel và gán dữ liệu vào
            var viewModel = new MenuViewModel
            {
                CategoriesWithFoods = categoriesWithFoods
            };

            // Trả về View cùng với ViewModel
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
        public ActionResult News()
        {
            return View();
        }
    }
}