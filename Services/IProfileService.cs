using System.Threading.Tasks;
using Nutriplate.Web.ViewModels;

namespace Nutriplate.Web.Services
{
    public interface IProfileService
    {
        Task<ProfileViewModel?> GetProfileAsync(string jwtToken);
        Task<bool> UpdateProfileAsync(ProfileViewModel model, string jwtToken);
    }
}
