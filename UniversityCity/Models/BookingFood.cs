using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using UniversityCity.Models;

namespace UnivercityCityFood.Models
{
    public class BookingFood
    {
        [Key]
        public int id { get; set; }
        public bool Received { get; set; }
        public bool Fine { get; set; }
        public bool Booked { get; set; }
        public string BookingDateToDay { get; set; }
        public DateTime BookingTimeToDay { get; set; }
        public int DayID { get; set; }
        public String ApplicationUserID { get; set; }
        public virtual Day day { get; set; }
        public virtual ApplicationUser appUser{ get; set; }

    }

}