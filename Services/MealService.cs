using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Nutriplate.Web.ViewModels;

namespace Nutriplate.Web.Services
{
    public class MealService : IMealService
    {
        private readonly HttpClient _httpClient;

        public MealService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;

            var baseUrl = configuration["BackendApiBaseUrl"];
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new InvalidOperationException("BackendApiBaseUrl appsettings içinde tanımlı değil.");

            // Örn: http://localhost:3000
            _httpClient.BaseAddress = new Uri(baseUrl);
        }

        private void AddAuthHeader(string jwtToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", jwtToken);
        }

        // ---------------------------------------------------------
        // ORTAK: JSON'dan MealListItemViewModel listesi üret
        // ---------------------------------------------------------
        private static IReadOnlyList<MealListItemViewModel> ParseMealList(string json)
        {
            var list = new List<MealListItemViewModel>();

            if (string.IsNullOrWhiteSpace(json))
                return list;

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.ValueKind != JsonValueKind.Array)
                return list;

            foreach (var el in root.EnumerateArray())
            {
                try
                {
                    var vm = new MealListItemViewModel
                    {
                        Id = el.TryGetProperty("id", out var idEl) ? idEl.GetInt32() : 0,
                        MealType = el.TryGetProperty("meal_type", out var mtEl)
                            ? mtEl.GetString() ?? string.Empty
                            : string.Empty,
                        MealDateTime = el.TryGetProperty("meal_datetime", out var dtEl)
                            ? dtEl.GetDateTime()
                            : DateTime.MinValue,
                        TotalKcal = el.TryGetProperty("total_calories", out var tcEl)
                            ? tcEl.GetInt32()
                            : 0
                    };

                    list.Add(vm);
                }
                catch
                {
                    // tek tek eleman patlarsa bile diğerlerini doldurmaya devam et
                }
            }

