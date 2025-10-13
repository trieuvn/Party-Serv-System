using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eParty.Models;
using System.Web.Mvc;

namespace eParty.Areas.Admin.Models
{
    public class FoodRecipeViewModel
    {
        // Thông tin của món ăn
        public Food Food { get; set; }

        // Danh sách các nguyên liệu đã có trong món ăn
        public List<FoodIngredient> Ingredients { get; set; }

        // Dùng cho form "Thêm mới nguyên liệu"
        public int NewIngredientId { get; set; }
        public string NewIngredientAmount { get; set; }

        // Dùng để đổ danh sách tất cả nguyên liệu ra dropdown
        public IEnumerable<SelectListItem> AllIngredients { get; set; }
    }
}