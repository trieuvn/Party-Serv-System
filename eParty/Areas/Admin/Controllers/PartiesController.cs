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
    public class PartiesController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: Admin/Parties
        public ActionResult Index()
        {
            var parties = db.Parties.Include(p => p.MenuRef).Include(p => p.Owner);
            return View(parties.ToList());
        }

        // GET: Admin/Parties/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Party party = db.Parties.Find(id);
            if (party == null)
            {
                return HttpNotFound();
            }
            return View(party);
        }

        // GET: Admin/Parties/Create
        public ActionResult Create()
        {
            ViewBag.Menu = new SelectList(db.Menus, "Id", "Name");
            ViewBag.User = new SelectList(db.SystemUsers, "Username", "Password");
            return View();
        }

        // POST: Admin/Parties/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Image,Type,Status,Cost,BeginTime,EndTime,CreatedDate,Description,Slots,Address,Latitude,Longitude,User,Menu")] Party party)
        {
            if (ModelState.IsValid)
            {
                db.Parties.Add(party);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Menu = new SelectList(db.Menus, "Id", "Name", party.Menu);
            ViewBag.User = new SelectList(db.SystemUsers, "Username", "Password", party.User);
            return View(party);
        }

        // GET: Admin/Parties/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Party party = db.Parties.Find(id);
            if (party == null)
            {
                return HttpNotFound();
            }
            ViewBag.Menu = new SelectList(db.Menus, "Id", "Name", party.Menu);
            ViewBag.User = new SelectList(db.SystemUsers, "Username", "Password", party.User);
            return View(party);
        }

        // POST: Admin/Parties/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Image,Type,Status,Cost,BeginTime,EndTime,CreatedDate,Description,Slots,Address,Latitude,Longitude,User,Menu")] Party party)
        {
            if (ModelState.IsValid)
            {
                db.Entry(party).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Menu = new SelectList(db.Menus, "Id", "Name", party.Menu);
            ViewBag.User = new SelectList(db.SystemUsers, "Username", "Password", party.User);
            return View(party);
        }

        // GET: Admin/Parties/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Party party = db.Parties.Find(id);
            if (party == null)
            {
                return HttpNotFound();
            }
            return View(party);
        }

        // POST: Admin/Parties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Party party = db.Parties.Find(id);
            db.Parties.Remove(party);
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
