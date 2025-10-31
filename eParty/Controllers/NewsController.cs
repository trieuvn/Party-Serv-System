using eParty.Models;
using System.Linq;
using System.Web.Mvc;

namespace eParty.Controllers
{
    public class NewsController : Controller
    {
        private readonly AppDbContext _context;

        public NewsController()
        {
            _context = new AppDbContext();
        }

        // GET: News/Detail/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            var news = _context.News
                .Include("Comments.UserRef")
                .Include("Author")
                .FirstOrDefault(n => n.Id == id);

            if (news == null)
            {
                return HttpNotFound();
            }

            // Tăng view count
            news.ViewCount++;
            _context.SaveChanges();

            // Chuẩn bị ViewModel
            var viewModel = new NewsDetailViewModel
            {
                News = news,
                Comments = news.Comments?.OrderByDescending(c => c.Stars).ToList() ?? new System.Collections.Generic.List<Comment>(),
                AverageRating = news.GetAvgStar(),
                TotalComments = news.Comments?.Count ?? 0
            };

            return View(viewModel);
        }

        // POST: News/AddComment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddComment(AddCommentViewModel model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["Error"] = "Bạn cần đăng nhập để comment.";
                return RedirectToAction("Details", new { id = model.NewsId });
            }

            if (ModelState.IsValid)
            {
                // Lấy username từ Identity
                var username = User.Identity.Name;

                // Kiểm tra xem user đã comment vào news này chưa
                var existingComment = _context.Comments
                    .FirstOrDefault(c => c.User == username && c.News == model.NewsId);

                if (existingComment != null)
                {
                    // Cập nhật comment hiện có
                    existingComment.Stars = model.Stars;
                    existingComment.Description = model.Description;
                    TempData["Success"] = "Cập nhật comment thành công!";
                }
                else
                {
                    // Tạo comment mới
                    var comment = new Comment
                    {
                        User = username,
                        News = model.NewsId,
                        Stars = model.Stars,
                        Description = model.Description
                    };

                    _context.Comments.Add(comment);
                    TempData["Success"] = "Thêm comment thành công!";
                }

                _context.SaveChanges();
            }
            else
            {
                // Gửi thông báo lỗi cụ thể để dễ debug
                var firstError = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).Where(m => !string.IsNullOrEmpty(m)));
                TempData["Error"] = string.IsNullOrEmpty(firstError) ? "Dữ liệu không hợp lệ." : firstError;
            }

            return RedirectToAction("Details", new { id = model.NewsId });
        }

        // GET: News/DeleteComment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteComment(int newsId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["Error"] = "Bạn cần đăng nhập.";
                return RedirectToAction("Details", new { id = newsId });
            }

            var username = User.Identity.Name;
            var comment = _context.Comments.FirstOrDefault(c => c.User == username && c.News == newsId);

            if (comment != null)
            {
                _context.Comments.Remove(comment);
                _context.SaveChanges();
                TempData["Success"] = "Xóa comment thành công!";
            }

            return RedirectToAction("Details", new { id = newsId });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}