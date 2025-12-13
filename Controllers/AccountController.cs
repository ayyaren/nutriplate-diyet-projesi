using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nutriplate.Web.Services;
using Nutriplate.Web.ViewModels;

namespace Nutriplate.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        // ----------------- LOGIN -----------------

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            var model = new LoginViewModel
            {
                ReturnUrl = returnUrl
            };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var authResult = await _authService.LoginAsync(model.Email, model.Password);

                if (authResult == null)
                {
                    TempData["Error"] = "E-posta veya şifre hatalı ya da sunucuya erişilemiyor.";
                    return View(model);
                }

                // Claims
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, authResult.UserId),
                    new Claim(ClaimTypes.Name, authResult.Name),
                    new Claim(ClaimTypes.Email, authResult.Email),
                    new Claim(ClaimTypes.Role, authResult.Role), // "Admin" / "Dietitian" / "User"
                    new Claim("JwtToken", authResult.Token)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal);

                // 1) Geçerli bir ReturnUrl varsa oraya git
                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }

                // 2) Rol'e göre yönlendirme
                return authResult.Role switch
                {
                    "Dietitian" => RedirectToAction("Index", "Dietitian"),
                    "Admin" => RedirectToAction("Index", "Dashboard"),
                    _ => RedirectToAction("Index", "Dashboard")
                };
            }
            catch
            {
                TempData["Error"] = "Giriş sırasında bir hata oluştu. Daha sonra tekrar deneyin.";
                return View(model);
            }
        }

        // ----------------- USER REGISTER -----------------

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var success = await _authService.RegisterAsync(model.Email, model.Password, model.Name);

                if (!success)
                {
                    TempData["Error"] = "Kayıt sırasında bir hata oluştu (e-posta zaten kayıtlı olabilir).";
                    return View(model);
                }

                TempData["Success"] = "Kayıt başarılı. Şimdi giriş yapabilirsiniz.";
                return RedirectToAction("Login");
            }
            catch
            {
                TempData["Error"] = "Sunucuya erişilemiyor. Biraz sonra tekrar deneyin.";
                return View(model);
            }
        }

        // ----------------- DİYETİSYEN REGISTER -----------------

        [HttpGet]
        [AllowAnonymous]
        public IActionResult DietitianRegister()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DietitianRegister(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var success = await _authService.DietitianRegisterAsync(model.Email, model.Password, model.Name);

                if (!success)
                {
                    TempData["Error"] = "Diyetisyen kaydı sırasında bir hata oluştu.";
                    return View(model);
                }

                TempData["Success"] = "Diyetisyen kaydı başarılı. Şimdi giriş yapabilirsiniz.";
                return RedirectToAction("Login");
            }
            catch
            {
                TempData["Error"] = "Sunucuya erişilemiyor. Biraz sonra tekrar deneyin.";
                return View(model);
            }
        }

        // ----------------- LOGOUT -----------------

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
