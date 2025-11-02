using eParty.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin; // Thêm dòng này
using Newtonsoft.Json;
using System;
using System.Data.Entity; // Thêm dòng này
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eParty.Areas.Staff.Controllers
{
    [Authorize(Roles = "Staff")] // Đảm bảo chỉ Staff mới truy cập được
    public class CalendarController : Controller
    {
        private AppDbContext db = new AppDbContext();
        private ApplicationUserManager _userManager;

        // Thêm constructor để có thể inject UserManager (nếu bạn có cấu hình DI)
        // Hoặc lấy UserManager từ OwinContext như bên dưới
        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }

        public ActionResult Index()
        {
            try
            {
                // Lấy ID của Staff đang đăng nhập
                var userId = User.Identity.GetUserId();
                // Tìm SystemUser tương ứng (giả sử Username trong SystemUser là Email trong Identity)
                var identityUser = UserManager.FindById(userId);
                if (identityUser == null)
                {
                    // Xử lý trường hợp không tìm thấy user
                    ViewBag.ServerEventsJson = new HtmlString("[]");
                    ViewBag.CalendarError = "Không thể xác định người dùng.";
                    return View();
                }
                var staffUsername = identityUser.Email; // Hoặc identityUser.UserName tùy cấu hình

                // Lấy danh sách các Party ID mà Staff này tham gia từ bảng StaffParty
                var partyIds = db.StaffParties
                                 .Where(sp => sp.Staff == staffUsername)
                                 .Select(sp => sp.Party)
                                 .ToList();

                // Lấy thông tin chi tiết các Party đó
                var parties = db.Parties
                                .Where(p => partyIds.Contains(p.Id) && p.BeginTime.HasValue && p.EndTime.HasValue)
                                .ToList();

                // Chuyển đổi dữ liệu sang định dạng JSON cho calendar
                var eventsData = parties.Select(p => new
                {
                    id = p.Id.ToString(), // ID không cần thiết lắm vì là read-only
                    title = p.Name,
                    startTime = p.BeginTime.Value.ToString("yyyy-MM-ddTHH:mm:ss"), // Định dạng ISO không có Z
                    endTime = p.EndTime.Value.ToString("yyyy-MM-ddTHH:mm:ss"), // Định dạng ISO không có Z
                    description = p.Description ?? "",
                    color = GetColorFromStatus(p.Status), // Hàm này có thể copy từ Admin/CalendarController
                    // Không cần reminder hoặc các trường khác cho view read-only
                }).ToList();

                ViewBag.ServerEventsJson = new HtmlString(JsonConvert.SerializeObject(eventsData));
                ViewBag.IsReadOnly = true; // Thêm biến này để báo cho View biết là chế độ chỉ xem
            }
            catch (Exception ex)
            {
                // Ghi log lỗi (quan trọng)
                System.Diagnostics.Debug.WriteLine("Lỗi khi lấy dữ liệu lịch cho Staff: " + ex.Message);
                ViewBag.ServerEventsJson = new HtmlString("[]");
                ViewBag.CalendarError = "Đã xảy ra lỗi khi tải dữ liệu lịch.";
                ViewBag.IsReadOnly = true;
            }

            return View();
        }

        // Copy hàm này từ Admin/CalendarController.cs
        private string GetColorFromStatus(string status)
        {
            switch (status?.ToLower())
            {
                case "upcoming": return "#4e73df"; // Lam
                case "ongoing": return "#1cc88a";  // Lục
                case "completed": return "#858796"; // Xám
                case "cancelled": return "#e74a3b"; // Đỏ
                default: return "#5a5c69"; // Xám đậm
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }
            }
            base.Dispose(disposing);
        }
    }
}