using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eParty.Models;

namespace eParty.Areas.Admin.Models
{
    public class DashboardViewModel
    {
        public List<Menu> Menus { get; set; }
        public List<MenuDetailDto> menuDetails { get; set; }
        public List<Provider> Providers { get; set; }
        public List<Food> Foods { get; set; }

        public List<Ingredient> Ingredients { get; set; }
        public List<SystemUser> RoleUsers { get; set; }
        public String TotalCost { get; set; }

        public string DailySales { get; set; }

        public List<long> MonthlyPartyCost { get; set; }

        public List<long> MonthlyPriceHistoryCost { get; set; }

        public List<long> MonthlyMenuCost { get; set; }

        public List<SystemUser> NewCustomer { get; set; }

        public List<Party> TransactionHistory { get; set; }

        /// <summary>
        /// Lưu trữ chi phí dự đoán cho các tháng tiếp theo
        /// (Sử dụng ML Hồi quy tuyến tính)
        /// </summary>
        public List<long> PredictedCosts { get; set; } // Đổi từ long sang List<long>

        /// <summary>
        /// Tháng hiện tại (1-based, ví dụ: 10 cho tháng 10)
        /// </summary>
        public int CurrentMonth { get; set; } // Thêm mới
    }
}