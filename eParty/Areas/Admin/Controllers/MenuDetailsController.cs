using eParty.Models;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace eParty.Areas.Admin.Controllers
{
    public class MenuDetailsController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: Admin/MenuDetails
        public ActionResult Index()
        {
            var menuDetails = db.MenuDetails
                .Include(m => m.FoodRef)
                .Include(m => m.MenuRef);
            return View(menuDetails.ToList());
        }

        // GET: Admin/MenuDetails/Details/5/3   → menuId=5, foodId=3
        public ActionResult Details(int? menuId, int? foodId)
        {
            if (menuId == null || foodId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            MenuDetail menuDetail = db.MenuDetails
                .FirstOrDefault(m => m.Menu == menuId && m.Food == foodId);

            if (menuDetail == null)
            {
                return HttpNotFound();
            }
            return View(menuDetail);
        }

        // GET: Admin/MenuDetails/Create
        public ActionResult Create()
        {
            ViewBag.Food = new SelectList(db.Foods, "Id", "Name");
            ViewBag.Menu = new SelectList(db.Menus, "Id", "Name");
            return View();
        }

        // POST: Admin/MenuDetails/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Menu,Food,Amount")] MenuDetail menuDetail)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra trùng khóa chính
                bool exists = db.MenuDetails.Any(m => m.Menu == menuDetail.Menu && m.Food == menuDetail.Food);
                if (exists)
                {
                    ModelState.AddModelError("", "This food is already in the menu.");
                }
                else
                {
                    db.MenuDetails.Add(menuDetail);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            ViewBag.Food = new SelectList(db.Foods, "Id", "Name", menuDetail.Food);
            ViewBag.Menu = new SelectList(db.Menus, "Id", "Name", menuDetail.Menu);
            return View(menuDetail);
        }

        // GET: Admin/MenuDetails/Edit/5/3
        public ActionResult Edit(int? menuId, int? foodId)
        {
            if (menuId == null || foodId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            MenuDetail menuDetail = db.MenuDetails
                .FirstOrDefault(m => m.Menu == menuId && m.Food == foodId);

            if (menuDetail == null)
            {
                return HttpNotFound();
            }

            ViewBag.Food = new SelectList(db.Foods, "Id", "Name", menuDetail.Food);
            ViewBag.Menu = new SelectList(db.Menus, "Id", "Name", menuDetail.Menu);
            return View(menuDetail);
        }

        // POST: Admin/MenuDetails/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Menu,Food,Amount")] MenuDetail menuDetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(menuDetail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Food = new SelectList(db.Foods, "Id", "Name", menuDetail.Food);
            ViewBag.Menu = new SelectList(db.Menus, "Id", "Name", menuDetail.Menu);
            return View(menuDetail);
        }

        // GET: Admin/MenuDetails/Delete/5/3
        public ActionResult Delete(int? menuId, int? foodId)
        {
            if (menuId == null || foodId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            MenuDetail menuDetail = db.MenuDetails
                .FirstOrDefault(m => m.Menu == menuId && m.Food == foodId);

            if (menuDetail == null)
            {
                return HttpNotFound();
            }
            return View(menuDetail);
        }

        // POST: Admin/MenuDetails/DeleteConfirmed
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int menuId, int foodId)
        {
            MenuDetail menuDetail = db.MenuDetails
                .FirstOrDefault(m => m.Menu == menuId && m.Food == foodId);

            if (menuDetail != null)
            {
                db.MenuDetails.Remove(menuDetail);
                db.SaveChanges();
            }
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