using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nutriplate.Web.Models;
using Nutriplate.Web.Services;

namespace Nutriplate.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApiClient _apiClient;

        public HomeController(ILogger<HomeController> logger, ApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }

        public async Task<IActionResult> Index()
        {
            // Backend ? Node.js API çaðrýsý (þimdilik hata alsa bile derlenir)
            string mealsJson;

            try
            {
                mealsJson = await _apiClient.GetMealsAsync();
            }
            catch (Exception ex)
            {
                // Logla, ama uygulamayý patlatma
                _logger.LogError(ex, "GetMealsAsync sýrasýnda hata oluþtu.");
                mealsJson = "[]";
            }

            ViewBag.Meals = mealsJson;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
