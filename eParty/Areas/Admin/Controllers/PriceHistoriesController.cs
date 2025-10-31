using eParty.Models;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace eParty.Areas.Admin.Controllers
{
    public class PriceHistoriesController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: Admin/PriceHistories
        public ActionResult Index()
        {
            var priceHistories = db.PriceHistories
                .Include(p => p.FoodRef)
                .Include(p => p.PartyRef);
            return View(priceHistories.ToList());
        }

        // GET: Admin/PriceHistories/Details
        public ActionResult Details(int party, int food)
        {
            var priceHistory = db.PriceHistories
                .Include(p => p.FoodRef)
                .Include(p => p.PartyRef)
                .FirstOrDefault(p => p.Party == party && p.Food == food);

            if (priceHistory == null)
                return HttpNotFound();

            return View(priceHistory);
        }

        // GET: Admin/PriceHistories/Create
        public ActionResult Create()
        {
            ViewBag.Food = new SelectList(db.Foods, "Id", "Name");
            ViewBag.Party = new SelectList(db.Parties, "Id", "Name");
            return View();
        }

        // POST: Admin/PriceHistories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Party,Food,Cost,Amount")] PriceHistory priceHistory)
        {
            if (ModelState.IsValid)
            {
                db.PriceHistories.Add(priceHistory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Food = new SelectList(db.Foods, "Id", "Name", priceHistory.Food);
            ViewBag.Party = new SelectList(db.Parties, "Id", "Name", priceHistory.Party);
            return View(priceHistory);
        }

        // GET: Admin/PriceHistories/Edit
        public ActionResult Edit(int party, int food)
        {
            var priceHistory = db.PriceHistories
                .Include(p => p.FoodRef)
                .Include(p => p.PartyRef)
                .FirstOrDefault(p => p.Party == party && p.Food == food);

            if (priceHistory == null)
                return HttpNotFound();

            ViewBag.Food = new SelectList(db.Foods, "Id", "Name", priceHistory.Food);
            ViewBag.Party = new SelectList(db.Parties, "Id", "Name", priceHistory.Party);
            return View(priceHistory);
        }

        // POST: Admin/PriceHistories/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Party,Food,Cost,Amount")] PriceHistory priceHistory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(priceHistory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Food = new SelectList(db.Foods, "Id", "Name", priceHistory.Food);
            ViewBag.Party = new SelectList(db.Parties, "Id", "Name", priceHistory.Party);
            return View(priceHistory);
        }

        // GET: Admin/PriceHistories/Delete
        public ActionResult Delete(int party, int food)
        {
            var priceHistory = db.PriceHistories
                .Include(p => p.FoodRef)
                .Include(p => p.PartyRef)
                .FirstOrDefault(p => p.Party == party && p.Food == food);

            if (priceHistory == null)
                return HttpNotFound();

            return View(priceHistory);
        }

        // POST: Admin/PriceHistories/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int party, int food)
        {
            var priceHistory = db.PriceHistories.FirstOrDefault(p => p.Party == party && p.Food == food);
            db.PriceHistories.Remove(priceHistory);
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
