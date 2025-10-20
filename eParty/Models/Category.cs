using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eParty.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [StringLength(30)]
        public string Name { get; set; }

        // Navigation property for the one-to-many relationship
        // One Category can have many Foods
        public virtual ICollection<Food> Foods { get; set; }
    }
}