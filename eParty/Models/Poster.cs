using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eParty.Models
{
    public class Poster
    {
        [Key] public int Id { get; set; }
        [StringLength(50)] public string Name { get; set; }
        public string Image { get; set; }
    }
}