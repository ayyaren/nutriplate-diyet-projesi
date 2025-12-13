using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using Nutriplate.GrpcService; // protobuf'dan gelen sınıflar

namespace Nutriplate.Web.Controllers
{
    public class BmiController : Controller
    {
        public async Task<IActionResult> Index(double? height, double? weight)
        {
            if (height.HasValue && weight.HasValue)
            {
                using var channel = GrpcChannel.ForAddress("https://localhost:7226");
                var client = new BmiCalculator.BmiCalculatorClient(channel);

                var request = new BmiRequest
                {
                    Height = height.Value,
                    Weight = weight.Value
                };

                var reply = await client.CalculateBmiAsync(request);

                ViewBag.Bmi = reply.Bmi;
                ViewBag.Category = reply.Category;
                ViewBag.Height = height;
                ViewBag.Weight = weight;

                // -------------------------------
                // Kategoriye göre beslenme önerisi
                // -------------------------------

                string advice = "";

                switch (reply.Category)
                {
                    case "Zayıf":
                        advice = "Kilonuz boyunuza göre düşük görünüyor. Daha dengeli ve kalori açısından zengin besinler tüketebilir, öğünlerinizi aksatmamaya dikkat edebilirsiniz. Gerekirse bir diyetisyen desteği almanız önerilir.";
                        break;

                    case "Normal":
                        advice = "Harika! Sağlıklı bir aralıktasınız. Bunu korumak için düzenli egzersiz, yeterli su tüketimi ve dengeli beslenmeye devam edin.";
                        break;

                    case "Fazla Kilolu":
                        advice = "Kilonuz normalin üzerinde. Yağlı ve şekerli gıdalardan uzak durabilir, daha fazla sebze-meyve tüketip günlük hareket miktarınızı artırabilirsiniz.";
                        break;

                    case "Obez":
                        advice = "BMI değeriniz obezite seviyesinde. Sağlık riskleri oluşabileceğinden profesyonel bir diyetisyen eşliğinde kontrollü kilo verme programı önerilir.";
                        break;
                }

                ViewBag.Advice = advice;
            }

            return View();
        }
    }
}
