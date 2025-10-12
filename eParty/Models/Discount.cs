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
        public int Value { get; set; }                  // %
        public DateTime? CreatedDate { get; set; }
        public DateTime? ExpiredDate { get; set; }
        [StringLength(10)] public string CouponCode { get; set; }
        public bool IsValid { get; set; }

        public virtual ICollection<UserDiscount> UserDiscounts { get; set; }
    }
}