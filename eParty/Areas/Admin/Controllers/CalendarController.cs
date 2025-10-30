using eParty.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace eParty.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CalendarController : Controller
    {
        private AppDbContext db = new AppDbContext();

        public ActionResult Index()
        {
            try
            {
                var parties = db.Parties
                                .Include(p => p.Owner)
                                .Include(p => p.MenuRef)
                                .Where(p => p.BeginTime.HasValue && p.EndTime.HasValue)
                                .ToList();

                var eventsData = parties.Select(p => new
                {
                    id = p.Id.ToString(),
                    title = p.Name,
                    startTime = p.BeginTime.Value.ToString("yyyy-MM-ddTHH:mm:ss"),
                    endTime = p.EndTime.Value.ToString("yyyy-MM-ddTHH:mm:ss"),
                    description = p.Description ?? "",
                    color = GetColorFromStatus(p.Status),
                    reminder = false,
                    type = p.Type,
                    image = p.Image,
                    status = p.Status,
                    userId = p.User,
                    menuId = p.Menu,
                    userName = p.Owner?.Username ?? "N/A",
                    menuName = p.MenuRef?.Name ?? "N/A",

                    // SỬA ĐỔI: Thêm 3 trường mới
                    address = p.Address,
                    latitude = p.Latitude,
                    longitude = p.Longitude
                }).ToList();

                ViewBag.ServerEventsJson = new HtmlString(JsonConvert.SerializeObject(eventsData));

                ViewBag.UsersList = new SelectList(db.SystemUsers.ToList(), "Username", "Username");
                ViewBag.MenusList = new SelectList(db.Menus.ToList(), "Id", "Name");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Lỗi khi lấy dữ liệu sự kiện cho Calendar: " + ex.Message);
                ViewBag.ServerEventsJson = new HtmlString("[]");
                ViewBag.CalendarError = "Đã xảy ra lỗi khi tải dữ liệu sự kiện.";

                ViewBag.UsersList = new SelectList(new List<SystemUser>(), "Username", "Username");
                ViewBag.MenusList = new SelectList(new List<Menu>(), "Id", "Name");
            }

            return View();
        }

        private string GetColorFromStatus(string status)
        {
            switch (status?.ToLower())
            {
                case "pending": return "#f6c23e";
                case "upcoming": return "#4e73df";
                case "ongoing": return "#1cc88a";
                case "completed": return "#858796";
                case "cancelled": return "#e74a3b";
                default: return "#5a5c69";
            }
        }

        // ===================================================================
        // SỬA ĐỔI: API TẠO MỚI (Thêm address, latitude, longitude)
        // ===================================================================

        [HttpPost]
        public async Task<JsonResult> CreateEvent(string title, string startTime, string endTime, string description, string color,
            string user, int? menu, string type, string image,
            string address, double? latitude, double? longitude) // <-- THÊM MỚI
        {
            try
            {
                DateTime newBeginTime = DateTime.Parse(startTime);
                DateTime newEndTime = DateTime.Parse(endTime);

                DateTime checkStartDate = newBeginTime.AddDays(-3).Date;
                DateTime checkEndDate = newBeginTime.AddDays(3).Date.AddDays(1).AddTicks(-1);

                bool hasConflict = await db.Parties.AnyAsync(p =>
                    (p.Status == "Upcoming" || p.Status == "Ongoing") &&
                    p.BeginTime.HasValue &&
                    p.BeginTime.Value >= checkStartDate &&
                    p.BeginTime.Value <= checkEndDate
                );

                string finalStatus = hasConflict ? "Pending" : "Upcoming";

                var newParty = new Party
                {
                    Name = title,
                    BeginTime = newBeginTime,
                    EndTime = newEndTime,
                    Description = description,
                    Status = finalStatus,
                    Cost = 0,
                    Slots = 0,
                    CreatedDate = DateTime.Now,
                    User = string.IsNullOrEmpty(user) ? null : user,
                    Menu = menu,
                    Type = type,
                    Image = string.IsNullOrEmpty(image) ? null : image,

                    // THÊM MỚI
                    Address = address,
                    Latitude = latitude,
                    Longitude = longitude
                };

                db.Parties.Add(newParty);
                await db.SaveChangesAsync();

                return Json(new { success = true, id = newParty.Id, newStatus = finalStatus });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ===================================================================
        // SỬA ĐỔI: API CẬP NHẬT (Thêm address, latitude, longitude)
        // ===================================================================
        [HttpPost]
        public async Task<JsonResult> UpdateEvent(int id, string title, string startTime, string endTime, string description, string color,
            string user, int? menu, string type, string image,
            string address, double? latitude, double? longitude) // <-- THÊM MỚI
        {
            try
            {
                var partyToUpdate = await db.Parties.FindAsync(id);
                if (partyToUpdate == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy sự kiện" });
                }

                DateTime newBeginTime = DateTime.Parse(startTime);

                partyToUpdate.Name = title;
                partyToUpdate.BeginTime = newBeginTime;
                partyToUpdate.EndTime = DateTime.Parse(endTime);
                partyToUpdate.Description = description;
                partyToUpdate.User = string.IsNullOrEmpty(user) ? null : user;
                partyToUpdate.Menu = menu;
                partyToUpdate.Type = type;
                if (image != null)
                {
                    partyToUpdate.Image = string.IsNullOrEmpty(image) ? null : image;
                }

                // THÊM MỚI
                partyToUpdate.Address = address;
                partyToUpdate.Latitude = latitude;
                partyToUpdate.Longitude = longitude;

                db.Entry(partyToUpdate).State = EntityState.Modified;
                await db.SaveChangesAsync();

                return Json(new { success = true, id = partyToUpdate.Id });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> DeleteEvent(int id)
        {
            try
            {
                var partyToDelete = await db.Parties.FindAsync(id);
                if (partyToDelete == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy sự kiện" });
                }

                db.Parties.Remove(partyToDelete);
                await db.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ===================================================================
        // API DUYỆT (Giữ nguyên)
        // ===================================================================
        [HttpPost]
        public async Task<JsonResult> ApproveEvent(int id, bool forceApprove = false)
        {
            try
            {
                var partyToApprove = await db.Parties.FindAsync(id);
                if (partyToApprove == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy sự kiện." });
                }

                if (partyToApprove.Status != "Pending")
                {
                    return Json(new { success = false, isWarning = false, message = "Sự kiện này không ở trạng thái 'Pending'." });
                }

                var beginTime = partyToApprove.BeginTime;
                var endTime = partyToApprove.EndTime;

                if (!beginTime.HasValue || !endTime.HasValue)
                {
                    return Json(new { success = false, isWarning = false, message = "Sự kiện thiếu thời gian bắt đầu hoặc kết thúc." });
                }

                // 1. Kiểm tra XUNG ĐỘT CHÍNH XÁC (Trùng giờ)
                var exactConflicts = await db.Parties.Where(p =>
                    p.Id != id &&
                    (p.Status == "Upcoming" || p.Status == "Ongoing") &&
                    p.BeginTime.HasValue && p.EndTime.HasValue &&
                    (beginTime.Value < p.EndTime.Value && endTime.Value > p.BeginTime.Value)
                ).ToListAsync();

                if (exactConflicts.Any())
                {
                    var conflictingPartyName = exactConflicts.First().Name;
                    return Json(new { success = false, isWarning = false, message = $"Lỗi: Lịch bị trùng giờ với sự kiện '{conflictingPartyName}'." });
                }

                // 2. Nếu là Lần 2 (forceApprove = true), bỏ qua kiểm tra lân cận và duyệt luôn
                if (forceApprove)
                {
                    partyToApprove.Status = "Upcoming";
                    db.Entry(partyToApprove).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return Json(new { success = true, newStatus = "Upcoming", message = "Đã duyệt (bỏ qua cảnh báo)." });
                }

                // 3. Nếu là Lần 1 (forceApprove = false), kiểm tra XUNG ĐỘT LÂN CẬN (+/- 1 ngày)
                DateTime checkStartDate = beginTime.Value.Date.AddDays(-1);
                DateTime checkEndDate = beginTime.Value.Date.AddDays(1).AddDays(1).AddTicks(-1);

                var proximalConflicts = await db.Parties.Where(p =>
                   p.Id != id &&
                   (p.Status == "Upcoming" || p.Status == "Ongoing") &&
                   p.BeginTime.HasValue &&
                   p.BeginTime.Value >= checkStartDate &&
                   p.BeginTime.Value <= checkEndDate
               ).ToListAsync();

                if (proximalConflicts.Any())
                {
                    // 3a. Nếu có xung đột lân cận -> Gửi cảnh báo
                    string warningMessage = "CẢNH BÁO: Lịch này gần các sự kiện:\n";
                    foreach (var conflict in proximalConflicts.Take(3))
                    {
                        warningMessage += $"- '{conflict.Name}' (Lúc {conflict.BeginTime.Value:HH:mm dd/MM})\n";
                    }
                    warningMessage += "\nBạn có chắc chắn muốn duyệt?";

                    return Json(new { success = false, isWarning = true, message = warningMessage });
                }

                // 4. Nếu không có xung đột nào -> Tự động duyệt
                partyToApprove.Status = "Upcoming";
                db.Entry(partyToApprove).State = EntityState.Modified;
                await db.SaveChangesAsync();

                return Json(new { success = true, newStatus = "Upcoming", message = "Đã duyệt thành công (không có xung đột)." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, isWarning = false, message = ex.Message });
            }
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}