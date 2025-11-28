using System.Threading.Tasks;

namespace Nutriplate.Web.Services
{
    // DİKKAT: interface olmalı, class değil
    public interface IAuthService
    {
        Task<AuthResult?> LoginAsync(string email, string password);
        Task<bool> RegisterAsync(string email, string password, string name);
    }

    // Node.js API'den dönecek sonucu temsil eden DTO
    public class AuthResult
    {
        public string Token { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
