using Microsoft.AspNet.Identity;
using PagedList;
using PagedList.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UnivercityCityFood.Models;
using UniversityCity.Models;

namespace UniversityCity.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        [Authorize(Roles = "طالب")]
        public ActionResult Index()
        {
            return View(db.Days.ToList());
        }

        [Authorize(Roles = "طالب")]
        public ActionResult Details(int dayID)
        {
            var day = db.Days.Find(dayID);
            if (day == null)
            {
                return HttpNotFound();
            }
            Session["dayId"] = dayID;
            Session["dayName"] = day.DayName;
            Session["Description"] = day.DayDescription;

            return View(day);
        }
        [Authorize(Roles = "طالب")]
        public ActionResult Booking()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Booking(bool book = true)
            {

            var userInfo = db.Users.Find(User.Identity.GetUserId());
            var Payfine = db.PayFines.Where(a => a.UserID == userInfo.UserID && a.PayFineDone == false).FirstOrDefault();
            if (Payfine != null)
            {
                if (Payfine.PayFineDate.AddDays(1) <= DateTime.Now && Payfine.PayFineDone == false)
                {
                    ViewBag.Message = "عزرا لقد مر من وقت الغرامة 24 ساعة ولم يتم دفعها حتى الان توجه لدفع الغرامة ثم قم بحجز وجبتك";
                    return View();
                }
            }
            DateTime Day = DateTime.Today.AddDays(1);
            var DayAfter = Day.ToString("d");

            DateTime dateValue = DateTime.Parse(DayAfter, CultureInfo.InvariantCulture);
            DateTimeOffset dateOffsetValue = new DateTimeOffset(dateValue, TimeZoneInfo.Local.GetUtcOffset(dateValue));

            bool temp = (int)dateValue.DayOfWeek == (int)Session["dayId"];
            if (temp)
            {

                DateTime thisDay = DateTime.Today;
                var thisDayAfter = thisDay.ToString("d");


                //(DateTime.Now.Hour >= 19 && DateTime.Now.Hour <= 23) == (timeRange = TimeRange.Parse("19:00-23:00"));

                TimeRange timeRange = new TimeRange();
                timeRange = TimeRange.Parse("19:00-23:00");
                bool IsNowInTheRange = timeRange.IsIn(DateTime.Now.TimeOfDay);




                var UserId = User.Identity.GetUserId();
                var DayId = (int)Session["dayId"];

                var Cheak = db.BookingFoods.Where(a => a.ApplicationUserID == UserId && a.BookingDateToDay == thisDayAfter).ToList();

                if (Cheak.Count < 1 && IsNowInTheRange)
                {
                    var booking = new BookingFood();
                    booking.ApplicationUserID = UserId;
                    booking.DayID = DayId;
                    booking.Booked = true;
                    booking.BookingDateToDay = thisDay.ToString("d");
                    booking.BookingTimeToDay = DateTime.Now;
                    booking.Fine = false;
                    db.BookingFoods.Add(booking);
                    db.SaveChanges();
                    ViewBag.Message = "تم حجز الوجبة بنجاح";
                }
                else
                {
                    ViewBag.Message = "عفوا لقد قمت بالحجز مسبقا او قد انتهى وقت حجز الوجبة";
                }
                return View();
            }
            ViewBag.Message = "خطأ  قم بالحجز فى اليوم الذى قادم ولا تحجز فى اى يوم مخالف لليوم القادم وشكرا";
            return View();
        }
        [Authorize(Roles = "طالب")]
        public ActionResult GetBookingByUsers()
        {
            var UserId = User.Identity.GetUserId();
            var Booked = db.BookingFoods.Where(a => a.ApplicationUserID == UserId);
            ViewBag.List = Booked.ToList();
            return View(Booked.ToList());
        }
        [Authorize(Roles = "طالب")]
        public ActionResult GetAllFineByUsers()
        {
            var userInfo = db.Users.Find(User.Identity.GetUserId());

            var payfine = db.PayFines.Where(a => a.UserID == userInfo.UserID);
            return View(payfine.ToList());
        }
        [Authorize(Roles = "طالب ")]
        public ActionResult DetailsOfBooking(int id)
        {
            var day = db.BookingFoods.Find(id);
            if (day == null)
            {
                return HttpNotFound();
            }

            return View(day);
        }
        [Authorize(Roles = "مدير مطعم , مشرف مطعم")]
        public ActionResult Edit(int id)
        {
            BookingFood Booked = db.BookingFoods.Find(id);
            if (Booked == null)
            {
                return HttpNotFound();
            }
            return View(Booked);
        }

        [HttpPost]
        public ActionResult Edit(BookingFood Book)
        {
            Book.Booked = true;
            if (ModelState.IsValid)
            {
                db.Entry(Book).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("GetAllBooking");

            }
            return View(Book);
        }
        [Authorize(Roles = "طالب")]
        public ActionResult Delete(int id)
        {
            var Book = db.BookingFoods.Find(id);
            if (Book == null)
            {
                return HttpNotFound();
            }
            return View(Book);
        }


        [HttpPost]
        public ActionResult Delete(BookingFood Book)
        {
            var myBook = db.BookingFoods.Find(Book.id);
            db.BookingFoods.Remove(myBook);
            db.SaveChanges();
            return RedirectToAction("GetBookingByUsers");
        }


        [Authorize(Roles = "مشرف مطعم , مدير مطعم")]
        public ActionResult GetAllBooking(DateTime? start, DateTime? end, string search,string name)
        {
            ViewBag.Message = Session["Message"];
            BookingFood book = new BookingFood();
            


            if (name== "AllBooking")
            {
                int countOfAllStudentBooking = db.BookingFoods.ToList().Count();
                var StudentBooking = db.BookingFoods.ToList();
                ViewBag.countOfAllStudentBooking = countOfAllStudentBooking;
                return View(StudentBooking);
            }
           
             else if(name== "BookingReceived")
            {
                int countOfAllStudentReceivedBooking = db.BookingFoods.Where(x => x.Received == true).Count();
                var studentReceived = db.BookingFoods.Where(x => x.Received == true).ToList();
                ViewBag.countOfAllStudentReceivedBooking = countOfAllStudentReceivedBooking;
                return View(studentReceived);
            }

           
            else if (name== "BookingToday")
            {
                var Day = DateTime.Today.AddDays(-1).ToString("d");
                DateTime dateValue = DateTime.Parse(Day, CultureInfo.InvariantCulture);

                int countOfthisDayStudentBooking = db.BookingFoods.Where(x => x.DayID == (int)dateValue.DayOfWeek && x.Received == false).Count();
                var StudentBookingToday = db.BookingFoods.Where(x => x.DayID == (int)dateValue.DayOfWeek && x.Received == false).ToList();
               
                return View(StudentBookingToday);
            }
            else if (start != null && end != null && search == "")
            {
                var filter = db.BookingFoods.Where(x => x.BookingTimeToDay >= start && x.BookingTimeToDay <= end).ToList();
                return View(filter);
            }
            else if (start != null && end != null && search != "")
            {
                var collectionData = db.BookingFoods.Where(x => x.appUser.UserName.StartsWith(search) || x.appUser.UserID.StartsWith(search) && x.BookingTimeToDay >= start && x.BookingTimeToDay <= end).ToList();
                return View(collectionData);
            }



            return View(db.BookingFoods.Where(
               x =>
           x.appUser.UserName.StartsWith(search) ||
           x.appUser.UserID.StartsWith(search) ||
           x.appUser.Email.StartsWith(search) ||
           search == null).ToList());
           
  
        }
        [Authorize(Roles = "مشرف مطعم , مدير مطعم")]
        public ActionResult SaveInFine(int id)
        {
            var day = db.BookingFoods.Find(id);
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                if (day == null)
                {
                    return HttpNotFound();
                }
                var cheak = db.PayFines.Where(c => c.UserID == day.appUser.UserID && c.PayFineDone == false && c.DayID == day.DayID).Count() == 1;
                if (!cheak)
                {
                    PayFine fine = new PayFine();
                    fine.DayID = day.DayID;
                    fine.UserID = day.appUser.UserID;
                    fine.UserName = day.appUser.UserName;
                    fine.PayFineDate = DateTime.Now;
                    fine.PayFineDone = false;
                    db.PayFines.Add(fine);
                    db.SaveChanges();


                    var ConfirmFine = db.BookingFoods.Where(c => c.appUser.UserID == day.appUser.UserID && c.Received == false && c.DayID == day.DayID && c.Fine == false).FirstOrDefault();
                    if (ConfirmFine != null)
                    {
                        ConfirmFine.Fine = true;
                        db.SaveChanges();
                    }
                    Session["Message"] = "تم حفظ غرامة بنجاح على  " + fine.UserName;
                    return RedirectToAction("GetAllBooking");
                }
                else
                {
                    Session["Message"] = "ناسف لقد تم حفظ غرامة من قبل على " + day.appUser.UserName;
                    return RedirectToAction("GetAllBooking");
                }
            }
        }

        public ActionResult About()
        {

            return View();
        }

        public ActionResult Contact()
        {

            return View();
        }
        public ActionResult Quetion()
        {
            return View();
        }
    }
}