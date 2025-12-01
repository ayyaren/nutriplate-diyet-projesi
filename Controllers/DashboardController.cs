using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nutriplate.Web.ViewModels;

namespace Nutriplate.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        [HttpGet]
        public IActionResult Index(DateTime? date)
        {
            // Tarih seçilmemişse bugünü kullan
            var targetDate = date ?? DateTime.Today;

            // Şimdilik DUMMY (örnek) veri; ileride Ceren'in /api/dashboard/daily endpoint'inden gelecek.
            var model = new DailySummaryViewModel
            {
                Date = targetDate,

                // Kahvaltı (450) + Öğle (650) + Akşam (700) + Atıştırmalık (200) = 2000
                TotalKcal = 2000,
                TargetKcal = 2000,

                ProteinGr = 90,
                CarbGr = 200,
                FatGr = 60,

                Meals = new List<MealListItemViewModel>
                {
                    new MealListItemViewModel
                    {
                        Id = 1,
                        MealType = "Kahvaltı",
                        MealDateTime = targetDate.AddHours(8),
                        TotalKcal = 450
                    },
                    new MealListItemViewModel
                    {
                        Id = 2,
                        MealType = "Öğle",
                        MealDateTime = targetDate.AddHours(13),
                        TotalKcal = 650
                    },
                    new MealListItemViewModel
                    {
                        Id = 3,
                        MealType = "Akşam",
                        MealDateTime = targetDate.AddHours(19),
                        TotalKcal = 700
                    },
                    // ✅ YENİ: Atıştırmalık
                    new MealListItemViewModel
                    {
                        Id = 4,
                        MealType = "Atıştırmalık",
                        MealDateTime = targetDate.AddHours(16), // örnek saat
                        TotalKcal = 200
                    }
                }
            };

            return View(model);
        }
    }
}
