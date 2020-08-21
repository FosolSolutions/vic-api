using System.Security.Claims;
using System.Threading.Tasks;
using Vic.Data.Entities;

namespace Vic.Api.Helpers.Authentication
{
    public interface IAuthenticationHelper
    {
        string HashPassword(string password);

        User Validate(string username, string password);

        Task<string> AuthenticateAsync(User user);
    }
}
