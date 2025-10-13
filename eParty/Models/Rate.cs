using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace eParty.Models
{
    public class Rate
    {
        [Key, Column(Order = 0), StringLength(50)]
        public string User { get; set; }
        [Key, Column(Order = 1)]
        public int Party { get; set; }
        public int Stars { get; set; }
        [StringLength(50)] public string Comment { get; set; }

        [ForeignKey(nameof(User))] public virtual User UserRef { get; set; }
        [ForeignKey(nameof(Party))] public virtual Party PartyRef { get; set; }
    }
}