using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace eParty.Models
{
    public class UserDiscount
    {
        [Key, Column(Order = 0), StringLength(50)]
        public string User { get; set; }
        [Key, Column(Order = 1)]
        public int Discount { get; set; }
        public int Amount { get; set; }

        [ForeignKey(nameof(User))] public virtual User UserRef { get; set; }
        [ForeignKey(nameof(Discount))] public virtual Discount DiscountRef { get; set; }
    }
}