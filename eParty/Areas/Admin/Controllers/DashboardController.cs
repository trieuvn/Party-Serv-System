using eParty.Areas.Admin.Models;
using eParty.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity; // <-- ĐẢM BẢO CÓ DÒNG NÀY
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eParty.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // ===== THAY THẾ TOÀN BỘ PHƯƠNG THỨC Index BẰNG ĐOẠN NÀY =====
        public ActionResult Index()
        {
            // --- Phần thẻ thống kê (Giữ nguyên logic cũ của bạn) ---
            long totalCostValue = 0;
            try
            {
                // Vẫn tính tổng Party.Cost cho thẻ "Total Cost"
                totalCostValue = db.Parties.ToList().Sum(f => (long)f.Cost);
            }
            catch (Exception) { /* Bỏ qua nếu không có party */ }

            String total_cost_string = totalCostValue.ToString("n0", CultureInfo.GetCultureInfo("vi-VN"));

            // === TÍNH TOÁN DAILY SALES (MỚI) ===
            long totalSalesHistory = 0;
            try
            {
                // 1. Tính tổng chi phí từ PriceHistory (Cost * Amount)
                // Dùng ToList() để tránh lỗi LINQ to Entities không hỗ trợ phép nhân long
                totalSalesHistory = db.PriceHistories.ToList()
                                      .Sum(ph => (long)ph.Cost * (long)ph.Amount);
            }
            catch (Exception)
            {
                // Bỏ qua nếu không có PriceHistory
                totalSalesHistory = 0;
            }

            // 2. Tính số ngày trôi qua từ 01/01/2025
            DateTime startDate = new DateTime(2025, 1, 1);
            // Dùng .Date để đảm bảo so sánh 2 ngày, bỏ qua giờ
            double daysElapsed = (DateTime.Now.Date - startDate.Date).TotalDays;

            // 3. Tính toán giá trị Daily Sales
            long dailySalesValue = 0;
            // Chỉ tính khi số ngày > 0 (tránh chia cho 0)
            if (daysElapsed > 0 && totalSalesHistory > 0)
            {
                // Thực hiện phép chia (double) rồi ép kiểu về long
                dailySalesValue = (long)(totalSalesHistory / daysElapsed);
            }

            // 4. Định dạng chuỗi
            string daily_sales_string = dailySalesValue.ToString("n0", CultureInfo.GetCultureInfo("vi-VN"));
            // === KẾT THÚC TÍNH TOÁN DAILY SALES ===


            // === LẤY TRANSACTION HISTORY (MỚI) ===
            // Lấy 7 Party mới nhất (theo BeginTime) CÓ PriceHistory
            // Dùng Include() để load thông tin Owner và MenuRef, tránh lỗi N+1 query trong View
            var transactionHistory = db.Parties
                .Include(p => p.Owner)       //
                .Include(p => p.MenuRef)     //
                .Where(p => p.PriceHistories.Any()) // Chỉ lấy Party có PriceHistory
                .OrderByDescending(p => p.BeginTime) // Sắp xếp mới nhất
                .Take(7)                     // Giới hạn 7 dòng
                .ToList();
            // === KẾT THÚC LẤY TRANSACTION HISTORY ===


            var list = new DashboardViewModel
            {
                Providers = db.Providers.ToList(),
                Foods = db.Foods.ToList(),
                Ingredients = db.Ingredients.ToList(),
                TotalCost = total_cost_string,
                DailySales = daily_sales_string,
                RoleUsers = db.SystemUsers.ToList(),
                NewCustomer = db.SystemUsers.Take(6).ToList(),
                TransactionHistory = transactionHistory // <-- THÊM DÒNG NÀY
            };

            // ===== LOGIC MỚI CHO 3 LINE CHART =====

            int currentYear = DateTime.Now.Year;

            // 1. Khởi tạo 3 mảng 12 tháng với giá trị 0
            var partyCosts = new long[12];
            var priceHistoryCosts = new long[12];
            var menuCosts = new long[12];
            for (int i = 0; i < 12; i++)
            {
                partyCosts[i] = 0;
                priceHistoryCosts[i] = 0;
                menuCosts[i] = 0;
            }

            // 2. Lấy tất cả Party (bao gồm Menu) trong năm hiện tại
            var partiesInYear = db.Parties
                .Include(p => p.MenuRef) //
                .Where(p => p.BeginTime.HasValue &&
                            p.BeginTime.Value.Year == currentYear) //
                .ToList(); // Lấy dữ liệu về

            // 3. Tính Line 1 (Party.Cost) và Line 3 (Menu.Cost) từ partiesInYear
            if (partiesInYear.Any())
            {
                var groupedParties = partiesInYear
                    .GroupBy(p => p.BeginTime.Value.Month); //

                foreach (var monthGroup in groupedParties)
                {
                    int monthIndex = monthGroup.Key - 1;

                    // Line 1: Sum của Party.Cost (Chi phí thực tế)
                    partyCosts[monthIndex] = monthGroup.Sum(p => (long)p.Cost); //

                    // Line 3: Sum của Menu.Cost (Chi phí lý thuyết)
                    menuCosts[monthIndex] = monthGroup.Sum(p => (p.MenuRef != null) ? (long)p.MenuRef.Cost : 0); //
                }
            }

            // 4. Tính Line 2 (PriceHistory) - (Query riêng vì cấu trúc khác)
            try
            {
                var priceHistoryCostsByMonth = db.PriceHistories
                    .Include(ph => ph.PartyRef) //
                    .Where(ph => ph.PartyRef.BeginTime.HasValue &&
                                 ph.PartyRef.BeginTime.Value.Year == currentYear) //
                    .ToList() // Lấy dữ liệu về
                    .GroupBy(ph => ph.PartyRef.BeginTime.Value.Month) //
                    .Select(g => new
                    {
                        Month = g.Key,
                        // Tính tổng (Cost * Amount)
                        TotalCost = g.Sum(ph => (long)ph.Cost * (long)ph.Amount) //
                    });

                foreach (var monthData in priceHistoryCostsByMonth)
                {
                    priceHistoryCosts[monthData.Month - 1] = monthData.TotalCost;
                }
            }
            catch (Exception ex)
            {
                // Bỏ qua nếu có lỗi
            }

            // 5. Gán 3 mảng kết quả vào ViewModel
            list.MonthlyPartyCost = partyCosts.ToList();
            list.MonthlyPriceHistoryCost = priceHistoryCosts.ToList();
            list.MonthlyMenuCost = menuCosts.ToList();

            // ===================================

            return View(list);
        }
    }
}