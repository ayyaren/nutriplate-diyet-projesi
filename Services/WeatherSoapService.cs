using System.Net.Http;
using System.Text;
using System.Xml.Linq;

namespace Nutriplate.Web.Services
{
    public class WeatherSoapService : IWeatherService
    {
        private readonly HttpClient _httpClient;

        public WeatherSoapService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetWeatherAsync(string celsius)
        {
            // 1) SOAP 1.1 request body (W3Schools dokümanındaki örnek)
            var soapEnvelope = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
               xmlns:xsd=""http://www.w3.org/2001/XMLSchema""
               xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
  <soap:Body>
    <CelsiusToFahrenheit xmlns=""https://www.w3schools.com/xml/"">
      <Celsius>{System.Security.SecurityElement.Escape(celsius)}</Celsius>
    </CelsiusToFahrenheit>
  </soap:Body>
</soap:Envelope>";

            var content = new StringContent(soapEnvelope, Encoding.UTF8, "text/xml");

            content.Headers.Add("SOAPAction", "\"https://www.w3schools.com/xml/CelsiusToFahrenheit\"");

            // 3) POST isteği – tam servis adresi
            var response = await _httpClient.PostAsync("https://www.w3schools.com/xml/tempconvert.asmx", content);

            var xml = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
           
                return $"HTTP {(int)response.StatusCode} - {response.ReasonPhrase}";
            }

            // 4) XML içinden sonucu çek
            var doc = XDocument.Parse(xml);
            XNamespace ns = "https://www.w3schools.com/xml/";

            var result = doc
                .Descendants(ns + "CelsiusToFahrenheitResult")
                .FirstOrDefault()?.Value;

            return result ?? "Servis sonucu çözülemedi.";
        }
    }
}
