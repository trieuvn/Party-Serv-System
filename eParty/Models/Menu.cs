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
    }
}