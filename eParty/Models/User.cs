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
        [StringLength(50)] public string Email { get; set; }
        [StringLength(50)] public string PhoneNumber { get; set; }
        [StringLength(20)] public string Role { get; set; }

        public virtual ICollection<Party> Parties { get; set; }
    }

    // TPH kế thừa từ User
    public class Staff : User
    {
        public int Salary { get; set; }

        public virtual ICollection<StaffParty> StaffParties { get; set; }
    }
}