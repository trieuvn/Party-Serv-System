using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace eParty.Models
{
    public class Ingredient
    {
        [Key] public int Id { get; set; }
        [StringLength(50)] public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Provider> Providers { get; set; }
        public virtual ICollection<FoodIngredient> FoodIngredients { get; set; }

        // New method added

        /// <summary>
        /// Gets the minimum cost from providers supplying this ingredient.
        /// Returns 0 if no providers are available.
        /// </summary>
        /// <returns>The lowest cost, or 0 if no providers</returns>
        public int GetMinCost()
        {
            if (Providers == null || !Providers.Any())
            {
                return 0;
            }

            // Get all providers that supply this ingredient and find minimum cost
            var costs = Providers.Select(p => p.Cost).Where(cost => cost > 0);
            if (!costs.Any())
            {
                return 0;
            }

            return costs.Min();
        }
    }

}