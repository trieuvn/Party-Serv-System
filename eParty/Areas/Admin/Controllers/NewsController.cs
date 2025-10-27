using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using eParty.Models;

namespace eParty.Areas.Admin.Controllers
{
    public class NewsController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: Admin/News
        public ActionResult Index()
        {
            var newsList = db.News.Include(n => n.Author)
                                  .OrderByDescending(n => n.CreatedDate)
                                  .ToList();
            return View(newsList);
        }

        // GET: Admin/News/Create
        public ActionResult Create()
        {
            PopulateUsersDropdown();
            return View();
        }

        // POST: Admin/News/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,Subject,Description,Status,User,IsPublished")] News news, HttpPostedFileBase ImageFile)
        {
            var author = db.SystemUsers.Find(news.User);
            if (author == null)
            {
                ModelState.AddModelError("User", "Selected user does not exist.");
            }

            if (ModelState.IsValid)
            {
                // Convert image sang Base64
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    using (var reader = new System.IO.BinaryReader(ImageFile.InputStream))
                    {
                        news.Image = Convert.ToBase64String(reader.ReadBytes(ImageFile.ContentLength));
                    }
                }

                news.CreatedDate = DateTime.Now;
                news.ViewCount = 0;
                news.Author = author;

                db.News.Add(news);
                db.SaveChanges();

                TempData["SuccessMessage"] = "News created successfully!";
                return RedirectToAction("Index");
            }

            PopulateUsersDropdown(news.User);
            return View(news);
        }

        // GET: Admin/News/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var news = db.News.Find(id);
            if (news == null) return HttpNotFound();

            PopulateUsersDropdown(news.User);
            return View(news);
        }

        // POST: Admin/News/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Subject,Description,Status,User,IsPublished")] News news, HttpPostedFileBase ImageFile)
        {
            var author = db.SystemUsers.Find(news.User);
            if (author == null)
            {
                ModelState.AddModelError("User", "Selected user does not exist.");
                PopulateUsersDropdown(news.User);
                return View(news);
            }

            if (ModelState.IsValid)
            {
                var existing = db.News.Find(news.Id);
                if (existing == null) return HttpNotFound();

                // Update fields
                existing.Name = news.Name;
                existing.Subject = news.Subject;
                existing.Description = news.Description;
                existing.Status = news.Status;
                existing.IsPublished = news.IsPublished;
                existing.User = author.Username;
                existing.Author = author; // đồng bộ FK

                // Chỉ update ảnh nếu có file mới
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    using (var reader = new System.IO.BinaryReader(ImageFile.InputStream))
                    {
                        existing.Image = Convert.ToBase64String(reader.ReadBytes(ImageFile.ContentLength));
                    }
                }

                db.SaveChanges();
                TempData["SuccessMessage"] = "News updated successfully!";
                return RedirectToAction("Index");
            }

            PopulateUsersDropdown(news.User);
            return View(news);
        }

        // GET: Admin/News/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Lấy news cùng với Author
            var news = db.News.Include(n => n.Author)
                              .FirstOrDefault(n => n.Id == id);

            if (news == null)
            {
                return HttpNotFound();
            }

            return View(news);
        }

        // GET: Admin/News/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var news = db.News.Include(n => n.Author).FirstOrDefault(n => n.Id == id);
            if (news == null) return HttpNotFound();

            return View(news);
        }

        // POST: Admin/News/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var news = db.News.Find(id);
            if (news != null)
            {
                db.News.Remove(news);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Populate dropdown list of users cho Create/Edit
        /// </summary>
        /// <param name="selectedUsername"></param>
        private void PopulateUsersDropdown(string selectedUsername = null)
        {
            var users = db.SystemUsers
                .Select(u => new SelectListItem
                {
                    Value = u.Username,
                    Text = string.IsNullOrEmpty(u.FirstName + u.LastName) ? u.Username : u.FirstName + " " + u.LastName
                }).ToList();

            ViewBag.Users = new SelectList(users, "Value", "Text", selectedUsername);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
