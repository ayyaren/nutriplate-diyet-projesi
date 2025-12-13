using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Nutriplate.Web.Services
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;

        public ApiClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;

            var baseUrl = configuration["BackendApiBaseUrl"];
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new InvalidOperationException("BackendApiBaseUrl appsettings içinde tanımlı değil.");

            _httpClient.BaseAddress = new Uri(baseUrl);
        }

        // Şimdilik sadece test amaçlı basit bir metod:
        public async Task<string> GetMealsAsync()
        {
            // Not: server.js içindeki gerçek route neyse onu koyacağız.
            // Şimdilik örnek:
            var response = await _httpClient.GetAsync("/api/meals");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
