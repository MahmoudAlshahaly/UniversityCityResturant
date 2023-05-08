using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace UniversityCity.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "الكود")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "تزكرنى ؟")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "البريد الالكترونى")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        //[Required]
        //[Display(Name = "البريد الالكترونى")]
        //[EmailAddress]
        //public string Email { get; set; }

        [Required]
        [DisplayName("اسم المستخدم")]
        public string UserName { get; set; }
        //[Required]
        //[DisplayName("الرقم القومى")]
        //public string UserSDN { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "كلمة السر")]
        public string Password { get; set; }

        [Display(Name = "تزكرنى ؟")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "اسم المستخدم مطلوب")]
        [DisplayName("اسم المستخدم")]
        public string UserName { get; set; }

        //[Required(AllowEmptyStrings = false, ErrorMessage = "نوع الحساب مطلوب")]
        [DisplayName("نوع الحساب")]
        public string UserType { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "الرقم القومى مطلوب")]
        [DisplayName("الرقم القومى")]
        public string UserID { get; set; }

        //[Required(AllowEmptyStrings = false, ErrorMessage = "اسم الكلية مطلوب")]
        [DisplayName("اسم الكلية")]
        public string UserFaculty { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "تاريخ الميلاد مطوب")]
        [DisplayName("تاريخ الميلاد")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public System.DateTime UserBirthDay { get; set; }

        [EmailAddress]
        [Required(AllowEmptyStrings = false, ErrorMessage = "ألبريد الالكترونى مطلوب")]
        [Display(Name = "البريد الالكترونى")]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "كلمة السر مطلوبة")]
        [DataType(DataType.Password)]
        [Display(Name = "كلمة السر")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "تأكيد كلمة السر")]
        [Compare("Password", ErrorMessage = "كلمة المرور وتأكيد كلمة المرور خطأ")]
        public string ConfirmPassword { get; set; }

        //[Required(AllowEmptyStrings = false, ErrorMessage = "الصورة الشخصية مطلوبة")]
        [DisplayName("الصورة الشخصية")]
        public string UserImage { get; set; }
    }

    public class EditProfileViewModel
    {
        public int id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "اسم المستخدم مطلوب")]
        [DisplayName("اسم المستخدم")]
        public string UserName { get; set; }

        [EmailAddress]
        [Required(AllowEmptyStrings = false, ErrorMessage = "ألبريد الالكترونى مطلوب")]
        [Display(Name = "البريد الالكترونى")]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "كلمة السر مطلوبة")]
        [DataType(DataType.Password)]
        [Display(Name = "كلمة السر الحالية")]
        public string CurrentPassword { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "كلمة السر مطلوبة")]
        [DataType(DataType.Password)]
        [Display(Name = "كلمة السر الجديدة")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "تأكيد كلمة السر")]
        [Compare("NewPassword", ErrorMessage = "كلمة المرور وتأكيد كلمة المرور خطأ")]
        public string ConfirmPassword { get; set; }

        [DisplayName("الصورة الشخصية")]
        public string UserImage { get; set; }
    }
    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "البريد الالكترونى")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "كلمة السر")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "تأكيد كلمة السر")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "البريد الالكترونى")]
        public string Email { get; set; }
    }
}
