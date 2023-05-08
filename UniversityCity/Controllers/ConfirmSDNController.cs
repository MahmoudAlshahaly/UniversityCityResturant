using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UniversityCity.Models;

namespace UnivercityCityFood.Controllers
{
    
    public class ConfirmSDNController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: ConfirmSDN
        public ActionResult Index()
        {
            ViewBag.UserGroup = new SelectList(db.Roles, "Name", "Name");
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index([Bind(Include = "UserID,UserIDConfirm,UserGroup")]string txt_UID,string Usergroup)
        {
            //.Where(a => !a.Name.Contains("Admins"))
            ViewBag.UserGroup = new SelectList(db.Roles, "Name", "Name");
            string Message = "";
            using (ApplicationDbContext db=new ApplicationDbContext())
            {   
                if (db.UserSDNs.Where(c => c.UserID == txt_UID && c.UserGroup == Usergroup && c.UserIDConfirm == false).Count() == 1 )
                {
                    Session["MyUID"] = txt_UID;
                    Session["UserGroup"] = Usergroup;
                    return RedirectToAction("Register", "Account");
                }
                else
                {
                    Session["MyUID"] = null;

                    Message = " الرقم القومى هذا غير صحيح او قد تم التسجيل به لحساب من قبل .!!";
                    ViewBag.Message = Message;
                    return View();
                }

            }


        }
    }
}
