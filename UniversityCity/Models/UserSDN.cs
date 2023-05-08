using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UnivercityCityFood.Models
{
    public class UserSDN
    {
        [Key]
        public int ID { get; set; }
        
        [Display(Name = "الرقم القومى")]
        public string UserID { get; set; }

        [Display(Name = "نوع المستخدم")]
        public string UserGroup { get; set; }


        [Display(Name = "هل تم تفعيله")]
        public bool UserIDConfirm { get; set; }
    }
}