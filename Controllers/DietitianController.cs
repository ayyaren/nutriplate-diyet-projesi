using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nutriplate.Web.ViewModels;

namespace Nutriplate.Web.Controllers
{
    [Authorize(Roles = "Dietitian")]
    public class DietitianController : Controller
    {
        public IActionResult Index()
        {
            // Şimdilik dummy danışan listesi
            var clients = new List<DietitianClientViewModel>
            {
                new DietitianClientViewModel
                {
                    Id = 1,
                    Name = "Yaren Danışan",
                    Email = "yaren.danisan@example.com",
                    CurrentWeight = 60,
                    TargetWeight = 55,
                    LastUpdate = DateTime.Today.AddDays(-1)
                },
                new DietitianClientViewModel
                {
                    Id = 2,
                    Name = "Ceren Danışan",
                    Email = "ceren.danisan@example.com",
                    CurrentWeight = 70,
                    TargetWeight = 62,
                    LastUpdate = DateTime.Today.AddDays(-3)
                }
            };

            return View(clients);
        }
    }
}
