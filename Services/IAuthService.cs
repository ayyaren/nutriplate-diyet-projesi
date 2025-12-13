using System.Threading.Tasks;
using Nutriplate.Web.ViewModels;

namespace Nutriplate.Web.Services
{
    public interface IAuthService
    {
        Task<AuthResult?> LoginAsync(string email, string password);
        Task<bool> RegisterAsync(string email, string password, string name);

        // Yeni: Diyetisyen kaydı
        Task<bool> DietitianRegisterAsync(string email, string password, string name);
    }


    public class AuthResult
    {
        public string Token { get; set; } = "";
        public string UserId { get; set; } = "";
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Role { get; set; } = "";
    }
}
