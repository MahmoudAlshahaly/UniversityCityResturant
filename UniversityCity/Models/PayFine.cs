using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UnivercityCityFood.Models
{
    public class PayFine
    {
        [Key]
        public int PayFineID { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
        public DateTime PayFineDate { get; set; }
        public bool PayFineDone { get; set; }
        public int DayID { get; set; }
        public virtual Day Day { get; set; }
    }
}