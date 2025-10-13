using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eParty.Models
{
    public class Ingredient
    {
        [Key] public int Id { get; set; }
        [StringLength(50)] public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Provider> Providers { get; set; }
        public virtual ICollection<FoodIngredient> FoodIngredients { get; set; }
    }
}