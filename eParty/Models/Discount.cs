using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eParty.Models
{
    public class Discount
    {
        [Key] public int Id { get; set; }
        public int Value { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ExpiredDate { get; set; }
        [StringLength(10)] public string CouponCode { get; set; }
        public bool IsValid { get; set; }

        public virtual ICollection<UserDiscount> UserDiscounts { get; set; }

        /// <summary>
        /// Gets the total usage amount by summing all Amount values from related UserDiscounts.
        /// Returns 0 if no UserDiscounts are loaded or available.
        /// </summary>
        /// <returns>Total amount of discount usage</returns>
        public int GetUsageCount()
        {
            if (UserDiscounts == null || !UserDiscounts.Any())
            {
                return 0;
            }

            // Sum all Amount values from UserDiscounts
            return UserDiscounts.Sum(ud => ud.Amount);
        }
    }

}