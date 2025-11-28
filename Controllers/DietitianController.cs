using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Nutriplate.Web.Controllers
{
    [Authorize(Roles = "Dietitian")]
    public class DietitianController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
