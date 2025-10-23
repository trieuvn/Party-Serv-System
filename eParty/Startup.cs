// Trong file Startup.cs

using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(eParty.Startup))]

namespace eParty
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // DÒNG NÀY CHỈ NÊN LÀ DÒNG CODE DUY NHẤT VÀ KHÔNG ĐƯỢC CHỨA CẤU HÌNH AUTH KHÁC
            ConfigureAuth(app);
        }
    }
}