using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Nutriplate.Web.Services
{
    public class OpenFoodFactsService
    {
        private readonly HttpClient _httpClient;

        public OpenFoodFactsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress ??= new Uri("https://world.openfoodfacts.org/");
        }

        public async Task<FoodProduct?> GetProductByBarcodeAsync(string barcode)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/v0/product/{barcode}.json");

                if (!response.IsSuccessStatusCode)
                    return null;

                var stream = await response.Content.ReadAsStreamAsync();

                var apiResponse = await JsonSerializer.DeserializeAsync<OpenFoodFactsResponse>(
                    stream,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return apiResponse?.Product;
            }
            catch
            {
                return null;
            }
        }
    }

    // ---- DTO'lar ----

    public class OpenFoodFactsResponse
    {
        [JsonPropertyName("product")]
        public FoodProduct? Product { get; set; }
    }

    public class FoodProduct
    {
        [JsonPropertyName("product_name")]
        public string? Name { get; set; }

        [JsonPropertyName("image_url")]
        public string? ImageUrl { get; set; }

        [JsonPropertyName("nutriments")]
        public Nutriments? Nutriments { get; set; }
    }

    public class Nutriments
    {
        [JsonPropertyName("energy-kcal_100g")]
        public double? Calories { get; set; }
    }
}
