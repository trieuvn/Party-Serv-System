using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eParty.Models
{
    public class FoodDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl {  get; set; }
        public int Unit { get; set; }
    }
}