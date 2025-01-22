using Kutse_App.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Kutse_App.Controllers
{
    public class HomeController : Controller
    {

        private static readonly Dictionary<int, string> Pidu = new Dictionary<int, string>
        {
            {1, "Head uut aastat!"},
            {2, "Head Eesti iseseisvuspäeva!"},
            {12, "Haid jõule"}
        };

        public ActionResult Index()
        {
            string greeting;



            int month = DateTime.Now.Month;
            int hour = DateTime.Now.Hour;

            if (hour >= 6 && hour < 12)
            {
                greeting = "Tere hommikust!";
            }
            else if (hour >= 12 && hour < 18)
            {
                greeting = "Tere päevast!";
            }
            else if (hour >= 18 && hour < 21)
            {
                greeting = "Tere õhtust!";
            }
            else
            {
                greeting = "Head ööd!";
            }

            ViewBag.Greeting = greeting;
            string holidayMessage = Pidu.ContainsKey(month) ? Pidu[month] : "";

            ViewBag.Greeting = greeting + (string.IsNullOrEmpty(holidayMessage) ? "" : " " + holidayMessage);
            ViewBag.Message = "Ootan sind minu peole! Palun tule!!!";
            return View();
        }

        [HttpGet]

        public ViewResult Ankeet()
        {
            return View();
        }

        [HttpPost]

        public ViewResult Ankeet(Guest guest)
        {
            E_mail(guest);
            if (ModelState.IsValid)
            {
                db.Guests.Add(guest);
                db.SaveChanges();
                return View("Thanks", guest);
            }
            else { return View(); }
        }
        public void E_mail(Guest guest)
        {
            try
            {
                WebMail.SmtpServer = "smtp.gmail.com";
                WebMail.SmtpPort = 587;
                WebMail.EnableSsl = true;
                WebMail.UserName = "jelizaveta.ostapjuk.work@gmail.com";
                WebMail.Password = "lsrs danp cdwm ogmd ";
                WebMail.From = "jelizaveta.ostapjuk.work@gmail.com";
                WebMail.Send(guest.Email, " Vastus kutsele ", guest.Name + " vastas " + ((guest.WillAttend ?? false ? " tuleb peole" : " ei tule saatnud")));
                ViewBag.Message = "Kiri on saatnud!";
            }
            catch (Exception)
            {
                ViewBag.Message = "Mul on kahju! Ei saa kirja saada!!!";
            }
        }

        [HttpPost]
        public ActionResult Meeldetuletus(Guest guest, string Meeldetuletus)
        {
            if (!string.IsNullOrEmpty(Meeldetuletus))
            {
                try
                {
                    WebMail.SmtpServer = "smtp.gmail.com";
                    WebMail.SmtpPort = 587;
                    WebMail.EnableSsl = true;
                    WebMail.UserName = "jelizaveta.ostapjuk.work@gmail.com";
                    WebMail.Password = "lsrs danp cdwm ogmd ";
                    WebMail.From = "jelizaveta.ostapjuk.work@gmail.com";

                    WebMail.Send(guest.Email, "Meeldetuletus", guest.Name + ", ara unusta. Pidu toimub 20.01.25! Sind ootavad väga!",
                    null, "jelizaveta.ostapjuk.work@gmail.com",
                    filesToAttach: new String[] { Path.Combine(Server.MapPath("~/Images/"), Path.GetFileName("yippy.jpg ")) }
                   );

                    ViewBag.Message = "Kutse saadetud!";
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "Tekkis viga kutse saatmisel: " + ex.Message;
                }
            }

            return View("Aitäh", guest);
        }
        GuestContext db = new GuestContext();

        [Authorize(Roles = "User")]
        public ActionResult Guests() 
        {
            IEnumerable<Guest> guests = db.Guests;
            return View(guests);
        }

        [Authorize(Roles = "User")]
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public ActionResult Create(Guest guest)
        {
            db.Guests.Add(guest);
            db.SaveChanges();
            return RedirectToAction("Guests");
        }

        [Authorize(Roles = "User")]
        [HttpGet]
        public ActionResult Delete(int id)
        {
            Guest g = db.Guests.Find(id);
            if (g==null)
            {
                return HttpNotFound();
            }
            return View(g);
        }

        [Authorize(Roles = "User")]
        [HttpPost,ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Guest g = db.Guests.Find(id);
            if (g == null)
            {
                return HttpNotFound();
            }
            db.Guests.Remove(g);
            db.SaveChanges();
            return RedirectToAction("Guests");
        }

        [Authorize(Roles = "User")]
        [HttpGet]
        public ActionResult Edit(int? id) 
        {
            Guest g = db.Guests.Find(id);
            if (g == null)
            {
                return HttpNotFound();
            }
            return View(g);
        }

        [Authorize(Roles = "User")]
        [HttpPost, ActionName("Edit")]
        public ActionResult EditConfirmed(Guest guest)
        {
            db.Entry(guest).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Guests");
        }

        [Authorize(Roles = "User")]
        public ActionResult WillAttendGuests()
        {
            var guests = db.Guests.Where(g => g.WillAttend == true).ToList();
            return View("Guests", guests);
        }

        [Authorize(Roles = "User")]
        public ActionResult NotAttendingGuests()
        {
            var guests = db.Guests.Where(g => g.WillAttend == false).ToList();
            return View("Guests", guests);
        }
        [Authorize(Roles = "User")]
        public ActionResult AllGuests()
        {
            var guests = db.Guests.ToList();
            return View("Guests", guests);
        }
        [Authorize(Roles = "Admin")]
        public ActionResult Holidays()
        {
            IEnumerable<Holiday> holidays = db.Holidays;
            return View(holidays);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Create2()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Create2(Holiday holiday)
        {
            db.Holidays.Add(holiday);
            db.SaveChanges();
            return RedirectToAction("Holidays");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Delete2(int id)
        {
            Holiday h = db.Holidays.Find(id);
            if (h == null)
            {
                return HttpNotFound();
            }
            return View(h);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete2")]
        public ActionResult DeleteConfirmed2(int id)
        {
            Holiday h = db.Holidays.Find(id);
            if (h == null)
            {
                return HttpNotFound();
            }
            db.Holidays.Remove(h);
            db.SaveChanges();
            return RedirectToAction("Holidays");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit2(int? id)
        {
            Holiday h = db.Holidays.Find(id);
            if (h == null)
            {
                return HttpNotFound();
            }
            return View(h);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Edit2")]
        public ActionResult EditConfirmed2(Holiday holiday)
        {
            db.Entry(holiday).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Holidays");
        }
    }
}