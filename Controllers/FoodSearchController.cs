using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nutriplate.Web.Services;
using System.Threading.Tasks;

namespace Nutriplate.Web.Controllers
{
    [Authorize]
    public class FoodSearchController : Controller
    {
        private readonly OpenFoodFactsService _foodService;

        public FoodSearchController(OpenFoodFactsService foodService)
        {
            _foodService = foodService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string barcode)
        {
            if (string.IsNullOrWhiteSpace(barcode))
                return View();

            var product = await _foodService.GetProductByBarcodeAsync(barcode);

            ViewBag.Product = product;
            ViewBag.Barcode = barcode;

            return View();
        }
    }
}
