using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eParty.Models
{
    public class Food
    {
        [Key] public int Id { get; set; }
        [StringLength(50)] public string Name { get; set; }
        public string Description { get; set; }
        public int Unit { get; set; }
        public int Cost { get; set; }
        public string Image { get; set; }               // nvarchar(MAX)
        public int? Discount { get; set; }

        public virtual ICollection<MenuDetail> MenuDetails { get; set; }
        public virtual ICollection<FoodIngredient> FoodIngredients { get; set; }
        public virtual ICollection<PriceHistory> PriceHistories { get; set; }
    }
}