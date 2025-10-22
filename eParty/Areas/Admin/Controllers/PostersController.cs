using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using eParty.Models;

namespace eParty.Areas.Admin.Controllers
{
    public class PostersController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: Admin/Posters
        public ActionResult Index()
        {
            return View(db.Posters.ToList());
        }

        // GET: Admin/Posters/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var poster = db.Posters.Find(id);
            if (poster == null) return HttpNotFound();

            return View(poster);
        }

        // GET: Admin/Posters/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Posters/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] Poster poster, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        imageFile.InputStream.CopyTo(ms);
                        poster.Image = Convert.ToBase64String(ms.ToArray());
                    }
                }

                db.Posters.Add(poster);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(poster);
        }

        // GET: Admin/Posters/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var poster = db.Posters.Find(id);
            if (poster == null) return HttpNotFound();

            return View(poster);
        }

        // POST: Admin/Posters/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Image")] Poster poster, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                var posterInDb = db.Posters.Find(poster.Id);
                if (posterInDb == null) return HttpNotFound();

                posterInDb.Name = poster.Name;

                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        imageFile.InputStream.CopyTo(ms);
                        posterInDb.Image = Convert.ToBase64String(ms.ToArray());
                    }
                }

                db.Entry(posterInDb).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(poster);
        }

        // GET: Admin/Posters/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var poster = db.Posters.Find(id);
            if (poster == null) return HttpNotFound();

            return View(poster);
        }

        // POST: Admin/Posters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var poster = db.Posters.Find(id);
            db.Posters.Remove(poster);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
