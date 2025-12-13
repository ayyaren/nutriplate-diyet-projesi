using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Nutriplate.Web.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;

            var baseUrl = configuration["BackendApiBaseUrl"];
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new InvalidOperationException("BackendApiBaseUrl appsettings içinde tanımlı değil.");

            _httpClient.BaseAddress = new Uri(baseUrl); // http://localhost:3000
        }

        
        public async Task<bool> RegisterAsync(string email, string password, string name)
        {
            var payload = new
            {
                full_name = name,
                email = email,
                password = password
            };

            var response = await _httpClient.PostAsJsonAsync("/api/auth/register", payload);

            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                // Email zaten kayıtlı
                return false;
            }

            return response.IsSuccessStatusCode;
        }

       
        public async Task<bool> DietitianRegisterAsync(string email, string password, string name)
        {
            var payload = new
            {
                full_name = name,
                email = email,
                password = password
            };

            HttpResponseMessage response;

            try
            {
                response = await _httpClient.PostAsJsonAsync("/api/auth/dietitian-register", payload);
            }
            catch
            {
                return false;
            }

            return response.IsSuccessStatusCode;
        }

       
        public async Task<AuthResult?> LoginAsync(string email, string password)
        {
            var payload = new
            {
                email,
                password
            };

            var response = await _httpClient.PostAsJsonAsync("/api/auth/login", payload);

            if (!response.IsSuccessStatusCode)
                return null;

            var dto = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
            if (dto == null)
                return null;

            string roleName = dto.role switch
            {
                1 => "Admin",
                2 => "Dietitian",
                3 => "User",
                _ => "User"
            };

            return new AuthResult
            {
                Token = dto.token,
                UserId = dto.userId.ToString(),
                Name = dto.name,
                Email = dto.email,
                Role = roleName   
            };
        }

        
        private class LoginResponseDto
        {
            public string token { get; set; } = "";
            public int userId { get; set; }
            public string name { get; set; } = "";
            public string email { get; set; } = "";
            public int role { get; set; }
        }
    }
}
