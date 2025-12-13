using Microsoft.AspNetCore.Authentication.Cookies;
using Nutriplate.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// -------------------------
// SERVICES
// -------------------------

// MVC
builder.Services.AddControllersWithViews();

//
// Node.js backend (http://localhost:3000) ile konuþan ApiClient
//
builder.Services.AddHttpClient<ApiClient>();

//
// AuthService (AccountController için IAuthService)
//
builder.Services.AddHttpClient<IAuthService, AuthService>();

//
// MealService (MealController için IMealService)
//
builder.Services.AddHttpClient<IMealService, MealService>();



builder.Services.AddHttpClient<OpenFoodFactsService>();


//
// ProfileService (ProfileController için IProfileService)
//
builder.Services.AddHttpClient<IProfileService, ProfileService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:3000");
});

//
// WeatherService (WeatherController için IWeatherService) – SOAP
// BaseAddress vermek þart deðil, servisin içinde tam URL ile çaðýracaðýz.
//
builder.Services.AddHttpClient<IWeatherService, WeatherSoapService>();

//

// Cookie tabanlý kimlik doðrulama ayarlarý
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";               // Giriþ yapýlmamýþsa buraya yönlendir
        options.AccessDeniedPath = "/Account/AccessDenied"; // Rol yetmezse buraya yönlendir
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

// Rol bazlý yetkilendirme
builder.Services.AddAuthorization();

var app = builder.Build();

// -------------------------
// HTTP REQUEST PIPELINE
// -------------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error/Error");
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Error/{0}");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ÖNCE Authentication, sonra Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
