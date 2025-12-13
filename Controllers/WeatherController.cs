using Microsoft.AspNetCore.Mvc;
using Nutriplate.Web.Services;

namespace Nutriplate.Web.Controllers
{
    public class WeatherController : Controller
    {
        private readonly IWeatherService _weatherService;

        public WeatherController(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        // /Weather/Index
        public async Task<IActionResult> Index()
        {
            // Örnek olsun diye: 20°C’yi dış servise soruyoruz
            var fahrenheit = await _weatherService.GetWeatherAsync("20");

            return View(model: fahrenheit);
        }
    }
}
