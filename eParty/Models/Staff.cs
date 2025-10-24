using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eParty.Models
{
    public class Staff : SystemUser
    {
        public int Salary { get; set; }

        public virtual ICollection<StaffParty> StaffParties { get; set; }
    }
}