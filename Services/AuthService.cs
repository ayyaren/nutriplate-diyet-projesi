using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Nutriplate.Web.Services
{
    // BURADA da IAuthService'i implemente ediyoruz
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public AuthService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;

            var baseUrl = _configuration["AuthApi:BaseUrl"];
            if (!string.IsNullOrEmpty(baseUrl))
            {
                _httpClient.BaseAddress = new Uri(baseUrl);
            }
        }

        // 🚨 SADECE GELİŞTİRME / TEST İÇİN 🚨
        // Şu an backend hazır olmadığı için,
        // gelen email/şifreye bakmadan, seni Admin olarak login sayıyoruz.
        public async Task<AuthResult?> LoginAsync(string email, string password)
        {
            return await Task.FromResult(new AuthResult
            {
                Token = "dummy-token",
                UserId = "1",
                Name = "Yaren (Admin Test)",
                Role = "Admin" // Burayı "Dietitian" veya "User" yaparak diğer rolleri de test edebilirsin.
            });
        }

        public async Task<bool> RegisterAsync(string email, string password, string name)
        {
            // 🚨 Backend hazır olana kadar SAHTE kayıt işlemi
            // Kayıt işlemini her zaman başarılı kabul ediyoruz.
            return await Task.FromResult(true);
        }
    }
}
