using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eParty.Models;
using System.Data.Entity;
using eParty.Service;

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
            var foodsDto = db.Foods.Select(f => new FoodDto
            {
                Id = f.Id,
                Name = f.Name,
                ImageUrl = f.Image,
            }).ToList();

            var viewModel = new HomeViewModel
            {
                Menus = db.Menus.ToList(),
                DtoFoods = foodsDto
            };

            return View(viewModel);

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
    
    [Authorize] // Yêu cầu người dùng phải đăng nhập trước khi kiểm tra
        public ActionResult RedirectToAdmin()
        {
            // Kiểm tra xem người dùng có vai trò "Admin" hay không
            if (User.IsInRole("Admin"))
            {
                // Nếu là Admin, chuyển hướng đến trang Dashboard
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
            else
            {
                // Nếu không phải Admin, lưu một thông báo vào TempData và chuyển hướng về trang chủ
                TempData["AdminMessage"] = "Tài khoản hiện tại không đủ quyền hạn. Vui lòng đăng nhập bằng tài khoản Admin để truy cập.";
                return RedirectToAction("Index", "Home");
            }
        }
    }
}