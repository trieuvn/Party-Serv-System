using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eParty.Models
{
    public class MenuDetailDto
    {
        public int MenuId { get; set; }
        public int FoodId { get; set; }
        public string FoodName { get; set; }
        public int Amount { get; set; }
    }
}