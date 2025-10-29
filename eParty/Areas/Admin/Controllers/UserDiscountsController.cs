using eParty.Models;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace eParty.Areas.Admin.Controllers
{
    public class UserDiscountsController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: Admin/UserDiscounts
        public ActionResult Index()
        {
            var userDiscounts = db.UserDiscounts
                .Include(u => u.UserRef)
                .Include(u => u.DiscountRef);
            return View(userDiscounts.ToList());
        }

        // GET: Admin/UserDiscounts/Details
        public ActionResult Details(string user, int discount)
        {
            if (string.IsNullOrEmpty(user))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var userDiscount = db.UserDiscounts
                .Include(u => u.UserRef)
                .Include(u => u.DiscountRef)
                .FirstOrDefault(u => u.User == user && u.Discount == discount);

            if (userDiscount == null)
                return HttpNotFound();

            return View(userDiscount);
        }

        // GET: Admin/UserDiscounts/Create
        public ActionResult Create()
        {
            ViewBag.Discount = new SelectList(db.Discounts, "Id", "CouponCode");
            ViewBag.User = new SelectList(db.SystemUsers, "Username", "Username");
            return View();
        }

        // POST: Admin/UserDiscounts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "User,Discount,Amount")] UserDiscount userDiscount)
        {
            if (ModelState.IsValid)
            {
                db.UserDiscounts.Add(userDiscount);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Discount = new SelectList(db.Discounts, "Id", "CouponCode", userDiscount.Discount);
            ViewBag.User = new SelectList(db.SystemUsers, "Username", "Username", userDiscount.User);
            return View(userDiscount);
        }

        // GET: Admin/UserDiscounts/Edit
        public ActionResult Edit(string user, int discount)
        {
            if (string.IsNullOrEmpty(user))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var userDiscount = db.UserDiscounts
                .Include(u => u.UserRef)
                .Include(u => u.DiscountRef)
                .FirstOrDefault(u => u.User == user && u.Discount == discount);

            if (userDiscount == null)
                return HttpNotFound();

            return View(userDiscount);
        }

        // POST: Admin/UserDiscounts/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UserDiscount model)
        {
            if (ModelState.IsValid)
            {
                var userDiscount = db.UserDiscounts
                    .FirstOrDefault(u => u.User == model.User && u.Discount == model.Discount);

                if (userDiscount != null)
                {
                    userDiscount.Amount = model.Amount;
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: Admin/UserDiscounts/Delete
        public ActionResult Delete(string user, int discount)
        {
            if (string.IsNullOrEmpty(user))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var userDiscount = db.UserDiscounts
                .Include(u => u.UserRef)
                .Include(u => u.DiscountRef)
                .FirstOrDefault(u => u.User == user && u.Discount == discount);

            if (userDiscount == null)
                return HttpNotFound();

            return View(userDiscount);
        }

        // POST: Admin/UserDiscounts/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string user, int discount)
        {
            var userDiscount = db.UserDiscounts
                .FirstOrDefault(u => u.User == user && u.Discount == discount);

            if (userDiscount != null)
            {
                db.UserDiscounts.Remove(userDiscount);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
