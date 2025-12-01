using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nutriplate.Web.ViewModels;

namespace Nutriplate.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            // Şimdilik dummy kullanıcı listesi
            var users = new List<AdminUserViewModel>
            {
                new AdminUserViewModel
                {
                    Id = 1,
                    Name = "Yaren Admin",
                    Email = "yaren@example.com",
                    Role = "Admin"
                },
                new AdminUserViewModel
                {
                    Id = 2,
                    Name = "Ceren Kullanıcı",
                    Email = "ceren@example.com",
                    Role = "User"
                },
                new AdminUserViewModel
                {
                    Id = 3,
                    Name = "Bersu Diyetisyen",
                    Email = "bersu@example.com",
                    Role = "Dietitian"
                }
            };

            return View(users);
        }
    }
}
