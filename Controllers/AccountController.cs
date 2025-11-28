using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            var model = new LoginViewModel
            {
                ReturnUrl = returnUrl
            };
            return View(model);
        }

        [HttpPost]
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
                    new Claim(ClaimTypes.Email, model.Email),
                    new Claim(ClaimTypes.Role, authResult.Role),
                    new Claim("JwtToken", authResult.Token)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal);

                // Eğer geçerli bir ReturnUrl varsa önce oraya git
                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }

                // Aksi halde doğrudan ÖĞÜNLERİM sayfasına yönlendir
                return RedirectToAction("Index", "Meal");
            }
            catch (Exception)
            {
                TempData["Error"] = "Giriş sırasında bir hata oluştu. Daha sonra tekrar deneyin.";
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var success = await _authService.RegisterAsync(model.Email, model.Password, model.Name);

                if (!success)
                {
                    TempData["Error"] = "Kayıt sırasında bir hata oluştu.";
                    return View(model);
                }

                TempData["Success"] = "Kayıt başarılı. Şimdi giriş yapabilirsiniz.";
                return RedirectToAction("Login");
            }
            catch (Exception)
            {
                TempData["Error"] = "Sunucuya erişilemiyor. Biraz sonra tekrar deneyin.";
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
