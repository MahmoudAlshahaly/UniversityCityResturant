
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
using UnivercityCityFood.Models;
using UniversityCity.Models;

namespace UnivercityCityFood.Controllers
{
    [Authorize(Roles = "Admins")]
    public class UserSDNsController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: UserSDNs
        public ActionResult Index(string search)
        {
            var usersdn = db.UserSDNs.ToList();
            return View(db.UserSDNs.Where(x => x.UserID.StartsWith(search) || search == null).ToList());
        }

        // GET: UserSDNs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserSDN userSDN = db.UserSDNs.Find(id);
            if (userSDN == null)
            {
                return HttpNotFound();
            }
            return View(userSDN);
        }

        // GET: UserSDNs/Create
        public ActionResult Create()
        {
            ViewBag.UserGroup = new SelectList(db.Roles.Where(a => !a.Name.Contains("Admins")), "Name", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,UserID,UserGroup,UserIDConfirm")] UserSDN userSDN)
        {
            ViewBag.UserGroup = new SelectList(db.Roles.Where(a => !a.Name.Contains("Admins")), "Name", "Name");
            if (ModelState.IsValid)
            {
                db.UserSDNs.Add(userSDN);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(userSDN);
        }

        // GET: UserSDNs/Edit/5
        public ActionResult Edit(int? id)
        {
            ViewBag.UserGroup = new SelectList(db.Roles.Where(a => !a.Name.Contains("Admins")), "Name", "Name");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserSDN userSDN = db.UserSDNs.Find(id);
            if (userSDN == null)
            {
                return HttpNotFound();
            }
            return View(userSDN);
        }

        // POST: UserSDNs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,UserID,UserGroup,UserIDConfirm")] UserSDN userSDN)
        {
            ViewBag.UserGroup = new SelectList(db.Roles.Where(a => !a.Name.Contains("Admins")), "Name", "Name");
            if (ModelState.IsValid)
            {
                db.Entry(userSDN).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(userSDN);
        }

        // GET: UserSDNs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserSDN userSDN = db.UserSDNs.Find(id);
            if (userSDN == null)
            {
                return HttpNotFound();
            }
            return View(userSDN);
        }

        // POST: UserSDNs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserSDN userSDN = db.UserSDNs.Find(id);
            db.UserSDNs.Remove(userSDN);
            db.SaveChanges();
            return RedirectToAction("Index");
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
