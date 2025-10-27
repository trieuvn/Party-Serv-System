using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using eParty.Models;
using System.IO;

namespace eParty.Areas.Admin.Controllers
{
    public class SystemUsersController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: Admin/SystemUsers
        public ActionResult Index()
        {
            return View(db.SystemUsers.ToList());
        }

        // GET: Admin/SystemUsers/Details/5
        public ActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var user = db.SystemUsers.Find(id);
            if (user == null) return HttpNotFound();

            return View(user);
        }

        // GET: Admin/SystemUsers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/SystemUsers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Username,Password,FirstName,LastName,Email,PhoneNumber,Role")] SystemUser user, HttpPostedFileBase avatarFile)
        {
            if (ModelState.IsValid)
            {
                if (avatarFile != null && avatarFile.ContentLength > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        avatarFile.InputStream.CopyTo(ms);
                        user.Avatar = Convert.ToBase64String(ms.ToArray());
                    }
                }

                db.SystemUsers.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(user);
        }

        // GET: Admin/SystemUsers/Edit/5
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var user = db.SystemUsers.Find(id);
            if (user == null) return HttpNotFound();

            return View(user);
        }

        // POST: Admin/SystemUsers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Username,Password,FirstName,LastName,Email,PhoneNumber,Role")] SystemUser user, HttpPostedFileBase avatarFile)
        {
            if (ModelState.IsValid)
            {
                var userInDb = db.SystemUsers.Find(user.Username);
                if (userInDb == null) return HttpNotFound();

                userInDb.FirstName = user.FirstName;
                userInDb.LastName = user.LastName;
                userInDb.Email = user.Email;
                userInDb.PhoneNumber = user.PhoneNumber;
                userInDb.Password = user.Password;
                userInDb.Role = user.Role;

                if (avatarFile != null && avatarFile.ContentLength > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        avatarFile.InputStream.CopyTo(ms);
                        userInDb.Avatar = Convert.ToBase64String(ms.ToArray());
                    }
                }

                db.Entry(userInDb).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: Admin/SystemUsers/Delete/5
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var user = db.SystemUsers.Find(id);
            if (user == null) return HttpNotFound();

            return View(user);
        }

        // POST: Admin/SystemUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            if (string.IsNullOrEmpty(id)) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var user = db.SystemUsers.Find(id);
            if (user == null) return HttpNotFound();

            db.SystemUsers.Remove(user);
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
