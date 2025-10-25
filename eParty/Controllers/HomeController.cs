using eParty.Models;
using eParty.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
                Unit = f.Unit,
            }).ToList();
            var menuDetailsDto = db.MenuDetails
                .Include(md => md.FoodRef)
                .Select(md => new MenuDetailDto
                {
                    MenuId = md.Menu,
                    FoodId = md.Food,
                    FoodName = md.FoodRef.Name,
                    Amount = md.Amount
                }).ToList();

            var viewModel = new HomeViewModel
            {
                Menus = db.Menus.ToList(),
                DtoFoods = foodsDto,
                menuDetails= menuDetailsDto
            };

            return View(viewModel);

        }
        [HttpPost]
        public JsonResult SaveCustomMenuBase64()
        {
            try
            {
                // đọc JSON
                string json;
                using (var reader = new System.IO.StreamReader(Request.InputStream))
                {
                    json = reader.ReadToEnd();
                }

                var request = JsonConvert.DeserializeObject<CustomMenuRequest>(json);

                if (request == null)
                    throw new Exception("Không nhận được dữ liệu từ client");

                // 1. Lưu Menu
                var menu = new Menu
                {
                    Name = request.Name,
                    Image = SaveBase64Image(request.ImageBase64), // hàm xử lý base64 -> lưu file, trả về path
                };
                db.Menus.Add(menu);
                db.SaveChanges(); // để lấy menu.Id

                // 2. Lưu MenuDetail
                foreach (var food in request.Foods)
                {
                    var detail = new MenuDetail
                    {
                        Menu = menu.Id,
                        Food = food.FoodId,
                        Amount = food.Amount
                    };
                    db.MenuDetails.Add(detail);
                }
                db.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Hàm xử lý base64 -> lưu file -> trả về path lưu trong DB
        private string SaveBase64Image(string base64)
        {
            try
            {
                var parts = base64.Split(',');
                var base64Data = parts.Length > 1 ? parts[1] : parts[0];
                byte[] bytes = Convert.FromBase64String(base64Data);

                var folderPath = Server.MapPath("~/Uploads/Menus");
                if (!System.IO.Directory.Exists(folderPath))
                    System.IO.Directory.CreateDirectory(folderPath);

                var fileName = Guid.NewGuid().ToString() + ".png";
                var path = System.IO.Path.Combine(folderPath, fileName);

                System.IO.File.WriteAllBytes(path, bytes);

                return "/Uploads/Menus/" + fileName;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lưu ảnh menu: " + ex.Message);
            }
        }





        // DTO
        public class CustomMenuRequest
        {
            public string Name { get; set; }
            public string ImageBase64 { get; set; }
            public List<MenuFoodDto> Foods { get; set; }
        }
        public class SaveMenuRequest
        {
            public string Name { get; set; }
            public string ImageBase64 { get; set; }
            public List<MenuFoodDto> Foods { get; set; }
        }

        public class MenuFoodDto
        {
            public int FoodId { get; set; }
            public int Amount { get; set; }
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