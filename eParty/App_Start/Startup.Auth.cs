using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eParty.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.Cookies;
using Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Google; // <-- ĐÃ THÊM: Cần thiết cho Google Auth

namespace eParty
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            // Cấu hình DbContext, User Manager và Sign-in Manager để sử dụng một instance duy nhất cho mỗi request
            app.CreatePerOwinContext(AppDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Cho phép ứng dụng sử dụng cookie để lưu trữ thông tin cho người dùng đã đăng nhập
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // CẤU HÌNH GOOGLE AUTHENTICATION
            // Đã sử dụng Client ID và Client Secret thực tế được xác nhận trước đó.
            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions
            {
                ClientId = "5256981425-brbfsc635e1d5e9s7757pvrrukvaoqi0.apps.googleusercontent.com",
                ClientSecret = "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"
            });
        }
    }
}