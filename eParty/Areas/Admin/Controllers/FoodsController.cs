using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using eParty.Areas.Admin.Models;
using eParty.Models;
using eParty.Utils;

namespace eParty.Areas.Admin.Controllers
{
    public class FoodsController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: Admin/Foods
        public ActionResult Index()
        {
            var foods = db.Foods.Include(f => f.Category);
            return View(foods.ToList());
        }

        // GET: Admin/Foods/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            // ✨ THÊM .Include(f => f.Category)
            var food = db.Foods.Include(f => f.Category).SingleOrDefault(f => f.Id == id);

            if (food == null) return HttpNotFound();

            var viewModel = new FoodRecipeViewModel
            {
                Food = food,
                Ingredients = db.FoodIngredients.Include(fi => fi.IngredientRef).Where(fi => fi.Food == id).ToList()
            };
            return View(viewModel);
        }

        // GET: Admin/Foods/Create
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name");
            return View();
        }

        // POST: Admin/Foods/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,Description,Unit,Cost,Discount,CategoryId")] Food food, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null)
                {
                    food.Image = StringUtils.ImageFileToBase64(imageFile);
                }
                db.Foods.Add(food);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Food created! You can now add ingredients.";
                return RedirectToAction("Edit", new { id = food.Id });
            }

            // Nếu thất bại, tải lại danh sách Category
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", food.CategoryId);
            return View(food);
        }

        // GET: Admin/Foods/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var food = db.Foods.Find(id);
            if (food == null) return HttpNotFound();

            var viewModel = new FoodRecipeViewModel
            {
                Food = food,
                Ingredients = db.FoodIngredients.Include(fi => fi.IngredientRef).Where(fi => fi.Food == id).ToList(),
                AllIngredients = db.Ingredients.ToList().Select(i => new SelectListItem { Value = i.Id.ToString(), Text = i.Name })
            };

            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", food.CategoryId);
            return View(viewModel);
        }

        // POST: Admin/Foods/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FoodRecipeViewModel viewModel, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValidField("Food"))
            {
                var foodToUpdate = db.Foods.Find(viewModel.Food.Id);
                if (foodToUpdate == null) return HttpNotFound();

                foodToUpdate.Name = viewModel.Food.Name;
                foodToUpdate.Description = viewModel.Food.Description;
                foodToUpdate.Unit = viewModel.Food.Unit;
                foodToUpdate.Cost = viewModel.Food.Cost;
                foodToUpdate.Discount = viewModel.Food.Discount;
                foodToUpdate.CategoryId = viewModel.Food.CategoryId;

                if (imageFile != null)
                {
                    foodToUpdate.Image = StringUtils.ImageFileToBase64(imageFile);
                }

                db.Entry(foodToUpdate).State = EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMessage"] = "Food info updated successfully!";
                return RedirectToAction("Edit", new { id = viewModel.Food.Id });
            }

            // Nếu có lỗi, tải lại dữ liệu cần thiết cho View
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", viewModel.Food.CategoryId);
            viewModel.AllIngredients = db.Ingredients.ToList().Select(i => new SelectListItem { Value = i.Id.ToString(), Text = i.Name });
            viewModel.Ingredients = db.FoodIngredients.Include(fi => fi.IngredientRef).Where(fi => fi.Food == viewModel.Food.Id).ToList();
            return View(viewModel);
        }

        // ... (Delete Actions) ...

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddIngredient(int foodId, string newIngredientId, string newIngredientAmount)
        {
            if (int.TryParse(newIngredientId, out int ingredientIdAsInt) && ingredientIdAsInt > 0)
            {
                // ✨ SỬA LỖI Ở ĐÂY: Chuyển đổi newIngredientAmount sang int
                if (int.TryParse(newIngredientAmount, out int amountAsInt))
                {
                    var existing = db.FoodIngredients.Find(foodId, ingredientIdAsInt);
                    if (existing == null)
                    {
                        db.FoodIngredients.Add(new FoodIngredient
                        {
                            Food = foodId,
                            Ingredient = ingredientIdAsInt,
                            Amount = amountAsInt // Gán giá trị đã chuyển đổi
                        });
                        db.SaveChanges();
                        TempData["SuccessMessage"] = "Ingredient added.";
                    }
                    else { TempData["ErrorMessage"] = "Ingredient already exists."; }
                }
                else { TempData["ErrorMessage"] = "Amount must be a valid number."; }
            }
            else { TempData["ErrorMessage"] = "Please select a valid ingredient."; }
            return RedirectToAction("Edit", new { id = foodId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateIngredientAmount(int foodId, int ingredientId, string amount)
        {
            var ingredientToUpdate = db.FoodIngredients.Find(foodId, ingredientId);
            if (ingredientToUpdate != null)
            {
                // ✨ SỬA LỖI Ở ĐÂY: Chuyển đổi amount sang int
                if (int.TryParse(amount, out int amountAsInt))
                {
                    ingredientToUpdate.Amount = amountAsInt; // Gán giá trị đã chuyển đổi
                    db.Entry(ingredientToUpdate).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Amount updated!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Amount must be a valid number.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Ingredient not found!";
            }
            return RedirectToAction("Edit", new { id = foodId });
        }

        // ... (Dispose method, Delete actions...)
    }
}