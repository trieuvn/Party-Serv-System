using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eParty.Models
{
    public class HomeViewModel
    {
        public List<Provider> Providers { get; set; }
        public List<Food> Foods { get; set; }
        public List<Ingredient> Ingredients { get; set; }
        public List<SystemUser> RoleUsers { get; set; }
        public int TotalCost { get; set; }

    }
}