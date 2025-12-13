using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Nutriplate.Web.ViewModels;

namespace Nutriplate.Web.Controllers
{
    [Authorize(Roles = "Dietitian")]
    public class DietitianController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public DietitianController(IHttpClientFactory httpClientFactory,
                                   IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        // Ortak: JWT ile HttpClient oluştur
        private HttpClient CreateClientWithJwt()
        {
            var client = _httpClientFactory.CreateClient();

            var baseUrl = _configuration["BackendApiBaseUrl"];
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new InvalidOperationException("BackendApiBaseUrl appsettings içinde yok.");
            }

            client.BaseAddress = new Uri(baseUrl);

            var token = User.Claims.FirstOrDefault(c => c.Type == "JwtToken")?.Value;
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            return client;
        }

        // ------------------------------------------------------
        //  /Dietitian  → Danışan listesi
        // ------------------------------------------------------
        public async Task<IActionResult> Index()
        {
            using var client = CreateClientWithJwt();

            var response = await client.GetAsync("/api/dietitian/clients");
            if (!response.IsSuccessStatusCode)
            {
                // Bir hata olursa basit mesaj gösterelim
                ViewBag.Error = "Danışan listesi alınırken bir hata oluştu.";
                return View(new List<DietitianClientViewModel>());
            }

            // JSON: [{ id, full_name, email, created_at }]
            var rawList = await response.Content.ReadFromJsonAsync<List<ClientDto>>()
                          ?? new List<ClientDto>();

            var viewModel = rawList.Select(x => new DietitianClientViewModel
            {
                Id = x.id,
                Name = x.full_name,
                Email = x.email
            }).ToList();

            return View(viewModel);
        }

        // ------------------------------------------------------
        //  /Dietitian/ClientSummary/5 → belirli danışanın günlük özeti
        // ------------------------------------------------------
        public async Task<IActionResult> ClientSummary(int id)
        {
            using var client = CreateClientWithJwt();

            var response = await client.GetAsync($"/api/dietitian/client/{id}/daily-summary");
            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Danışanın günlük özeti alınırken bir hata oluştu.";
                return View(new List<ClientDailySummaryViewModel>());
            }

            // JSON: [{ summary_date, total_calories }, ...]
            var rawList = await response.Content
                .ReadFromJsonAsync<List<ClientDailySummaryDto>>()
                ?? new List<ClientDailySummaryDto>();

            var viewModel = rawList.Select(x => new ClientDailySummaryViewModel
            {
                SummaryDate = x.summary_date,
                TotalCalories = x.total_calories
            }).ToList();

            ViewBag.ClientId = id; // başlıkta göstermek için
            return View(viewModel);
        }

        // ==== DTO sınıfları (sadece bu controller içinde kullanıyoruz) ====

        private class ClientDto
        {
            public int id { get; set; }
            public string full_name { get; set; } = string.Empty;
            public string email { get; set; } = string.Empty;
        }

        private class ClientDailySummaryDto
        {
            public DateTime summary_date { get; set; }
            public int total_calories { get; set; }
        }
    }
}
