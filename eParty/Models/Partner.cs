using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eParty.Models
{
    public class Partner
    {
        [Key] public int Id { get; set; }
        [StringLength(50)] public string Name { get; set; }
        [StringLength(50)] public string Subject { get; set; }
        public string Description { get; set; }
    }
}