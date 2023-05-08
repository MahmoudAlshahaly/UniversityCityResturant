using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using UniversityCity.Models;

namespace UnivercityCityFood.Models
{
    public class Day
    {
      
        public int DayID { get; set; }
        public string DayName { get; set; }
        public string DayDescription { get; set; }

        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<PayFine> PayFine { get; set; }
    }
}