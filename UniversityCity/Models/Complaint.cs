using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using UniversityCity.Models;

namespace UnivercityCityFood.Models
{
    public class Complaint
    {
        [Key]
        public int Id { get; set; }
        public string CompContent { get; set; }
        public DateTime CompDate { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

    }
}