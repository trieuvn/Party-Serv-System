using eParty.Areas.Staff.Models;
using eParty.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.VisualBasic.ApplicationServices;
using System.Data.Entity;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace eParty.Areas.Staff.Controllers
{
    [Authorize]
    public class PersonalAccountController : Controller
    {
        private ApplicationUserManager _userManager;
        private AppDbContext db = new AppDbContext();
        public ApplicationUserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            private set { _userManager = value; }
        }

        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

        // GET: Update info
        [HttpGet]
        public async Task<ActionResult> UpdateInfo()
        {
            var userId = User.Identity.GetUserId();
            var user = await UserManager.FindByIdAsync(userId);

            if (user == null) return HttpNotFound();
            UserInformation userInfo = new UserInformation
            {
                user = user,
                systemUser = db.SystemUsers.FirstOrDefault(su => su.Username == user.Email)
            };
            return View(userInfo);
        }

        // POST: Update info
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Tên phương thức phải KHỚP VỚI action trong Form (ví dụ: "UpdateProfile" hoặc "UpdateInfo")
        // 1. Thêm tất cả các tham số từ form
        public async Task<ActionResult> UpdateInfo(string FirstName, string LastName, string Avatar, string PhoneNumber)
        {
            var userId = User.Identity.GetUserId();
            var user = await UserManager.FindByIdAsync(userId);
            if (user == null)
            {
                return HttpNotFound();
            }

            // --- 2. Cập nhật ApplicationUser (Identity) ---
            user.PhoneNumber = PhoneNumber;
            var identityResult = await UserManager.UpdateAsync(user);

            // --- 3. Cập nhật SystemUser (Database) ---
            bool dbUpdateSucceeded = false;
            string dbErrors = string.Empty;

            try
            {
                // Tìm SystemUser dựa trên Email của Identity user
                var systemUser = await db.SystemUsers.FirstOrDefaultAsync(su => su.Username == user.Email);

                if (systemUser != null)
                {
                    systemUser.FirstName = FirstName;
                    systemUser.LastName = LastName;
                    systemUser.PhoneNumber = PhoneNumber;

                    // Chỉ cập nhật Avatar nếu người dùng có chọn ảnh mới
                    // (input 'Avatar' sẽ chứa chuỗi Base64 mới)
                    // Nếu bạn muốn giữ ảnh cũ nếu người dùng không chọn, 
                    // logic của file JS (ở câu trả lời trước) đã xử lý việc này.
                    systemUser.Avatar = Avatar; // Đây là chuỗi Base64

                    await db.SaveChangesAsync();
                    dbUpdateSucceeded = true;
                }
                else
                {
                    dbErrors = "Lỗi: Không tìm thấy SystemUser tương ứng.";
                }
            }
            catch (Exception ex)
            {
                // (Nên log lỗi 'ex' vào hệ thống của bạn)
                dbErrors = "Lỗi CSDL khi cập nhật thông tin.";
            }

            // --- 4. Gửi thông báo dựa trên KẾT QUẢ CỦA CẢ HAI ---
            if (identityResult.Succeeded && dbUpdateSucceeded)
            {
                TempData["Message"] = "Cập nhật thông tin thành công!";
            }
            else
            {
                string identityErrors = !identityResult.Succeeded ? string.Join(", ", identityResult.Errors) : "";
                TempData["Message"] = ("Cập nhật thất bại: " + identityErrors + " " + dbErrors).Trim();
            }

            // Chuyển hướng về trang xem/cập nhật thông tin
            // (RedirectToAction("Index") để xem, hoặc "UpdateInfo" để ở lại form)
            return RedirectToAction("Index");
        }

        // GET: Change password
        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View();
        }

        // POST: Change password
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                TempData["Message"] = "Mật khẩu mới và xác nhận không khớp!";
                return RedirectToAction("ChangePassword");
            }

            var userId = User.Identity.GetUserId();
            var result = await UserManager.ChangePasswordAsync(userId, currentPassword, newPassword);

            TempData["Message"] = result.Succeeded
                ? "Đổi mật khẩu thành công!"
                : "Đổi mật khẩu thất bại: " + string.Join(", ", result.Errors);

            return RedirectToAction("ChangePassword");
        }

        // GET: Google link
        public ActionResult GoogleLink()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkGoogle()
        {
            string redirectUrl = Url.Action("LinkGoogleCallback", "PersonalAccount", null, Request.Url.Scheme);
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            properties.Dictionary["XsrfKey"] = User.Identity.GetUserId();
            AuthenticationManager.Challenge(properties, "Google");
            return new HttpUnauthorizedResult();
        }

        public async Task<ActionResult> LinkGoogleCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync("XsrfKey", User.Identity.GetUserId());
            if (loginInfo == null)
            {
                TempData["Message"] = "Không lấy được thông tin từ Google.";
                return RedirectToAction("GoogleLink");
            }

            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            TempData["Message"] = result.Succeeded
                ? "Đã liên kết Google thành công!"
                : "Liên kết Google thất bại: " + string.Join(", ", result.Errors);

            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            return RedirectToAction("GoogleLink");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UnlinkGoogle()
        {
            var userId = User.Identity.GetUserId();
            var login = UserManager.GetLogins(userId).FirstOrDefault(l => l.LoginProvider == "Google");

            if (login == null)
            {
                TempData["Message"] = "Người dùng chưa liên kết Google.";
                return RedirectToAction("GoogleLink");
            }

            var userLogin = new UserLoginInfo(login.LoginProvider, login.ProviderKey);
            var result = await UserManager.RemoveLoginAsync(userId, userLogin);

            TempData["Message"] = result.Succeeded
                ? "Đã ngắt liên kết Google!"
                : "Ngắt liên kết thất bại: " + string.Join(", ", result.Errors);

            return RedirectToAction("GoogleLink");
        }
        public async Task<ActionResult> Index()
        {
            var userId = User.Identity.GetUserId();
            var user = await UserManager.FindByIdAsync(userId);
            if (user == null) return HttpNotFound();
            UserInformation userInfo = new UserInformation
            {
                user = user,
                systemUser = db.SystemUsers.FirstOrDefault(su => su.Username == user.Email)
            };

            return View(userInfo);
        }
    }

}
