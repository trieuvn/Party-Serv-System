using eParty.Areas.Admin.Models;
using eParty.Models;
using OfficeOpenXml;
// using OfficeOpenXml.License; // Đã xóa cho EPPlus v4
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace eParty.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReportsController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: Admin/Reports
        public async Task<ActionResult> Index(DateTime? startDate, DateTime? endDate, string reportType = "Summary")
        {
            var end = endDate ?? DateTime.Now;
            var start = startDate ?? end.AddMonths(-3);

            // SỬA ĐỔI: Lấy dữ liệu chi tiết (theo từng Party)
            var reportData = await GetPartyReportData(start, end);
            var reportTypes = GetReportTypes(reportType);

            var viewModel = new ReportViewModel
            {
                StartDate = start,
                EndDate = end,
                ReportType = reportType,
                ReportTypes = reportTypes,
                TableData = reportData,
                // SỬA ĐỔI: Biểu đồ giờ sẽ nhóm theo NGÀY
                ChartData = FormatDataForChart_ByDay(reportData, reportType),
                Totals = CalculateTotals(reportData)
            };

            return View(viewModel);
        }

        // GET: Admin/Reports/ExportToExcel
        public async Task<ActionResult> ExportToExcel(DateTime startDate, DateTime endDate, string reportType = "Summary")
        {
            // SỬA ĐỔI: Lấy dữ liệu chi tiết
            var data = await GetPartyReportData(startDate, endDate);
            var totals = CalculateTotals(data);

            // Đã xóa LicenseContext cho EPPlus v4

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("BaoCao");

                worksheet.Cells["A1"].Value = $"BÁO CÁO CHI TIẾT: {reportType}";
                worksheet.Cells["A2"].Value = $"Từ ngày: {startDate:dd/MM/yyyy} Đến ngày: {endDate:dd/MM/yyyy}";

                // SỬA ĐỔI: Thay đổi tiêu đề bảng
                int col = 1;
                worksheet.Cells[4, col++].Value = "ID Party";
                worksheet.Cells[4, col++].Value = "Tên Party";
                worksheet.Cells[4, col++].Value = "Ngày";

                if (reportType == "Summary" || reportType == "Revenue" || reportType == "PartyCount")
                {
                    worksheet.Cells[4, col++].Value = "Số lượng";
                }
                if (reportType == "Summary" || reportType == "Revenue" || reportType == "Costs")
                {
                    worksheet.Cells[4, col++].Value = "Doanh thu";
                }
                if (reportType == "Summary" || reportType == "Costs")
                {
                    worksheet.Cells[4, col++].Value = "Chi phí NVL (COGS)";
                    worksheet.Cells[4, col++].Value = "Lợi nhuận";
                }
                if (reportType == "Costs")
                {
                    worksheet.Cells[4, col++].Value = "Chi phí PriceHistory";
                    worksheet.Cells[4, col++].Value = "Giá Menu (Lý thuyết)";
                    worksheet.Cells[4, col++].Value = "Giá Food (Lý thuyết)";
                }

                // SỬA ĐỔI: Đổ dữ liệu theo từng Party
                int row = 5;
                foreach (var item in data)
                {
                    col = 1;
                    worksheet.Cells[row, col++].Value = item.PartyId;
                    worksheet.Cells[row, col++].Value = item.PartyName;
                    worksheet.Cells[row, col++].Value = item.BeginTime;

                    if (reportType == "Summary" || reportType == "Revenue" || reportType == "PartyCount")
                    {
                        worksheet.Cells[row, col++].Value = 1; // 1 party
                    }
                    if (reportType == "Summary" || reportType == "Revenue" || reportType == "Costs")
                    {
                        worksheet.Cells[row, col++].Value = item.Revenue;
                    }
                    if (reportType == "Summary" || reportType == "Costs")
                    {
                        worksheet.Cells[row, col++].Value = item.IngredientCost;
                        worksheet.Cells[row, col++].Value = item.Profit;
                    }
                    if (reportType == "Costs")
                    {
                        worksheet.Cells[row, col++].Value = item.PriceHistoryCost;
                        worksheet.Cells[row, col++].Value = item.MenuCost;
                        worksheet.Cells[row, col++].Value = item.FoodCost;
                    }
                    row++;
                }

                // SỬA ĐỔI: Dòng tổng cộng
                col = 1;
                worksheet.Cells[row, col++].Value = "TỔNG CỘNG";
                col++; // Bỏ qua Tên
                col++; // Bỏ qua Ngày

                if (reportType == "Summary" || reportType == "Revenue" || reportType == "PartyCount")
                {
                    worksheet.Cells[row, col++].Value = totals.PartyCount;
                }
                if (reportType == "Summary" || reportType == "Revenue" || reportType == "Costs")
                {
                    worksheet.Cells[row, col++].Value = totals.Revenue;
                }
                if (reportType == "Summary" || reportType == "Costs")
                {
                    worksheet.Cells[row, col++].Value = totals.IngredientCost;
                    worksheet.Cells[row, col++].Value = totals.Profit;
                }
                if (reportType == "Costs")
                {
                    worksheet.Cells[row, col++].Value = totals.PriceHistoryCost;
                    worksheet.Cells[row, col++].Value = totals.MenuCost;
                    worksheet.Cells[row, col++].Value = totals.FoodCost;
                }

                // Format
                worksheet.Cells[5, 3, row, col].Style.Numberformat.Format = "#,##0";
                worksheet.Cells[5, 3, row - 1, 3].Style.Numberformat.Format = "dd/mm/yyyy hh:mm";
                worksheet.Cells.AutoFitColumns();

                var fileBytes = package.GetAsByteArray();
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"BaoCao_ChiTiet_{reportType}_{DateTime.Now:yyyy-MM-dd}.xlsx");
            }
        }

        // --- CÁC HÀM HELPER (ĐÃ CẬP NHẬT) ---

        private List<SelectListItem> GetReportTypes(string selectedType)
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value = "Summary", Text = "Báo cáo Tổng hợp (Lợi nhuận)", Selected = selectedType == "Summary" },
                new SelectListItem { Value = "Revenue", Text = "Báo cáo Doanh thu (Party.Cost)", Selected = selectedType == "Revenue" },
                new SelectListItem { Value = "Costs", Text = "Báo cáo Chi phí (Chi tiết)", Selected = selectedType == "Costs" },
                new SelectListItem { Value = "PartyCount", Text = "Báo cáo Số lượng Tiệc", Selected = selectedType == "PartyCount" }
            };
        }

        /// <summary>
        /// SỬA ĐỔI: Hàm lõi giờ đây trả về danh sách chi tiết theo từng Party
        /// </summary>
        private async Task<List<PartyReportRow>> GetPartyReportData(DateTime start, DateTime end)
        {
            DateTime endOfDay = end.Date.AddDays(1).AddTicks(-1);

            var parties = await db.Parties
                .Include(p => p.PriceHistories)
                .Include(p => p.MenuRef.MenuDetails.Select(md => md.FoodRef.FoodIngredients.Select(fi => fi.IngredientRef.Providers)))
                .Where(p => p.BeginTime.HasValue &&
                            p.BeginTime.Value >= start &&
                            p.BeginTime.Value <= endOfDay &&
                            (p.Status == "Upcoming" || p.Status == "Ongoing" || p.Status == "Completed"))
                .OrderBy(p => p.BeginTime)
                .ToListAsync();

            var reportRows = new List<PartyReportRow>();

            // SỬA ĐỔI: Không GroupBy, lặp qua từng Party
            foreach (var party in parties)
            {
                var row = new PartyReportRow
                {
                    PartyId = party.Id,
                    PartyName = party.Name,
                    BeginTime = party.BeginTime.Value,
                    Status = party.Status,
                    Revenue = (long)party.Cost
                };

                var priceHistories = party.PriceHistories ?? new List<PriceHistory>();
                row.PriceHistoryCost = priceHistories.Sum(ph => (long)ph.Cost * (long)ph.Amount);

                var menuDetails = party.MenuRef?.MenuDetails ?? new List<MenuDetail>();

                row.MenuCost = (long)(party.MenuRef?.Cost ?? 0);
                row.FoodCost = menuDetails.Sum(md => (long)md.Amount * (long)(md.FoodRef?.Cost ?? 0));
                row.IngredientCost = menuDetails.Sum(md => (long)md.Amount * (long)(md.FoodRef?.GetMinCost() ?? 0));

                row.Profit = row.Revenue - row.IngredientCost;

                reportRows.Add(row);
            }

            return reportRows;
        }

        /// <summary>
        /// SỬA ĐỔI: Chuyển đổi dữ liệu cho Chart.js (NHÓM THEO NGÀY)
        /// </summary>
        private ReportChartData FormatDataForChart_ByDay(List<PartyReportRow> data, string reportType)
        {
            // Nhóm dữ liệu theo Ngày
            var groupedByDay = data
                .GroupBy(p => p.BeginTime.Date)
                .OrderBy(g => g.Key);

            var labels = groupedByDay.Select(g => g.Key.ToString("dd/MM")).ToList();
            var chartData = new ReportChartData { Labels = labels };

            switch (reportType)
            {
                case "Revenue":
                    chartData.RevenueData = groupedByDay.Select(g => g.Sum(p => p.Revenue)).ToList();
                    break;
                case "Costs":
                    chartData.CostData = groupedByDay.Select(g => g.Sum(p => p.IngredientCost)).ToList();
                    chartData.PriceHistoryCost = groupedByDay.Select(g => g.Sum(p => p.PriceHistoryCost)).ToList();
                    chartData.FoodCost = groupedByDay.Select(g => g.Sum(p => p.FoodCost)).ToList();
                    chartData.MenuCost = groupedByDay.Select(g => g.Sum(p => p.MenuCost)).ToList();
                    break;
                case "PartyCount":
                    chartData.PartyCount = groupedByDay.Select(g => (long)g.Count()).ToList();
                    break;
                case "Summary":
                default:
                    chartData.RevenueData = groupedByDay.Select(g => g.Sum(p => p.Revenue)).ToList();
                    chartData.CostData = groupedByDay.Select(g => g.Sum(p => p.IngredientCost)).ToList();
                    chartData.ProfitData = groupedByDay.Select(g => g.Sum(p => p.Profit)).ToList();
                    break;
            }
            return chartData;
        }

        /// <summary>
        /// SỬA ĐỔI: Tính tổng từ List<PartyReportRow>
        /// </summary>
        private ReportTotals CalculateTotals(List<PartyReportRow> data)
        {
            if (data == null || !data.Any())
                return new ReportTotals();

            return new ReportTotals
            {
                PartyCount = data.Count, // Sửa: data.Sum(r => r.PartyCount)
                Revenue = data.Sum(r => r.Revenue),
                MenuCost = data.Sum(r => r.MenuCost),
                FoodCost = data.Sum(r => r.FoodCost),
                IngredientCost = data.Sum(r => r.IngredientCost),
                PriceHistoryCost = data.Sum(r => r.PriceHistoryCost),
                Profit = data.Sum(r => r.Profit)
            };
        }
    }
}