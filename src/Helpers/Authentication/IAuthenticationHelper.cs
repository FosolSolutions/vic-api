using System.Security.Claims;
using System.Threading.Tasks;
using Vic.Api.Models.Auth;
using Vic.Data.Entities;

namespace Vic.Api.Helpers.Authentication
{
    public interface IAuthenticationHelper
    {
        string HashPassword(string password);

        User FindUser(string username);

        User Validate(string username, string password);

        Task<TokenModel> AuthenticateAsync(User user);
    }
}
