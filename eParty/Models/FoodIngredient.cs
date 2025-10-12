using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace eParty.Models
{
    public class FoodIngredient
    {
        [Key, Column(Order = 0)] public int Food { get; set; }
        [Key, Column(Order = 1)] public int Ingredient { get; set; }
        public int Amount { get; set; }

        [ForeignKey(nameof(Food))] public virtual Food FoodRef { get; set; }
        [ForeignKey(nameof(Ingredient))] public virtual Ingredient IngredientRef { get; set; }
    }
}