using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Nutriplate.Web.ViewModels;

namespace Nutriplate.Web.Services
{
    public class ProfileService : IProfileService
    {
        private readonly HttpClient _httpClient;

        public ProfileService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            if (_httpClient.BaseAddress == null)
                _httpClient.BaseAddress = new Uri("http://localhost:3000");
        }

        // ------------------ GET /api/profile ------------------
        public async Task<ProfileViewModel?> GetProfileAsync(string jwtToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/profile");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return null;

            var dto = await response.Content.ReadFromJsonAsync<ProfileDto>();
            if (dto == null)
                return null;

            return new ProfileViewModel
            {
                Name = dto.name ?? string.Empty,
                Email = dto.email ?? string.Empty,
                BirthDate = dto.birthDate,
                Gender = dto.gender,
                ActivityLevel = dto.activityLevel,
                HeightCm = dto.heightCm,
                WeightKg = dto.weightKg,
                DailyCalorieTarget = dto.dailyCalorieTarget
            };
        }

        // ------------------ PUT /api/profile ------------------
        public async Task<bool> UpdateProfileAsync(ProfileViewModel model, string jwtToken)
        {
            var body = new
            {
                name = model.Name,
                birthDate = model.BirthDate?.ToString("yyyy-MM-dd"),
                gender = model.Gender,
                activityLevel = model.ActivityLevel,
                heightCm = model.HeightCm,
                weightKg = model.WeightKg,
                dailyCalorieTarget = model.DailyCalorieTarget
            };

            var request = new HttpRequestMessage(HttpMethod.Put, "/api/profile")
            {
                Content = JsonContent.Create(body)
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        // ----------- DTO: backend camelCase döndürüyor -----------
        private sealed class ProfileDto
        {
            public string? name { get; set; }
            public string? email { get; set; }
            public DateTime? birthDate { get; set; }
            public string? gender { get; set; }
            public string? activityLevel { get; set; }
            public int? heightCm { get; set; }
            public double? weightKg { get; set; }
            public int? dailyCalorieTarget { get; set; }
        }
    }
}
