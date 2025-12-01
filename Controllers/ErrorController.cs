using Microsoft.AspNetCore.Mvc;

namespace Nutriplate.Web.Controllers
{
    public class ErrorController : Controller
    {
        // 500 ve genel hatalar
        [Route("Error/Error")]
        public IActionResult Error()
        {
            return View("Error");
        }

        // Status code'lar (404, 403 vs.) buraya düşecek
        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            return statusCode switch
            {
                404 => View("NotFound"),
                403 => View("AccessDenied"),
                _ => View("Error")
            };
        }
    }
}
