using eParty.Models;
using eParty.Service;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace eParty.Controllers
{
    public class HomeController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // ================== TRANG CHỦ ==================
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

        // ================== TRANG ĐẶT TIỆC ==================
        [HttpGet]
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
                .Include("FoodRef")
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
                menuDetails = menuDetailsDto
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Book(Party model)
        {
            // ✅ Kiểm tra đăng nhập
            if (Session["UserEmail"] == null)
            {
                TempData["LoginMessage"] = "Vui lòng đăng nhập để đặt tiệc.";
                return RedirectToAction("Login", "Account");
            }

            // ✅ Lấy email người dùng
            string userEmail = Session["UserEmail"].ToString();
            var currentUser = db.AppUsers.FirstOrDefault(u => u.Email == userEmail);
            if (currentUser == null)
            {
                TempData["Error"] = "Tài khoản không tồn tại.";
                return RedirectToAction("Login", "Account");
            }

            // Tính toán EndTime (ví dụ mặc định 2 giờ)
            if (model.BeginTime.HasValue && model.Slots > 0)
            {
                model.EndTime = model.BeginTime.Value.AddHours(2);
            }

            // Gán thông tin cơ bản
            model.User = currentUser.Username;
            model.Owner = currentUser;
            model.Status = "Active";
            model.Type = Request.Form["Type"];

            if (int.TryParse(Request.Form["MenuId"], out int menuId))
            {
                model.Menu = menuId;
                model.MenuRef = db.Menus.Find(menuId);
            }
            else
            {
                model.Menu = null;
                model.MenuRef = null;
            }
                model.CreatedDate = DateTime.Now;

                try
                {
                    // Sinh mã xác nhận
                    string confirmationCode = new Random().Next(100000, 999999).ToString();
                    Session["ConfirmCode"] = confirmationCode;
                    Session["PendingParty"] = model;

                    // Gửi email xác nhận
                    using (var smtpClient = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtpClient.EnableSsl = true;
                        smtpClient.Credentials = new NetworkCredential("phucnguyen2716@gmail.com", "ooag phsw rzno onxy");

                        var mail = new MailMessage("your_email@gmail.com", currentUser.Email)
                        {
                            Subject = "Xác nhận đăng ký đặt tiệc",
                            Body = $"Mã xác nhận của bạn là: {confirmationCode}"
                        };

                        smtpClient.Send(mail);
                    }

                    return RedirectToAction("ConfirmEmail");
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Không thể gửi email xác nhận: " + ex.Message;
                    return RedirectToAction("Book");
                }
            }

        // ================== XÁC NHẬN EMAIL ==================
        [HttpGet]
        public ActionResult ConfirmEmail()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmEmail(string confirmCode)
        {
            // Lấy code và pending party từ session
            var savedCode = Session["ConfirmCode"] as string;
            var pendingParty = Session["PendingParty"] as Party;

            if (savedCode == null || pendingParty == null)
            {
                TempData["Error"] = "Phiên xác nhận đã hết hạn. Vui lòng đặt lại.";
                return RedirectToAction("Book");
            }

            if (confirmCode == savedCode)
            {
                try
                {
                    var newParty = new Party
                    {
                        Name = pendingParty.Name,
                        Image = pendingParty.Image,
                        Type = pendingParty.Type,
                        Status = pendingParty.Status,
                        Cost = pendingParty.Cost,
                        BeginTime = pendingParty.BeginTime,
                        EndTime = pendingParty.EndTime,
                        CreatedDate = DateTime.Now,
                        Description = pendingParty.Description,
                        Slots = pendingParty.Slots,
                        Address = pendingParty.Address,
                        Latitude = pendingParty.Latitude,
                        Longitude = pendingParty.Longitude,
                        User = pendingParty.User,
                        Menu = pendingParty.Menu

                    };

                    db.Parties.Add(newParty);
                    db.SaveChanges();

                    // Xóa session
                    Session.Remove("ConfirmCode");
                    Session.Remove("PendingParty");

                    TempData["Success"] = "Đặt tiệc thành công! Thông tin của bạn đã được lưu.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Không thể lưu dữ liệu: " + ex.Message;
                    return RedirectToAction("Book");
                }
            }
            else
            {
                TempData["Error"] = "Mã xác nhận không đúng. Vui lòng thử lại.";
                return View();
            }
        }


        // ================== LƯU MENU TÙY CHỈNH ==================
        [HttpPost]
        public JsonResult SaveCustomMenuBase64()
        {
            try
            {
                string json;
                using (var reader = new System.IO.StreamReader(Request.InputStream))
                {
                    json = reader.ReadToEnd();
                }

                var request = JsonConvert.DeserializeObject<CustomMenuRequest>(json);
                if (request == null)
                    throw new Exception("Không nhận được dữ liệu từ client");

                // Lưu Menu
                var menu = new Menu
                {
                    Name = request.Name,
                    Image = SaveBase64Image(request.ImageBase64)
                };
                db.Menus.Add(menu);
                db.SaveChanges();

                // Lưu MenuDetail
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

        public class CustomMenuRequest
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

        // ================== TEAM, LOGIN, NEWS ==================
        public ActionResult Team() => View();
        public ActionResult Login() => View();

        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            if (email == "admin@gmail.com" && password == "123")
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
            ViewBag.ErrorMessage = "Email hoặc mật khẩu không chính xác.";
            return View();
        }

        public ActionResult News() => View();

        [Authorize]
        public ActionResult RedirectToAdmin()
        {
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
            TempData["AdminMessage"] = "Tài khoản hiện tại không đủ quyền hạn. Vui lòng đăng nhập bằng tài khoản Admin để truy cập.";
            return RedirectToAction("Index", "Home");
        }
    }
}
