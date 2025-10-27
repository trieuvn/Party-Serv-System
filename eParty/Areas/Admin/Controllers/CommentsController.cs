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
    public class CommentsController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: Admin/Comments
        public ActionResult Index()
        {
            var comments = db.Comments.Include(c => c.NewsRef).Include(c => c.UserRef);
            return View(comments.ToList());
        }
        // GET: Admin/Comments/Details?userId=xxx&newsId=yyy
        public ActionResult Details(string userId, int newsId)
        {
            if (string.IsNullOrEmpty(userId))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var comment = db.Comments
                            .Include(c => c.NewsRef)
                            .Include(c => c.UserRef)
                            .FirstOrDefault(c => c.User == userId && c.News == newsId);

            if (comment == null)
                return HttpNotFound();

            return View(comment);
        }


        // GET: Admin/Comments/Create
        public ActionResult Create()
        {
            ViewBag.News = new SelectList(db.News, "Id", "Name");
            ViewBag.User = new SelectList(db.Users, "Username", "Password");
            return View();
        }

        // POST: Admin/Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "User,News,Stars,Description")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                db.Comments.Add(comment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.News = new SelectList(db.News, "Id", "Name", comment.News);
            ViewBag.User = new SelectList(db.Users, "Username", "Password", comment.User);
            return View(comment);
        }

        // GET: Admin/Comments/Edit/5
        public ActionResult Edit(string userId, int newsId)
        {
            if (string.IsNullOrEmpty(userId))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var comment = db.Comments.FirstOrDefault(c => c.User == userId && c.News == newsId);
            if (comment == null)
                return HttpNotFound();

            return View(comment);
        }


        // POST: Admin/Comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "User,News,Stars,Description")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                var existing = db.Comments.FirstOrDefault(c => c.User == comment.User && c.News == comment.News);
                if (existing == null)
                    return HttpNotFound();

                existing.Stars = comment.Stars;
                existing.Description = comment.Description;
                db.SaveChanges();

                TempData["SuccessMessage"] = "Comment updated successfully!";
                return RedirectToAction("Index");
            }
            return View(comment);
        }


        // GET: Admin/Comments/Delete/5
        public ActionResult Delete(string userId, int newsId)
        {
            if (string.IsNullOrEmpty(userId))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var comment = db.Comments.FirstOrDefault(c => c.User == userId && c.News == newsId);
            if (comment == null)
                return HttpNotFound();

            return View(comment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string userId, int newsId)
        {
            var comment = db.Comments.FirstOrDefault(c => c.User == userId && c.News == newsId);
            if (comment != null)
            {
                db.Comments.Remove(comment);
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
