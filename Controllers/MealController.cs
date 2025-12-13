using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nutriplate.Web.Services;
using Nutriplate.Web.ViewModels;
using System.Security.Claims;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Nutriplate.Web.Controllers
{
    [Authorize] // İstersen Roles = "User,Dietitian" diye daraltabilirsin
    public class MealController : Controller
    {
        private readonly IMealService _mealService;

        public MealController(IMealService mealService)
        {
            _mealService = mealService;
        }

        // Yardımcı: claim'lerden userId + token çek
        private bool TryGetAuthInfo(out int userId, out string token)
        {
            userId = 0;
            token = string.Empty;

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var jwt = User.FindFirst("JwtToken")?.Value;

            if (string.IsNullOrEmpty(userIdStr) || string.IsNullOrEmpty(jwt))
                return false;

            if (!int.TryParse(userIdStr, out userId))
                return false;

            token = jwt;
            return true;
        }

        // ------------- LİSTE (Meals/Index) -------------
        // Artık seçili güne göre liste getiriyoruz. ?date=YYYY-MM-DD vermezsen bugün.
        [HttpGet]
        public async Task<IActionResult> Index(DateTime? date)
        {
            if (!TryGetAuthInfo(out var userId, out var token))
                return RedirectToAction("Login", "Account");

            var selectedDate = date ?? DateTime.Today;

            // Yeni servis methodu: sadece o güne ait öğünler
            var meals = await _mealService.GetMealsForDayAsync(selectedDate, token);

            // View'de kullanmak istersen diye:
            ViewBag.SelectedDate = selectedDate;

            return View(meals); // Views/Meal/Index.cshtml → @model IEnumerable<MealListItemViewModel>
        }

        // ------------- DETAY (Meals/Details) -------------
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (!TryGetAuthInfo(out _, out var token))
                return RedirectToAction("Login", "Account");

            var meal = await _mealService.GetMealAsync(id, token);
            if (meal == null)
            {
                // Login'de görünmesin diye farklı key
                TempData["MealError"] = "Öğün bulunamadı.";
                return RedirectToAction("Index");
            }

            return View(meal); // Views/Meal/Details.cshtml
        }

        // ------------- OLUŞTUR (manual ekleme + fromId ile kopya) -------------
        [HttpGet]
        public async Task<IActionResult> Create(int? fromId, bool? similar)
        {
            if (!TryGetAuthInfo(out _, out var token))
                return RedirectToAction("Login", "Account");

            MealCreateViewModel model;

            if (fromId.HasValue)
            {
                var existing = await _mealService.GetMealAsync(fromId.Value, token);

                if (existing == null)
                {
                    model = new MealCreateViewModel
                    {
                        MealDateTime = DateTime.Now
                    };
                }
                else
                {
                    model = new MealCreateViewModel
                    {
                        MealType = existing.MealType,
                        MealDateTime = DateTime.Now,
                        TotalKcal = existing.TotalKcal,
                        Notes = existing.Notes,
                        Items = existing.Items
                            .Select(x => new MealFoodItemViewModel
                            {
                                FoodName = x.FoodName,
                                Gram = x.Gram,
                                Kcal = x.Kcal
                            })
                            .ToList()
                    };

                    if (similar == true)
                    {
                        model.Notes = (model.Notes ?? string.Empty) + " (benzer öğün taslağı)";
                    }
                }
            }
            else
            {
                model = new MealCreateViewModel
                {
                    MealDateTime = DateTime.Now
                };
            }

            return View(model); // Views/Meal/Create.cshtml
        }

        // ------------- OLUŞTUR (POST) -------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MealCreateViewModel model)
        {
            // Kullanıcı / token bilgisi yoksa login'e gönder
            if (!TryGetAuthInfo(out var userId, out var token))
                return RedirectToAction("Login", "Account");

            // Backend'e kaydet
            var ok = await _mealService.CreateMealAsync(model, token);

            if (!ok)
            {
                TempData["MealError"] = "Öğün kaydedilirken bir hata oluştu.";
            }
            else
            {
                TempData["Success"] = "Öğün başarıyla kaydedildi.";
            }

            // Kaydedilen öğünün tarihine göre listeye dön
            var targetDate = model.MealDateTime.Date;
            return RedirectToAction("Index", new { date = targetDate });
        }

        // ------------- FOTOĞRAFTAN ÖĞÜN EKLE (GET) -------------
        [HttpGet]
        public IActionResult UploadPhoto()
        {
            // BOŞ ama null olmayan model
            var model = new MealPhotoUploadViewModel();
            return View(model); // Views/Meal/UploadPhoto.cshtml
        }

        // ------------- FOTOĞRAFTAN ÖĞÜN EKLE (POST) -------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadPhoto(MealPhotoUploadViewModel model)
        {
            if (!TryGetAuthInfo(out var userId, out var token))
                return RedirectToAction("Login", "Account");

            if (model.Photo == null || model.Photo.Length == 0)
            {
                ModelState.AddModelError("Photo", "Lütfen bir fotoğraf seçin.");
                return View(model);
            }

            // Şimdilik backend'e gerçekten göndermiyoruz; sahte örnek
            model.AnalysisResults = new System.Collections.Generic.List<MealFoodItemViewModel>
            {
                new MealFoodItemViewModel
                {
                    FoodName = "Örnek yiyecek",
                    Gram = 150,
                    Kcal = 320
                }
            };

            TempData["Success"] = "Fotoğraf yüklendi. (Örnek analiz sonuçları gösteriliyor)";
            return View(model); // Aynı sayfada sonuçları göster
        }

        // ------------- SİL -------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!TryGetAuthInfo(out _, out var token))
                return RedirectToAction("Login", "Account");

            var ok = await _mealService.DeleteMealAsync(id, token);

            if (!ok)
                TempData["MealError"] = "Öğün silinirken bir hata oluştu.";
            else
                TempData["Success"] = "Öğün silindi.";

            return RedirectToAction("Index");
        }
    }
}
