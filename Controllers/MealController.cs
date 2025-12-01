using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Nutriplate.Web.ViewModels;
// using Microsoft.AspNetCore.Authorization;

namespace Nutriplate.Web.Controllers
{
    // Ceren login akışını bitirdiğinde açacağız:
    // [Authorize]
    public class MealController : Controller
    {
        // ---------- ÖĞÜN LİSTESİ (INDEX) ----------
        public IActionResult Index()
        {
            // Şimdilik SAHTE veri (dummy); ileride API'den gelecek.
            var meals = new List<MealListItemViewModel>
            {
                new MealListItemViewModel
                {
                    Id = 1,
                    MealType = "Kahvaltı",
                    MealDateTime = DateTime.Today.AddHours(8),
                    TotalKcal = 450
                },
                new MealListItemViewModel
                {
                    Id = 2,
                    MealType = "Öğle",
                    MealDateTime = DateTime.Today.AddHours(13),
                    TotalKcal = 650
                },
                new MealListItemViewModel
                {
                    Id = 3,
                    MealType = "Akşam",
                    MealDateTime = DateTime.Today.AddHours(19),
                    TotalKcal = 700
                },
                // ✅ YENİ: Atıştırmalık
                new MealListItemViewModel
                {
                    Id = 4,
                    MealType = "Atıştırmalık",
                    MealDateTime = DateTime.Today.AddHours(16), // örnek saat
                    TotalKcal = 200
                }
            };

            return View(meals);
        }

        // ---------- ÖĞÜN DETAYI (DETAILS) ----------
        public IActionResult Details(int id)
        {
            // Şimdilik örnek bir öğün detayı
            var model = new MealDetailViewModel
            {
                Id = id,
                MealType = "Akşam",
                MealDateTime = DateTime.Today.AddHours(19),
                TotalKcal = 700,
                Notes = "Bu öğün şimdilik örnek verilerle gösteriliyor.",
                Items = new List<MealFoodItemViewModel>
                {
                    new MealFoodItemViewModel { FoodName = "Izgara Tavuk",  Gram = 150, Kcal = 250 },
                    new MealFoodItemViewModel { FoodName = "Pirinç Pilavı", Gram = 120, Kcal = 200 },
                    new MealFoodItemViewModel { FoodName = "Mevsim Salata", Gram = 80,  Kcal = 50  }
                }
            };

            var suggestions = new[]
            {
                new { Original = "Pirinç Pilavı", Alternative = "Bulgur Pilavı", Reason = "Daha fazla lif, daha düşük glisemik indeks" },
                new { Original = "Kızartma",      Alternative = "Izgara / Fırın", Reason = "Daha az yağ, daha az kalori" },
                new { Original = "Gazlı İçecek",  Alternative = "Su / Ayran",     Reason = "Şeker alımını azaltır" }
            };

            ViewBag.Suggestions = suggestions;

            return View(model);
        }

        // ---------- ÖĞÜN OLUŞTUR (CREATE) ----------
        [HttpGet]
        public IActionResult Create()
        {
            var model = new MealCreateViewModel
            {
                MealDateTime = DateTime.Now
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Create(MealCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            TempData["Success"] = "Öğün başarıyla kaydedildi (şimdilik sadece sahte kayıt).";

            return RedirectToAction("Index");
        }

        // ---------- FOTOĞRAF YÜKLEME (UPLOAD PHOTO) ----------
        [HttpGet]
        public IActionResult UploadPhoto()
        {
            var model = new MealPhotoUploadViewModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult UploadPhoto(MealPhotoUploadViewModel model)
        {
            if (!ModelState.IsValid || model.Photo == null)
            {
                model.ErrorMessage = "Lütfen bir fotoğraf seçin.";
                return View(model);
            }

            // Şimdilik SAHTE analiz sonucu oluşturalım:
            model.AnalysisResults = new List<MealFoodItemViewModel>
            {
                new MealFoodItemViewModel { FoodName = "Izgara Tavuk",  Gram = 150, Kcal = 250 },
                new MealFoodItemViewModel { FoodName = "Pirinç Pilavı", Gram = 120, Kcal = 200 },
                new MealFoodItemViewModel { FoodName = "Salata",        Gram = 80,  Kcal = 50  }
            };

            TempData["Success"] = "Fotoğraf yüklendi ve örnek analiz gösteriliyor. Daha sonra ML servisi ile gerçek analiz yapılacak.";

            return View(model);
        }
    }
}
