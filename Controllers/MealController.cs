using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Nutriplate.Web.ViewModels;
// using Microsoft.AspNetCore.Authorization;

namespace Nutriplate.Web.Controllers
{
    // Ceren login akışını bitirdiğinde bunu açacağız:
    // [Authorize]
    public class MealController : Controller
    {
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
                }
            };

            return View(meals);
        }

        public IActionResult Details(int id)
        {
            // Şimdilik sadece id'yi gösteren basit bir sayfa
            ViewData["MealId"] = id;
            return View();
        }

        // ---------- YENİ: ÖĞÜN OLUŞTUR (CREATE) ----------

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

            // TODO: Burada Ceren'in Node.js API'sine istek atılacak.
            TempData["Success"] = "Öğün başarıyla kaydedildi (şimdilik sahte kayıt).";
            return RedirectToAction("Index");
        }

        // ---------- YENİ: FOTOĞRAFTAN ÖĞÜN ANALİZİ (UPLOAD PHOTO) ----------

        [HttpGet]
        public IActionResult UploadPhoto()
        {
            var model = new MealPhotoUploadViewModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult UploadPhoto(MealPhotoUploadViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // TODO: Burada fotoğrafı gerçek ML servisine göndereceğiz.
            // Şimdilik SAHTE analiz sonucu gösteriyoruz.

            model.AnalysisResults = new List<MealPhotoAnalysisItem>
            {
                new MealPhotoAnalysisItem { FoodName = "Izgara Tavuk", Gram = 150, Kcal = 250 },
                new MealPhotoAnalysisItem { FoodName = "Pirinç Pilavı", Gram = 120, Kcal = 200 },
                new MealPhotoAnalysisItem { FoodName = "Salata", Gram = 80, Kcal = 50 }
            };

            TempData["Success"] = "Fotoğraf yüklendi ve örnek analiz gösteriliyor. Daha sonra ML servisi ile gerçek analiz yapılacak.";

            return View(model);
        }
    }
}
