namespace Nutriplate.Web.Services
{
    public interface IWeatherService
    {
        // Örnek:  "20" gönderirsek 20°C için Fahrenheit değeri dönecek
        Task<string> GetWeatherAsync(string celsius);
    }
}
