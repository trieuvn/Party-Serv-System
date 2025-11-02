using eParty.Models;
using Microsoft.AspNet.Identity; // Cần cho GetUserId()
using Microsoft.AspNet.Identity.Owin; // Cần cho GetUserManager
using System;
using System.Data.Entity; // Cần cho Include (nếu dùng)
using System.Globalization;
using System.Linq;
using System.Threading.Tasks; // Cần cho async Task
using System.Web;
using System.Web.Mvc;

namespace eParty.Areas.Staff.Controllers
{
    [Authorize(Roles = "Staff")]
    public class DashboardController : Controller
    {
        private AppDbContext db = new AppDbContext();
        private ApplicationUserManager _userManager;

        // Property để lấy UserManager từ OwinContext
        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }

        public async Task<ActionResult> Index() // Chuyển thành async Task
        {
            long dailySalesStaffValue = 0;
            DateTime? firstPartyDate = null; // Ngày bắt đầu tính trung bình

            try
            {
                // 1. Lấy thông tin Staff đang đăng nhập
                var userId = User.Identity.GetUserId();
                var identityUser = await UserManager.FindByIdAsync(userId); // Dùng async

                if (identityUser != null)
                {
                    var staffUsername = identityUser.Email; // Hoặc UserName tùy thuộc vào cách bạn lưu trong SystemUser

                    // 2. Tìm các Party IDs mà Staff này tham gia
                    var staffPartyIds = db.StaffParties
                                          .Where(sp => sp.Staff == staffUsername)
                                          .Select(sp => sp.Party)
                                          .ToList();

                    if (staffPartyIds.Any())
                    {
                        // 3. Tìm ngày diễn ra Party sớm nhất của Staff này để tính số ngày
                        firstPartyDate = db.Parties
                                           .Where(p => staffPartyIds.Contains(p.Id) && p.BeginTime.HasValue)
                                           .Min(p => p.BeginTime); // Chỉ lấy ngày sớm nhất

                        // 4. Lọc PriceHistory chỉ cho các Party mà Staff tham gia
                        var relevantPriceHistories = db.PriceHistories
                                                       .Where(ph => staffPartyIds.Contains(ph.Party))
                                                       .ToList(); // Lấy dữ liệu về để tính toán

                        // 5. Tính tổng doanh thu từ các PriceHistory liên quan
                        long totalSalesForStaff = relevantPriceHistories.Sum(ph => (long)ph.Cost * (long)ph.Amount);

                        // 6. Tính số ngày đã trôi qua kể từ Party đầu tiên của Staff
                        if (firstPartyDate.HasValue)
                        {
                            // Dùng .Date để so sánh ngày, bỏ qua giờ
                            double daysElapsed = Math.Max(1.0, (DateTime.Now.Date - firstPartyDate.Value.Date).TotalDays); // Đảm bảo ít nhất là 1 ngày

                            // 7. Tính Daily Sales trung bình
                            if (totalSalesForStaff > 0)
                            {
                                dailySalesStaffValue = (long)(totalSalesForStaff / daysElapsed);
                            }
                        }
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "Không tìm thấy thông tin nhân viên.";
                }
            }
            catch (Exception ex)
            {
                // Ghi log lỗi ở đây nếu cần
                System.Diagnostics.Debug.WriteLine($"Lỗi tính Daily Sales cho Staff: {ex.Message}");
                ViewBag.ErrorMessage = "Có lỗi xảy ra khi tính toán doanh thu.";
            }

            // 8. Định dạng và truyền vào ViewBag
            ViewBag.DailySalesStaff = dailySalesStaffValue.ToString("n0", CultureInfo.GetCultureInfo("vi-VN")) + " VND";
            ViewBag.CalculationStartDate = firstPartyDate?.ToString("dd/MM/yyyy") ?? "N/A";
            ViewBag.TodayDate = DateTime.Now.ToString("dd/MM/yyyy");


            // Không cần truyền các ViewModel khác nữa
            return View();
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