using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nutriplate.Web.Services;
using Nutriplate.Web.ViewModels;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Nutriplate.Web.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        // -----------------------------------------------------
        // GET: /Profile/Edit
        //  -> JWT'yi cookie'den al
        //  -> Node.js /api/profile'dan gerçek veriyi oku
        // -----------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var token = GetJwtTokenFromClaims();
            if (string.IsNullOrEmpty(token))
            {
                // Kullanıcı login değilse tekrar login'e gönder
                return RedirectToAction("Login", "Account");
            }

            var model = await _profileService.GetProfileAsync(token);

            // Eğer backend henüz boş ise, en azından email'i dolduralım
            if (model == null)
            {
                model = new ProfileViewModel
                {
                    Name = User.Identity?.Name ?? string.Empty,
                    Email = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty
                };
            }

            if (TempData["Success"] != null)
            {
                ViewBag.Success = TempData["Success"];
            }

            return View(model);
        }

        // -----------------------------------------------------
        // POST: /Profile/Edit
        //  -> ModelState kontrolü
        //  -> Node.js /api/profile'a PUT at
        // -----------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProfileViewModel model)
        {
            var token = GetJwtTokenFromClaims();
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var success = await _profileService.UpdateProfileAsync(model, token);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, "Profil güncellenirken bir hata oluştu.");
                return View(model);
            }

            TempData["Success"] = "Profil bilgileriniz başarıyla güncellendi.";
            return RedirectToAction("Edit");
        }

        // -----------------------------------------------------
        // JWT'yi cookie içindeki claim'lerden okuyan helper
        // -----------------------------------------------------
        private string? GetJwtTokenFromClaims()
        {
            // Login olurken hangi claim'e yazdıysan önce onu dene
            return User.FindFirst("JwtToken")?.Value
                   ?? User.FindFirst("AccessToken")?.Value;
        }
    }
}
