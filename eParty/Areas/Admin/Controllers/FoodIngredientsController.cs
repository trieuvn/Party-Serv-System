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
    [Authorize(Roles = "Admin")] // <-- THÊM DÒNG NÀY
    public class FoodIngredientsController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: Admin/FoodIngredients
        public ActionResult Index()
        {
            var foodIngredients = db.FoodIngredients.Include(f => f.FoodRef).Include(f => f.IngredientRef);
            return View(foodIngredients.ToList());
        }

        // GET: Admin/FoodIngredients/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FoodIngredient foodIngredient = db.FoodIngredients.Find(id);
            if (foodIngredient == null)
            {
                return HttpNotFound();
            }
            return View(foodIngredient);
        }

        // GET: Admin/FoodIngredients/Create
        public ActionResult Create()
        {
            ViewBag.Food = new SelectList(db.Foods, "Id", "Name");
            ViewBag.Ingredient = new SelectList(db.Ingredients, "Id", "Name");
            return View();
        }

        // POST: Admin/FoodIngredients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Food,Ingredient,Amount")] FoodIngredient foodIngredient)
        {
            if (ModelState.IsValid)
            {
                db.FoodIngredients.Add(foodIngredient);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Food = new SelectList(db.Foods, "Id", "Name", foodIngredient.Food);
            ViewBag.Ingredient = new SelectList(db.Ingredients, "Id", "Name", foodIngredient.Ingredient);
            return View(foodIngredient);
        }

        // GET: Admin/FoodIngredients/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FoodIngredient foodIngredient = db.FoodIngredients.Find(id);
            if (foodIngredient == null)
            {
                return HttpNotFound();
            }
            ViewBag.Food = new SelectList(db.Foods, "Id", "Name", foodIngredient.Food);
            ViewBag.Ingredient = new SelectList(db.Ingredients, "Id", "Name", foodIngredient.Ingredient);
            return View(foodIngredient);
        }

        // POST: Admin/FoodIngredients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Food,Ingredient,Amount")] FoodIngredient foodIngredient)
        {
            if (ModelState.IsValid)
            {
                db.Entry(foodIngredient).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Food = new SelectList(db.Foods, "Id", "Name", foodIngredient.Food);
            ViewBag.Ingredient = new SelectList(db.Ingredients, "Id", "Name", foodIngredient.Ingredient);
            return View(foodIngredient);
        }

        // GET: Admin/FoodIngredients/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FoodIngredient foodIngredient = db.FoodIngredients.Find(id);
            if (foodIngredient == null)
            {
                return HttpNotFound();
            }
            return View(foodIngredient);
        }

        // POST: Admin/FoodIngredients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FoodIngredient foodIngredient = db.FoodIngredients.Find(id);
            db.FoodIngredients.Remove(foodIngredient);
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
