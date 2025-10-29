using eParty.Models;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace eParty.Areas.Admin.Controllers
{
    public class RatesController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: Admin/Rates
        public ActionResult Index()
        {
            var rates = db.Rates
                .Include(r => r.PartyRef)
                .Include(r => r.UserRef)
                .ToList();
            return View(rates);
        }

        // GET: Admin/Rates/Details
        public ActionResult Details(string user, int? party)
        {
            if (string.IsNullOrEmpty(user) || party == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var rate = db.Rates
                .Include(r => r.PartyRef)
                .Include(r => r.UserRef)
                .AsNoTracking()
                .FirstOrDefault(r => r.User == user && r.Party == party);

            if (rate == null)
                return HttpNotFound();

            return View(rate);
        }

        // GET: Admin/Rates/Create
        public ActionResult Create()
        {
            ViewBag.Party = new SelectList(db.Parties, "Id", "Name");
            ViewBag.User = new SelectList(db.SystemUsers, "Username", "Username");
            return View();
        }

        // POST: Admin/Rates/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "User,Party,Stars,Comment")] Rate rate)
        {
            if (ModelState.IsValid)
            {
                db.Rates.Add(rate);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Party = new SelectList(db.Parties, "Id", "Name", rate.Party);
            ViewBag.User = new SelectList(db.SystemUsers, "Username", "Username", rate.User);
            return View(rate);
        }

        // GET: Admin/Rates/Edit
        public ActionResult Edit(string user, int? party)
        {
            if (string.IsNullOrEmpty(user) || party == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var rate = db.Rates
                .Include(r => r.PartyRef)
                .Include(r => r.UserRef)
                .FirstOrDefault(r => r.User == user && r.Party == party);

            if (rate == null)
                return HttpNotFound();

            ViewBag.Party = new SelectList(db.Parties, "Id", "Name", rate.Party);
            ViewBag.User = new SelectList(db.SystemUsers, "Username", "Username", rate.User);
            return View(rate);
        }


        // POST: Admin/Rates/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "User,Party,Stars,Comment")] Rate rate)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rate).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Party = new SelectList(db.Parties, "Id", "Name", rate.Party);
            ViewBag.User = new SelectList(db.SystemUsers, "Username", "Username", rate.User);
            return View(rate);
        }

        // GET: Admin/Rates/Delete
        public ActionResult Delete(string user, int? party)
        {
            if (string.IsNullOrEmpty(user) || party == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var rate = db.Rates
                .Include(r => r.PartyRef)
                .Include(r => r.UserRef)
                .FirstOrDefault(r => r.User == user && r.Party == party);

            if (rate == null)
                return HttpNotFound();

            return View(rate);
        }

        // POST: Admin/Rates/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string user, int party)
        {
            var rate = db.Rates.FirstOrDefault(r => r.User == user && r.Party == party);
            if (rate != null)
            {
                db.Rates.Remove(rate);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Rating deleted successfully!";
            }
            return RedirectToAction("Index");
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}
