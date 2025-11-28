using Microsoft.AspNetCore.Mvc;
using Nutriplate.Web.ViewModels;
// using Microsoft.AspNetCore.Authorization;

namespace Nutriplate.Web.Controllers
{
    // [Authorize]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            // Şimdilik dummy veri
            var model = new DailySummaryViewModel
            {
                Date = DateTime.Today,
                TotalKcal = 1800,
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
                }
            };

            return View(model);
        }
    }
}
