using eParty.Areas.Admin.Models;
using eParty.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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

        public ActionResult Index()
        {
            // ===== BẮT ĐẦU PHẦN CODE BỊ THIẾU (ĐÃ THÊM LẠI) =====

            // --- Phần thẻ thống kê (Total Cost) ---
            long totalCostValue = 0;
            try
            {
                totalCostValue = db.Parties.ToList().Sum(f => (long)f.Cost);
            }
            catch (Exception) { /* Bỏ qua nếu không có party */ }

            String total_cost_string = totalCostValue.ToString("n0", CultureInfo.GetCultureInfo("vi-VN"));

            // === TÍNH TOÁN DAILY SALES ===
            long totalSalesHistory = 0;
            try
            {
                totalSalesHistory = db.PriceHistories.ToList()
                                      .Sum(ph => (long)ph.Cost * (long)ph.Amount);
            }
            catch (Exception)
            {
                totalSalesHistory = 0;
            }

            DateTime startDate = new DateTime(2025, 1, 1);
            double daysElapsed = (DateTime.Now.Date - startDate.Date).TotalDays;
            long dailySalesValue = 0;
            if (daysElapsed > 0 && totalSalesHistory > 0)
            {
                dailySalesValue = (long)(totalSalesHistory / daysElapsed);
            }
            string daily_sales_string = dailySalesValue.ToString("n0", CultureInfo.GetCultureInfo("vi-VN"));

            // ===== KẾT THÚC PHẦN CODE BỊ THIẾU =====


            // === LẤY TRANSACTION HISTORY ===
            var transactionHistory = db.Parties
                .Include(p => p.Owner)
                .Include(p => p.MenuRef)
                .Where(p => p.PriceHistories.Any())
                .OrderByDescending(p => p.BeginTime)
                .Take(7)
                .ToList();

            // === LẤY NEW CUSTOMER ===
            var newCustomer = db.SystemUsers.Take(6).ToList();


            var list = new DashboardViewModel
            {
                Providers = db.Providers.ToList(),
                Foods = db.Foods.ToList(),
                Ingredients = db.Ingredients.ToList(),
                TotalCost = total_cost_string,       // Biến đã tồn tại
                DailySales = daily_sales_string,      // Biến đã tồn tại
                RoleUsers = db.SystemUsers.ToList(),
                NewCustomer = newCustomer,
                TransactionHistory = transactionHistory
            };

            // ===== LOGIC CHO 3 LINE CHART =====

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
                .Include(p => p.MenuRef)
                .Where(p => p.BeginTime.HasValue &&
                            p.BeginTime.Value.Year == currentYear)
                .ToList();

            // 3. Tính Line 1 (Party.Cost) và Line 3 (Menu.Cost) từ partiesInYear
            if (partiesInYear.Any())
            {
                var groupedParties = partiesInYear
                    .GroupBy(p => p.BeginTime.Value.Month);

                foreach (var monthGroup in groupedParties)
                {
                    int monthIndex = monthGroup.Key - 1;

                    partyCosts[monthIndex] = monthGroup.Sum(p => (long)p.Cost);
                    menuCosts[monthIndex] = monthGroup.Sum(p => (p.MenuRef != null) ? (long)p.MenuRef.Cost : 0);
                }
            }

            // 4. Tính Line 2 (PriceHistory) - (Query riêng vì cấu trúc khác)
            try
            {
                var priceHistoryCostsByMonth = db.PriceHistories
                    .Include(ph => ph.PartyRef)
                    .Where(ph => ph.PartyRef.BeginTime.HasValue &&
                                 ph.PartyRef.BeginTime.Value.Year == currentYear)
                    .ToList()
                    .GroupBy(ph => ph.PartyRef.BeginTime.Value.Month)
                    .Select(g => new
                    {
                        Month = g.Key,
                        TotalCost = g.Sum(ph => (long)ph.Cost * (long)ph.Amount)
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

            // 5. DỰ ĐOÁN (Machine Learning - Hồi quy tuyến tính)
            // === THAY ĐỔI TỪ ĐÂY ===
            int monthsToPredict = 3; // Dự đoán 3 tháng tới (thay vì 1)
            List<long> predictedCosts = PredictFutureCosts(partyCosts, monthsToPredict);

            // 6. Gán 3 mảng kết quả VÀ 1 giá trị dự đoán vào ViewModel
            list.MonthlyPartyCost = partyCosts.ToList();
            list.MonthlyPriceHistoryCost = priceHistoryCosts.ToList();
            list.MonthlyMenuCost = menuCosts.ToList();

            list.PredictedCosts = predictedCosts; // Gán danh sách dự đoán
            list.CurrentMonth = DateTime.Now.Month; // Gán tháng hiện tại (ví dụ: 10)

            return View(list);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="monthlyData"></param>
        /// <param name="monthsToPredict"></param>
        /// <returns></returns>
        private List<long> PredictFutureCosts(long[] monthlyData, int monthsToPredict)
        {
            int n = monthlyData.Length;
            if (n < 2) return new List<long>();

            double sumX = 0, sumY = 0, sumXY = 0, sumX2 = 0;

            for (int i = 0; i < n; i++)
            {
                double x = i + 1; 
                double y = monthlyData[i]; 

                sumX += x;
                sumY += y;
                sumXY += x * y;
                sumX2 += x * x;
            }

            double m_numerator = (n * sumXY) - (sumX * sumY);
            double m_denominator = (n * sumX2) - (sumX * sumX);

            if (m_denominator == 0)
            {
                return new List<long>();
            }

            double m = m_numerator / m_denominator;
            double b = (sumY - m * sumX) / n;

            var predictions = new List<long>();
            for (int i = 1; i <= monthsToPredict; i++)
            {
                double predictedCost = m * (n + i) + b;

                if (predictedCost < 0) predictedCost = 0;

                predictions.Add((long)predictedCost);
            }

            return predictions;
        }
    }
}