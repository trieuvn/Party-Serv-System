using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace eParty.Models
{
    public class Provider
    {
        [Key] public int Id { get; set; }

        // FK -> Ingredient
        public int Ingredient { get; set; }
        [ForeignKey(nameof(Ingredient))] public virtual Ingredient IngredientRef { get; set; }

        [StringLength(50)] public string Name { get; set; }
        public string Description { get; set; }
        [StringLength(15)] public string PhoneNumber { get; set; }
        [StringLength(100)] public string Address { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int Cost { get; set; }
    }
}