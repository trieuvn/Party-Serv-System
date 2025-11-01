using eParty.Models;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace eParty.Areas.Admin.Controllers
{
    public class UserPermissionController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private AppDbContext db = new AppDbContext();

        public UserPermissionController() { }

        public UserPermissionController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        [HttpGet]
        public ActionResult RegisterRole()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisterRole(RegisterViewModel model, List<String> Role)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            var result = await UserManager.CreateAsync(user, model.Password);

            SystemUser systemUser = new SystemUser();

            if (result.Succeeded)
            {
                for (int i = 0; i < Role.Count; i++)
                {
                    await UserManager.AddToRoleAsync(user.Id, Role[i]);

                    systemUser.Role = Role[i];

                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                }
                systemUser.Username = user.Email;

                systemUser.Password = "123456";

                systemUser.Email = user.Email;

                db.SystemUsers.Add(systemUser);
                db.SaveChanges();
                return RedirectToAction("Index", "Dashboard");
            }


            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }

            return View(model);
        }

    }
}
