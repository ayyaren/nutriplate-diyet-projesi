using Microsoft.AspNetCore.Authentication.Cookies;
using Nutriplate.Web.Services; // IAuthService ve AuthService buradaysa

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Node.js Auth API'ye istek atmak için HttpClient
builder.Services.AddHttpClient();

// AuthService'i DI container'a ekle
builder.Services.AddScoped<IAuthService, AuthService>();

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

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

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
