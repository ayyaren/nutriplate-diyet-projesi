using System;
using Microsoft.AspNetCore.Mvc;
using Nutriplate.Web.ViewModels;

namespace Nutriplate.Web.ViewComponents
{
    public class DailySummaryViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            // Şimdilik dummy; ileride Dashboard API'den gelecek
            var model = new DailySummaryBoxViewModel
            {
                Date = DateTime.Today,
                TotalKcal = 1850,
                TargetKcal = 2000,
                ProteinGr = 80,
                CarbGr = 210,
                FatGr = 60
            };

            // İsim belirtmesek de olur ama garanti olsun:
            return View("Default", model);
        }
    }
}
