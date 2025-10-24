using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
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
            return View(user);
        }

        // POST: Update info
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateInfo(string phoneNumber)
        {
            var userId = User.Identity.GetUserId();
            var user = await UserManager.FindByIdAsync(userId);
            if (user == null) return HttpNotFound();

            user.PhoneNumber = phoneNumber;
            var result = await UserManager.UpdateAsync(user);

            TempData["Message"] = result.Succeeded
                ? "Cập nhật thông tin thành công!"
                : "Cập nhật thất bại: " + string.Join(", ", result.Errors);

            return RedirectToAction("UpdateInfo");
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

            return View(user);
        }
    }

}
