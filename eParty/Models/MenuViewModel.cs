using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eParty.Models // Hoặc eParty.ViewModels
{
    // Lớp này sẽ chứa tất cả dữ liệu cần thiết cho trang Menu
    public class MenuViewModel
    {
        // Danh sách các danh mục, mỗi danh mục đã bao gồm các món ăn của nó
        public List<Category> CategoriesWithFoods { get; set; }
    }
}