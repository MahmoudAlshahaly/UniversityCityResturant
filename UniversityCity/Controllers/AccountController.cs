using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using UniversityCity.Models;
using System.IO;
using System.Data.Entity;
using UnivercityCityFood.Models;
using System.Collections.Generic;

namespace UniversityCity.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext db;
        public AccountController()
        {
            db = new ApplicationDbContext();
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

       
        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]

        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            string ShowMessage = "";
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true

            var userid = UserManager.FindByName(model.UserName).Id;

            ApplicationDbContext db = new ApplicationDbContext();
            var userData = db.Users.Where(a => a.Id == userid).SingleOrDefault();
            Session["backGround"] = userData.UserType;

            if(userid==null)
            {
                return HttpNotFound();
            }
            if (!UserManager.IsEmailConfirmed(userid))
            {
                ShowMessage = "هذا الحساب غير مفعل توجة للبريد الالكترونى الخاص بك وقم بتفعيل الحساب ثم عاود المحاولة  !";
                ViewBag.Message = ShowMessage;
                return View(model);
            }

            var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);

            //var user = db.Users.Where(u => u.Email.Equals(model.Email)).Single(); // where db is ApplicationDbContext instance
            //var result = await SignInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                        return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }

        }


        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {

            if (Session["MyUID"] == null)
            {
                return RedirectToAction("Index", "ConfirmSDN");
            }
            else

            {
                ViewBag.UserFaculty = new SelectList(db.Faculties, "Name", "Name");
                return View();
            }

        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model, HttpPostedFileBase UploadUserImage)
        {
            model.UserType = (string)Session["UserGroup"];

            ViewBag.UserFaculty = new SelectList(db.Faculties, "Name", "Name");

            bool ShowStatus = false;
            string ShowMessage = "";
            if (ModelState.IsValid)
            {
                
                #region SDN Is exist in UserSDN
                var isExistStudentSDN = IsSDNExistInUsertSDNs(model.UserID, model.UserType);

                if (isExistStudentSDN)
                {
                    #region SDN Is Already Exist in Users 
                    var isExistStudentRegister = IsSDNExistInUsers(model.UserID, model.UserType);
                    if (isExistStudentRegister)
                    {
                        ModelState.AddModelError("SDNExist", "هذا الرقم القومى مؤكد به حساب اخر");
                        return View(model);
                    }
                    #endregion

                }
                else
                {
                    //SDN Is not exist in UserSdn
                    ModelState.AddModelError("SDNExist", "الرقم القومى هذا غير موجود او قد تكون ادخلت بيانات خاطئة");
                    return View(model);
                }
                #endregion
                
                if (UploadUserImage == null)
                {

                    model.UserImage = "photo.jpg";
                }
                else
                {
                    string path = Path.Combine(Server.MapPath("~/Uploads"), UploadUserImage.FileName);
                    UploadUserImage.SaveAs(path);

                    model.UserImage = UploadUserImage.FileName;
                }
                var user = new ApplicationUser { UserName = model.UserName, UserType = model.UserType, UserID = model.UserID, UserFaculty = model.UserFaculty, UserBirthDay = model.UserBirthDay, Email = model.Email, UserImage = model.UserImage };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    await UserManager.SendEmailAsync(user.Id, "تأكيد الحساب الخاص بك", "Please confirm your account by clicking this link<a href=\"" + callbackUrl + "\">here</a>");
                    ShowMessage = " تم انشاء الحساب الخاص بك ,لو سمحت توجه الى بريدك الالكترونى لتأكيد حسابك." + model.Email;
                    ShowStatus = true;

                    //add role to account
                    await UserManager.AddToRoleAsync(user.Id, model.UserType);
                }
                AddErrors(result);

                //confirm sdn true
                var userConfirm = db.UserSDNs.Where(c => c.UserID == model.UserID).FirstOrDefault();
                if (userConfirm != null)
                {
                    userConfirm.UserIDConfirm = true;
                    db.SaveChanges();
                }
            }
            else
            {
                ShowMessage = "تسجيل خطأ !";
            }

            ViewBag.Message = ShowMessage;
            ViewBag.Status = ShowStatus;
            return View(model);
        }

        [NonAction]
        public bool IsSDNExistInUsertSDNs(string sdn, string userType)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var v = db.UserSDNs.Any(a => a.UserID == sdn && a.UserGroup == userType && a.UserIDConfirm == false);
                return v;
            }
        }

        [NonAction]
        public bool IsSDNExistInUsers(string sdn, string userType)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var v = db.Users.Where(a => a.UserID == sdn && a.UserType == userType).FirstOrDefault();
                return v != null;
            }
        }
        [HttpGet]
        public ActionResult EditProfile()
        {
            var userID = User.Identity.GetUserId();
            var user = db.Users.Where(a => a.Id == userID).SingleOrDefault();

            EditProfileViewModel profile = new EditProfileViewModel();
            profile.UserName = user.UserName;
            profile.Email = user.Email;
            profile.UserImage = user.UserImage;

            return View(profile);
        }
        [HttpPost]
        public ActionResult EditProfile(EditProfileViewModel profile,HttpPostedFileBase profilePhoto)
        {
            var user = User.Identity.GetUserId();
            var CurrentUser = db.Users.Where(a => a.Id == user).SingleOrDefault();

            if (profilePhoto == null)
            {

                profile.UserImage = "photo.jpg";
            }
            else
            {
                string path = Path.Combine(Server.MapPath("~/Uploads"), profilePhoto.FileName);
                profilePhoto.SaveAs(path);

                profile.UserImage = profilePhoto.FileName;
            }
            if (!UserManager.CheckPassword(CurrentUser, profile.CurrentPassword))
            {
                ViewBag.Message = "كلمة السر الحالية غير صحيحة";
            }
            else
            {
                var newpasswordhash = UserManager.PasswordHasher.HashPassword(profile.NewPassword);
                CurrentUser.UserName = profile.UserName;
                CurrentUser.Email = profile.Email;
                CurrentUser.PasswordHash = newpasswordhash;
                CurrentUser.UserImage = profile.UserImage;
                db.Entry(CurrentUser).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.Message = "تمت عملية التحديث بنجاج";

            }
            return View(profile);
        }
        public ActionResult DisplayProfile(string name,int id=0)
        {

            if(id!=0)
            {
                var bookDetails = db.BookingFoods.Find(id);
                ViewBag.Details = bookDetails;
                var tuple1 = new Tuple<List<BookingFood>, List<PayFine>, List<Complaint>>(BookingByUser(), FineByUser(), ComplaintByUser());
                return View(tuple1);

            }
            //var UserId = User.Identity.GetUserId();
            //List<BookingFood> Booked = db.BookingFoods.Where(a => a.ApplicationUserID == UserId).ToList();
            //ViewBag.BookingByUser = Booked;
            //List<PayFine> payfine = db.PayFines.Where(a => a.UserID == UserId).ToList();
            //ViewBag.FineByUser = payfine;
            //List<Complaint> complaint = db.Complaints.Where(a => a.UserId == UserId).ToList();
            //ViewBag.ComplaintByUser = complaint;
            //return View("DisplayProfile");

            var tuple = new Tuple<List<BookingFood>, List<PayFine>, List<Complaint>>(BookingByUser(), FineByUser(), ComplaintByUser());
            return View(tuple);


        }
        public BookingFood BookingDetails(int id)
        {
            var bookDetails = db.BookingFoods.Find(id);
           
            ViewBag.Details = bookDetails;
            return bookDetails;
        }
        public List<BookingFood> BookingByUser()
        {
            var UserId = User.Identity.GetUserId();
            List<BookingFood> Booked = db.BookingFoods.Where(a => a.ApplicationUserID == UserId).ToList();
            return Booked;
        }
        public List<PayFine> FineByUser()
        {
            var UserId = User.Identity.GetUserId();
            List<PayFine> payfine = db.PayFines.Where(a => a.UserID == UserId).ToList();
            return payfine;
        }

        public List<Complaint> ComplaintByUser()
        {
            var UserId = User.Identity.GetUserId();
            List<Complaint> complaint = db.Complaints.Where(a => a.UserId == UserId).ToList();
            return complaint;
        }
        public ApplicationUser userInformation()
        {
            var UserId = User.Identity.GetUserId();
            var userInfo = db.Users.Where(x => x.Id == UserId).SingleOrDefault();
            return userInfo;
        }
        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "تغير كلمة المرور", "please press here to reset your email password<a href=\"" + callbackUrl + "\">here</a>");
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}