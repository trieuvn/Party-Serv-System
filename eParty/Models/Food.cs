using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

        public int CategoryId { get; set; }

        // Navigation property to the parent Category
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        public virtual ICollection<MenuDetail> MenuDetails { get; set; }
        public virtual ICollection<FoodIngredient> FoodIngredients { get; set; }
        public virtual ICollection<PriceHistory> PriceHistories { get; set; }
    }
}