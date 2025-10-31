using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using eParty.Models;

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
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemUser systemUser = db.SystemUsers.Find(id);
            if (systemUser == null)
            {
                return HttpNotFound();
            }
            return View(systemUser);
        }

        // GET: Admin/SystemUsers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/SystemUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Username,Password,FirstName,LastName,Avatar,Email,PhoneNumber,Role,Salary")] SystemUser systemUser)
        {
            if (ModelState.IsValid)
            {
                db.SystemUsers.Add(systemUser);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(systemUser);
        }

        // GET: Admin/SystemUsers/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemUser systemUser = db.SystemUsers.Find(id);
            if (systemUser == null)
            {
                return HttpNotFound();
            }
            return View(systemUser);
        }

        // POST: Admin/SystemUsers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Username,Password,FirstName,LastName,Avatar,Email,PhoneNumber,Role,Salary")] SystemUser systemUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(systemUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(systemUser);
        }

        // GET: Admin/SystemUsers/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemUser systemUser = db.SystemUsers.Find(id);
            if (systemUser == null)
            {
                return HttpNotFound();
            }
            return View(systemUser);
        }

        // POST: Admin/SystemUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            SystemUser systemUser = db.SystemUsers.Find(id);
            db.SystemUsers.Remove(systemUser);
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
