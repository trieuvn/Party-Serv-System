using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace eParty.Models
{
    public class News
    {
        [Key] public int Id { get; set; }
        [StringLength(50)] public string Name { get; set; }
        [StringLength(100)] public string Subject { get; set; }
        public string Description { get; set; }
        [StringLength(20)] public string Status { get; set; }

        // FK -> User (author)
        [StringLength(50)]
        public string User { get; set; }
        [ForeignKey(nameof(User))] public virtual User Author { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
    }
}