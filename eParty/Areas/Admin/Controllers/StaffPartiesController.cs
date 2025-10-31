using eParty.Models;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace eParty.Areas.Admin.Controllers
{
    public class StaffPartiesController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: Admin/StaffParties
        public ActionResult Index()
        {
            var staffParties = db.StaffParties.Include(s => s.PartyRef).Include(s => s.StaffRef);
            return View(staffParties.ToList());
        }

        // GET: Admin/StaffParties/Details
        public ActionResult Details(string staff, int? party)
        {
            if (string.IsNullOrEmpty(staff) || party == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var staffParty = db.StaffParties.Find(staff, party);
            if (staffParty == null)
                return HttpNotFound();

            return View(staffParty);
        }

        // GET: Admin/StaffParties/Create
        public ActionResult Create()
        {
            ViewBag.Staff = new SelectList(db.SystemUsers, "Username", "Username");
            ViewBag.Party = new SelectList(db.Parties, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Staff,Party")] StaffParty staffParty)
        {
            if (ModelState.IsValid)
            {
                db.StaffParties.Add(staffParty);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Staff = new SelectList(db.SystemUsers, "Username", "Username", staffParty.Staff);
            ViewBag.Party = new SelectList(db.Parties, "Id", "Name", staffParty.Party);
            return View(staffParty);
        }

        // GET: Admin/StaffParties/Edit
        public ActionResult Edit(string staff, int? party)
        {
            if (string.IsNullOrEmpty(staff) || party == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var staffParty = db.StaffParties.Find(staff, party);
            if (staffParty == null)
                return HttpNotFound();

            ViewBag.Staff = new SelectList(db.SystemUsers, "Username", "Username", staffParty.Staff);
            ViewBag.Party = new SelectList(db.Parties, "Id", "Name", staffParty.Party);
            return View(staffParty);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Staff,Party")] StaffParty staffParty)
        {
            if (ModelState.IsValid)
            {
                var existing = db.StaffParties.Find(staffParty.Staff, staffParty.Party);
                if (existing != null)
                {
                    // Nếu muốn update các thông tin khác, gán ở đây. Hiện StaffParty chỉ có 2 key, không có field khác.
                    db.Entry(existing).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("", "Staff assignment not found.");
            }

            ViewBag.Staff = new SelectList(db.SystemUsers, "Username", "Username", staffParty.Staff);
            ViewBag.Party = new SelectList(db.Parties, "Id", "Name", staffParty.Party);
            return View(staffParty);
        }

        // GET: Admin/StaffParties/Delete
        public ActionResult Delete(string staff, int? party)
        {
            if (string.IsNullOrEmpty(staff) || party == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var staffParty = db.StaffParties.Find(staff, party);
            if (staffParty == null)
                return HttpNotFound();

            return View(staffParty);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string staff, int party)
        {
            var staffParty = db.StaffParties.Find(staff, party);
            if (staffParty != null)
            {
                db.StaffParties.Remove(staffParty);
                db.SaveChanges();
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
