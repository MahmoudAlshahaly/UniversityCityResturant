using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using UnivercityCityFood.Models;
using UniversityCity.Models;
using PagedList;
using PagedList.Mvc;
namespace UnivercityCityFood.Controllers
{
    public class PayFinesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: PayFines
        public ActionResult Index(DateTime ? start,DateTime ? end,string search,string name)
        {
            if(name=="AllFine")
            {
                int countOfStudentPayAndNotPay = db.PayFines.ToList().Count();
                ViewBag.countOfStudentPayAndNotPay = countOfStudentPayAndNotPay;
                var allFinePayAndNotPay = db.PayFines.ToList();
                return View(allFinePayAndNotPay);
            }
            else if(name== "FineNotPay")
            {

                int countOfStudentPayNotDone = db.PayFines.Where(x => x.PayFineDone == false).Count();
                var FineNotPay = db.PayFines.Where(x => x.PayFineDone == false).ToList();
                ViewBag.countOfStudentPayNotDone = countOfStudentPayNotDone;
                return View(FineNotPay);
            }
            else if(name== "FinePay")
            {
                int countOfStudentPayDone = db.PayFines.Where(x => x.PayFineDone == true).Count();
                ViewBag.countOfStudentPayDone = countOfStudentPayDone;
                var FinePay = db.PayFines.Where(x => x.PayFineDone == true).ToList();
                return View(FinePay);
            }
            else if (start != null && end != null && search == "")
            {
                var filter = db.PayFines.Where(x => x.PayFineDate >= start && x.PayFineDate <= end).ToList();
                return View(filter);
            }
            else if (start != null && end != null && search != "")
            {
                var collectionData = db.PayFines.Where(x => x.UserID.StartsWith(search) || x.UserName.StartsWith(search) && x.PayFineDate >= start && x.PayFineDate <= end).ToList();
                return View(collectionData);
            }



            return View(db.PayFines.Where(a=>a.UserID.StartsWith(search) ||a.UserName.StartsWith(search)||search == null).ToList());
        }


        // GET: PayFines/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PayFine payFine = db.PayFines.Find(id);
            Session["UserName"] = payFine.UserName;
            if (payFine == null)
            {
                return HttpNotFound();
            }
            return View(payFine);
        }

      
        [HttpPost]
        public ActionResult Edit(PayFine payFine)
        {
            payFine.UserName = (string)Session["UserName"];
            if (ModelState.IsValid)
            {
                db.Entry(payFine).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(payFine);
        }

      
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