            return list;
        }

        // ----------------- LİSTE (TÜM KULLANICI ÖĞÜNLERİ) -----------------
        // GET /api/meals/user/:id
        public async Task<IReadOnlyList<MealListItemViewModel>> GetMealsForUserAsync(int userId, string jwtToken)
        {
            AddAuthHeader(jwtToken);

            try
            {
                var response = await _httpClient.GetAsync($"/api/meals/user/{userId}");
                if (!response.IsSuccessStatusCode)
                    return new List<MealListItemViewModel>();

                var json = await response.Content.ReadAsStringAsync();
                return ParseMealList(json);
            }
            catch
            {
                return new List<MealListItemViewModel>();
            }
        }

        // ----------------- LİSTE (SEÇİLİ GÜN) -----------------
        // GET /api/meals/day?date=YYYY-MM-DD
        public async Task<IReadOnlyList<MealListItemViewModel>> GetMealsForDayAsync(DateTime date, string jwtToken)
        {
            AddAuthHeader(jwtToken);

            var dateStr = date.ToString("yyyy-MM-dd");

            try
            {
                var response = await _httpClient.GetAsync($"/api/meals/day?date={dateStr}");
                if (!response.IsSuccessStatusCode)
                    return new List<MealListItemViewModel>();

                var json = await response.Content.ReadAsStringAsync();
                return ParseMealList(json);
            }
            catch
            {
                return new List<MealListItemViewModel>();
            }
        }

        // ----------------- GÜNLÜK ÖZET (SEÇİLİ GÜN) -----------------
        // GET /api/meals/day-summary?date=YYYY-MM-DD
        public async Task<DailySummaryViewModel?> GetDailySummaryForDayAsync(DateTime date, string jwtToken)
        {
            AddAuthHeader(jwtToken);

            var dateStr = date.ToString("yyyy-MM-dd");

            try
            {
                var response = await _httpClient.GetAsync($"/api/meals/day-summary?date={dateStr}");
                if (!response.IsSuccessStatusCode)
                    return null;

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                // API cevabı: { "date": "2025-12-04", "totalCalories": 65 }
                var total = root.TryGetProperty("totalCalories", out var tcEl)
                    ? tcEl.GetInt32()
                    : 0;

                var vm = new DailySummaryViewModel
                {
                    Date = date
                };

                // ViewModel içinde hangi property varsa onu doldur (TotalKcal veya TotalCalories)
                var t = typeof(DailySummaryViewModel);
                var propTotalKcal = t.GetProperty("TotalKcal");
                var propTotalCalories = t.GetProperty("TotalCalories");

                if (propTotalKcal != null && propTotalKcal.CanWrite)
                    propTotalKcal.SetValue(vm, total);
                if (propTotalCalories != null && propTotalCalories.CanWrite)
                    propTotalCalories.SetValue(vm, total);

                return vm;
            }
            catch
            {
                return null;
            }
        }

        // ----------------- DETAY -----------------
        // GET /api/meals/:mealId
        public async Task<MealDetailViewModel?> GetMealAsync(int mealId, string jwtToken)
        {
            AddAuthHeader(jwtToken);

            try
            {
                var response = await _httpClient.GetAsync($"/api/meals/{mealId}");
                if (!response.IsSuccessStatusCode)
                    return null;

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                var detail = new MealDetailViewModel
                {
                    Id = root.TryGetProperty("id", out var idEl) ? idEl.GetInt32() : 0,
                    MealType = root.TryGetProperty("meal_type", out var mtEl)
                        ? mtEl.GetString() ?? string.Empty
                        : string.Empty,
                    MealDateTime = root.TryGetProperty("meal_datetime", out var dtEl)
                        ? dtEl.GetDateTime()
                        : DateTime.MinValue,
                    TotalKcal = root.TryGetProperty("total_calories", out var tcEl)
                        ? tcEl.GetInt32()
                        : 0,
                    Notes = root.TryGetProperty("note", out var noteEl)
                        ? noteEl.GetString()
                        : null,
                    Items = new List<MealFoodItemViewModel>()
                };

                if (root.TryGetProperty("items", out var itemsEl) &&
                    itemsEl.ValueKind == JsonValueKind.Array)
                {
                    foreach (var it in itemsEl.EnumerateArray())
                    {
                        try
                        {
                            var itemVm = new MealFoodItemViewModel
                            {
                                FoodName = it.TryGetProperty("foodName", out var fnEl)
                                    ? fnEl.GetString() ?? string.Empty
                                    : string.Empty,
                                Gram = it.TryGetProperty("gram", out var gEl)
                                    ? gEl.GetInt32()
                                    : 0,
                                Kcal = it.TryGetProperty("kcal", out var kEl)
                                    ? kEl.GetInt32()
                                    : 0
                            };

                            detail.Items.Add(itemVm);
                        }
                        catch
                        {
                            // tek satır patlasa bile diğerlerini okumaya devam et
                        }
                    }
                }

                return detail;
            }
            catch
            {
                return null;
            }
        }

        // ----------------- OLUŞTUR -----------------
        // POST /api/meals
        public async Task<bool> CreateMealAsync(MealCreateViewModel model, string jwtToken)
        {
            AddAuthHeader(jwtToken);

            // Toplam kaloriyi hesapla (kullanıcı girmediyse satırlardan topla)
            var totalFromItems = (model.Items ?? new List<MealFoodItemViewModel>())
                .Sum(i => i.Kcal);

            var payload = new
            {
                meal_datetime = model.MealDateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                meal_type = model.MealType,   // Artık enum değil, doğrudan string
                total_calories = model.TotalKcal ?? totalFromItems,
                note = model.Notes,
                items = (model.Items ?? new List<MealFoodItemViewModel>())
                    .Where(i => !string.IsNullOrWhiteSpace(i.FoodName))
                    .Select(i => new
                    {
                        foodName = i.FoodName,
                        gram = i.Gram,
                        kcal = i.Kcal
                    })
                    .ToList()
            };

            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/meals", payload);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        // ----------------- SİL -----------------
        // DELETE /api/meals/:mealId
        public async Task<bool> DeleteMealAsync(int mealId, string jwtToken)
        {
            AddAuthHeader(jwtToken);

            try
            {
                var response = await _httpClient.DeleteAsync($"/api/meals/{mealId}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
