using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace eParty.Models
{
    public class StaffParty
    {
        [Key, Column(Order = 0), StringLength(50)]
        public string Staff { get; set; }
        [Key, Column(Order = 1)]
        public int Party { get; set; }

        [ForeignKey(nameof(Staff))] public virtual SystemUser StaffRef { get; set; }
        [ForeignKey(nameof(Party))] public virtual Party PartyRef { get; set; }
    }
}