using Microsoft.AspNetCore.Mvc;
using Nutriplate.Web.ViewModels;
// using Microsoft.AspNetCore.Authorization;

namespace Nutriplate.Web.Controllers
{
    // [Authorize]
    public class ProfileController : Controller
    {
        [HttpGet]
        public IActionResult Edit()
        {
            // TODO: Ceren backend'den gerçek kullanıcıyı çekecek.
            // Şimdilik dummy veri:
            var model = new ProfileViewModel
            {
                Name = "Yaren Kullanıcı",
                Email = "yaren@example.com",
                BirthDate = new DateTime(2000, 1, 1),
                Gender = "Kadın",
                HeightCm = 165,
                WeightKg = 60,
                ActivityLevel = "Orta"
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // TODO: Burada backend'e kaydetme işlemi yapılacak.
            TempData["Success"] = "Profil bilgileriniz güncellendi (şimdilik örnek).";

            return RedirectToAction("Edit");
        }
    }
}
