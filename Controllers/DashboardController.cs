using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nutriplate.Web.Services;
using Nutriplate.Web.ViewModels;

namespace Nutriplate.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IMealService _mealService;

        public DashboardController(IMealService mealService)
        {
            _mealService = mealService;
        }

        // Claim'lerden token'ı çekmek için helper (MealController ile aynı mantık)
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

        [HttpGet]
        public async Task<IActionResult> Index(DateTime? date)
        {
            if (!TryGetAuthInfo(out var userId, out var token))
                return RedirectToAction("Login", "Account");

            // Tarih seçilmemişse bugünü kullan
            var targetDate = date ?? DateTime.Today;

            // ✅ Backend'den, sadece o güne ait öğünleri çek
            var mealsForDay = await _mealService.GetMealsForDayAsync(targetDate, token);

            // Toplam kaloriyi öğünlerden hesapla
            var totalKcal = mealsForDay.Sum(m => m.TotalKcal);


            // İleride Profile'dan dailyCalorieTarget çekebilirsin, şimdilik sabit/dummy
            var model = new DailySummaryViewModel
            {
                Date = targetDate,
                TotalKcal = totalKcal,

                // TODO: Profile'dan günlük hedef alabilirsin
                TargetKcal = 2000,

                // Şimdilik makroları hesaplamıyoruz, ileride eklersin
                ProteinGr = 0,
                CarbGr = 0,
                FatGr = 0,

                Meals = mealsForDay.ToList()
            };

            return View(model);
        }
    }
}
