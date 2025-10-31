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
    }
}