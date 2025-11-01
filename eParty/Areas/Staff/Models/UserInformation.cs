using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eParty.Models;
using System.Runtime;

namespace eParty.Areas.Staff.Models
{
    public class UserInformation
    {
        public ApplicationUser user { get; set; }
        public SystemUser systemUser { get; set; }
    }
}