using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace eParty.Models
{
    public class PriceHistory
    {
        [Key, Column(Order = 0)] public int Party { get; set; }
        [Key, Column(Order = 1)] public int Food { get; set; }
        public int Cost { get; set; }
        public int Amount { get; set; }

        [ForeignKey(nameof(Party))] public virtual Party PartyRef { get; set; }
        [ForeignKey(nameof(Food))] public virtual Food FoodRef { get; set; }
    }
}