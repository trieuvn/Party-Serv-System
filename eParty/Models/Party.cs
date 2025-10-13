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
    }
}