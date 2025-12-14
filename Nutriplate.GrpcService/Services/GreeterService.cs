using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Nutriplate.GrpcService.Services
{
    // .proto içindeki service BmiCalculator ile eþleþiyor
    public class BmiService : BmiCalculator.BmiCalculatorBase
    {
        private readonly ILogger<BmiService> _logger;

        public BmiService(ILogger<BmiService> logger)
        {
            _logger = logger;
        }

        public override Task<BmiReply> CalculateBmi(BmiRequest request, ServerCallContext context)
        {
            // basit validasyon
            if (request.Height <= 0)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, "Boy 0'dan büyük olmalýdýr."));
            }

            // BMI hesaplama: kilo / (boy * boy)
            var bmi = request.Weight / (request.Height * request.Height);

            string category;
            if (bmi < 18.5)
                category = "Zayýf";
            else if (bmi < 25)
                category = "Normal";
            else if (bmi < 30)
                category = "Fazla kilolu";
            else
                category = "Obez";

            return Task.FromResult(new BmiReply
            {
                Bmi = bmi,
                Category = category
            });
        }
    }
}
