using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nutriplate.Web.ViewModels;

namespace Nutriplate.Web.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        [HttpGet]
        public IActionResult Edit()
        {
            // Şimdilik dummy veri; ileride Ceren'in API'sinden gelecek.
            var model = new ProfileViewModel
            {
                Name = User.Identity?.Name ?? "Bilinmeyen Kullanıcı",
                Email = "yaren@example.com", // Gerçekte backend'den gelmeli
                BirthDate = new DateTime(2000, 1, 1),
                Gender = "Kadın",
                HeightCm = 165,
                WeightKg = 60,
                ActivityLevel = "Orta",
                DailyCalorieTarget = 2000
            };

            if (TempData["Success"] != null)
            {
                ViewBag.Success = TempData["Success"];
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // TODO: Burada Ceren'in Profile API'sine (PUT /api/users/me) istek atılacak.
            // Şimdilik sadece başarılı kabul ediyoruz.
            TempData["Success"] = "Profil bilgileriniz güncellendi (şimdilik sahte).";

            return RedirectToAction("Edit");
        }
    }
}
