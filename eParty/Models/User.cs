using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eParty.Models
{
    public class User
    {
        [Key, StringLength(50)]
        public string Username { get; set; }

        [Required, StringLength(50)] public string Password { get; set; }
        [StringLength(50)] public string FirstName { get; set; }
        [StringLength(50)] public string LastName { get; set; }
        public string Avatar { get; set; } // nvarchar(MAX)
        [StringLength(50)] public string Email { get; set; }
        [StringLength(50)] public string PhoneNumber { get; set; }
        [StringLength(20)] public string Role { get; set; }

        public virtual ICollection<Party> Parties { get; set; }
        public virtual ICollection<UserDiscount> UserDiscounts { get; set; }
        public virtual ICollection<Rate> Rates { get; set; }
        public virtual ICollection<News> NewsPosts { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}