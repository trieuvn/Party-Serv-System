using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace eParty.Models
{
    public class Party
    {
        [Key] public int Id { get; set; }

        [StringLength(50)] public string Name { get; set; }
        public string Image { get; set; }               // nvarchar(MAX)
        [StringLength(20)] public string Type { get; set; }
        [StringLength(20)] public string Status { get; set; }
        public int Cost { get; set; }
        public DateTime? BeginTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string Description { get; set; }
        public int Slots { get; set; }
        [StringLength(100)] public string Address { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        // FK -> User
        [StringLength(50)]
        public string User { get; set; }
        [ForeignKey(nameof(User))]
        public virtual User Owner { get; set; }

        // FK -> Menu
        public int? Menu { get; set; }
        [ForeignKey(nameof(Menu))]
        public virtual Menu MenuRef { get; set; }

        public virtual ICollection<StaffParty> StaffParties { get; set; }
        public virtual ICollection<PriceHistory> PriceHistories { get; set; }
        public virtual ICollection<Rate> Rates { get; set; }

        //METHOD
        public string GetImagePath()
        {
            if (string.IsNullOrEmpty(Image))
            {
                return "/images/party/party-default.png"; // Default image path
            }
            return Image;
        }

        public double GetAverageRating()
        {
            if (Rates == null || Rates.Count == 0)
            {
                return 0; // No ratings available
            }
            return Rates.Average(r => r.Stars);
        }

        public int GetDiscountedPrice(int originalPrice, int? discountPercentage)
        {
            if (discountPercentage.HasValue && discountPercentage.Value > 0 && discountPercentage.Value <= 100)
            {
                return originalPrice - (originalPrice * discountPercentage.Value / 100);
            }
            return originalPrice; // No discount applied
        }

        public string GetStatusBadge()
        {
            switch (Status?.ToLower())
            {
                case "upcoming":
                    return "<span class='badge badge-info'>Upcoming</span>";
                case "ongoing":
                    return "<span class='badge badge-success'>Ongoing</span>";
                case "completed":
                    return "<span class='badge badge-secondary'>Completed</span>";
                case "cancelled":
                    return "<span class='badge badge-danger'>Cancelled</span>";
                default:
                    return "<span class='badge badge-light'>Unknown</span>";
            }
        }

        public bool IsUserOwner(string username)
        {
            return string.Equals(User, username, StringComparison.OrdinalIgnoreCase);
        }

        public bool IsPartyActive()
        {
            return string.Equals(Status, "ongoing", StringComparison.OrdinalIgnoreCase);
        }

        public bool HasUserRated(string username)
        {
            return Rates != null && Rates.Any(r => r.User == username);
        }

        public string GetFormattedDateRange()
        {
            if (BeginTime.HasValue && EndTime.HasValue)
            {
                return $"{BeginTime.Value:MMM dd, yyyy} - {EndTime.Value:MMM dd, yyyy}";
            }
            return "Date not specified";
        }

        
    }
}