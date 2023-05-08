using Microsoft.AspNet.Identity;
using PagedList;
using PagedList.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using UnivercityCityFood;
using UnivercityCityFood.Models;
using UniversityCity.Models;

namespace UnivercityCityFood.Controllers
{
    public class ComplaintsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        // GET: Complaints/Create
        [Authorize(Roles = "طالب")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(String Message)
        {
            var UserId = User.Identity.GetUserId();

            var check = db.Complaints.Where(a => a.UserId == UserId && a.CompContent == Message).ToList();
            if (check.Count < 1)
            {
                var complaint = new Complaint();
                complaint.UserId = UserId;
                complaint.CompDate = DateTime.Now;
                complaint.CompContent = Message;

                db.Complaints.Add(complaint);
                db.SaveChanges();
                ViewBag.Result = "تم ارسال الرساله بنجاح شكرا لمساعدتك فى التطوير .";
            }
            else
            {
                ViewBag.Result = "لقد سبق وارسلت هذه الرسالة من قبل ";
            }
            return View();
        }
        [Authorize(Roles = "طالب")]
        public ActionResult GetComplaintByUsers()
        {
            var UserId = User.Identity.GetUserId();
            var complaint = db.Complaints.Where(a => a.UserId == UserId);
            return View(complaint.ToList());
        }

        [Authorize(Roles = "طالب , مدير مطعم")]
        public ActionResult DetailsOfComplaint(int id)
        {
            var complaint = db.Complaints.Find(id);
            if (complaint == null)
            {
                return HttpNotFound();
            }

            return View(complaint);
        }

        [Authorize(Roles = "طالب")]
        public ActionResult Delete(int id)
        {
            var complaint = db.Complaints.Find(id);
            if (complaint == null)
            {
                return HttpNotFound();
            }
            return View(complaint);
        }


        [HttpPost]
        public ActionResult Delete(Complaint complaint)
        {
            var mycomplaint = db.Complaints.Find(complaint.Id);
            db.Complaints.Remove(mycomplaint);
            db.SaveChanges();
            return RedirectToAction("GetComplaintByUsers");

        }

        [Authorize(Roles = "مدير مطعم")]
        public ActionResult GetAllComplaints(string search, int? i)
        {
            var AllComplaint = from complaint in db.Complaints
                               select complaint;

            return View(AllComplaint.Where(x => x.User.UserName.StartsWith(search) ||
            x.User.UserID.StartsWith(search) ||
            x.User.Email.StartsWith(search) ||
            x.User.UserFaculty.StartsWith(search) ||
            x.CompContent.StartsWith(search) ||
            search == null).ToList().ToPagedList(i ?? 1, 5));

            //return View(AllComplaint.ToList());
        }


    }
}
