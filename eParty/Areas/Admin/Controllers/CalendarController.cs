using eParty.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Newtonsoft.Json;

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
                                .Where(p => p.BeginTime.HasValue && p.EndTime.HasValue)
                                .ToList();

                var eventsData = parties.Select(p => new
                {
                    id = p.Id.ToString(),
                    title = p.Name,

                    // *** SỬA LỖI MÚI GIỜ (Bản vá quan trọng nhất) ***
                    // Chỉ gửi chuỗi ngày giờ (theo giờ local của server), KHÔNG gửi offset (K) hoặc (Z)
                    // JS sẽ tự động parse đây là "giờ địa phương" của trình duyệt
                    startTime = p.BeginTime.Value.ToString("yyyy-MM-ddTHH:mm:ss"),
                    endTime = p.EndTime.Value.ToString("yyyy-MM-ddTHH:mm:ss"),

                    description = p.Description ?? "",
                    color = GetColorFromStatus(p.Status),
                    reminder = false
                }).ToList();

                // Dùng tên biến serverEvents để tránh trùng lặp
                ViewBag.ServerEventsJson = new HtmlString(JsonConvert.SerializeObject(eventsData));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Lỗi khi lấy dữ liệu sự kiện cho Calendar: " + ex.Message);
                ViewBag.ServerEventsJson = new HtmlString("[]");
                ViewBag.CalendarError = "Đã xảy ra lỗi khi tải dữ liệu sự kiện.";
            }

            return View();
        }

        private string GetColorFromStatus(string status)
        {
            switch (status?.ToLower())
            {
                case "upcoming": return "#4e73df";
                case "ongoing": return "#1cc88a";
                case "completed": return "#858796";
                case "cancelled": return "#e74a3b";
                default: return "#5a5c69";
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