using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eParty.Models
{
    public class Menu
    {
        [Key] public int Id { get; set; }
        [StringLength(50)] public string Name { get; set; }
        public string Description { get; set; }
        public int Cost { get; set; }
        [StringLength(20)] public string Status { get; set; }
        public string Image { get; set; }               // nvarchar(MAX)
        public int? Discount { get; set; }              // optional %

        public virtual ICollection<MenuDetail> MenuDetails { get; set; }
        public virtual ICollection<Party> Parties { get; set; }

        /// <summary>
        /// Gets the average rating of all parties that use this menu.
        /// Returns 0 if no parties or ratings are available.
        /// </summary>
        /// <returns>Average rating of parties using this menu</returns>
        public double GetAvgRate()
        {
            if (Parties == null || !Parties.Any())
            {
                return 0;
            }

            var allRatings = new List<double>();
            foreach (var party in Parties)
            {
                if (party.Rates != null && party.Rates.Any())
                {
                    var partyAvgRating = party.Rates.Average(r => r.Stars);
                    allRatings.Add(partyAvgRating);
                }
            }

            if (!allRatings.Any())
            {
                return 0;
            }

            return allRatings.Average();
        }
    }
}