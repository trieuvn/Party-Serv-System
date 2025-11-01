using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace eParty.Areas.Admin.Models
{
    /// <summary>
    /// ViewModel chính cho toàn bộ trang Báo cáo.
    /// </summary>
    public class ReportViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // SỬA ĐỔI: Thay đổi kiểu dữ liệu của bảng
        public List<PartyReportRow> TableData { get; set; }

        public ReportChartData ChartData { get; set; }
        public ReportTotals Totals { get; set; }
        public string ReportType { get; set; }
        public List<SelectListItem> ReportTypes { get; set; }
    }

    /// <summary>
    /// THAY ĐỔI: Đổi tên thành PartyReportRow và thêm các trường chi tiết
    /// Dữ liệu cho một hàng trong bảng (giờ đây là 1 Party)
    /// </summary>
    public class PartyReportRow
    {
        public int PartyId { get; set; }
        public string PartyName { get; set; }
        public DateTime BeginTime { get; set; }
        public string Status { get; set; }
        public int PartyCount { get { return 1; } } // Mỗi hàng là 1 party
        public long Revenue { get; set; } // (Party.Cost)
        public long MenuCost { get; set; } // (Menu.Cost)
        public long FoodCost { get; set; } // (SUM(MD.Amount * Food.Cost))
        public long IngredientCost { get; set; } // (SUM(MD.Amount * Food.GetMinCost()))
        public long PriceHistoryCost { get; set; } // (SUM(PriceHistory.Cost * PH.Amount))
        public long Profit { get; set; } // (Revenue - IngredientCost)
    }

    /// <summary>
    /// Dữ liệu tổng cộng (Giữ nguyên)
    /// </summary>
    public class ReportTotals
    {
        public int PartyCount { get; set; }
        public long Revenue { get; set; }
        public long MenuCost { get; set; }
        public long FoodCost { get; set; }
        public long IngredientCost { get; set; }
        public long PriceHistoryCost { get; set; }
        public long Profit { get; set; }
    }

    /// <summary>
    /// Dữ liệu biểu đồ (Giữ nguyên)
    /// </summary>
    public class ReportChartData
    {
        public List<string> Labels { get; set; }
        public List<long> RevenueData { get; set; }
        public List<long> CostData { get; set; }
        public List<long> ProfitData { get; set; }
        public List<long> PriceHistoryCost { get; set; }
        public List<long> FoodCost { get; set; }
        public List<long> MenuCost { get; set; }
        public List<long> PartyCount { get; set; }
    }
}