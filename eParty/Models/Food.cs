using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

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

        // New method added

        /// <summary>
        /// Calculates the minimum cost to produce this Food using the lowest prices of ingredients from providers.
        /// For each ingredient in FoodIngredients, it fetches the min price via Ingredient.GetMinCost() and multiplies by Amount.
        /// Returns the total min cost, or 0 if no ingredients or data missing.
        /// </summary>
        /// <returns>The minimum production cost</returns>
        public int GetMinCost()
        {
            if (FoodIngredients == null || !FoodIngredients.Any())
            {
                return 0;
            }

            int totalMinCost = 0;

            foreach (var foodIng in FoodIngredients)
            {
                if (foodIng.IngredientRef == null)
                {
                    continue; // Skip if ingredient not loaded
                }

                int minPricePerUnit = foodIng.IngredientRef.GetMinCost(); // GetMinCost returns int
                int amount = foodIng.Amount; // Amount is int
                totalMinCost += minPricePerUnit * amount;
            }

            return totalMinCost;
        }
    }

}