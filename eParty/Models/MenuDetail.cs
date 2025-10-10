using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace eParty.Models
{
    public class MenuDetail
    {
        [Key, Column(Order = 0)] public int Menu { get; set; }
        [Key, Column(Order = 1)] public int Food { get; set; }
        public int Amount { get; set; }

        [ForeignKey(nameof(Menu))] public virtual Menu MenuRef { get; set; }
        [ForeignKey(nameof(Food))] public virtual Food FoodRef { get; set; }
    }
}