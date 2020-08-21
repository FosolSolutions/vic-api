using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Vic.Api.Helpers.Authentication;
using Vic.Api.Models.Auth;

namespace Vic.Api.Controllers
{
    /// <summary>
    /// MetadaAuthControllertaController class, provides a controller for authentication endpoints.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        #region Variables
        private readonly IAuthenticationHelper _auth;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a AuthController object, initializes with specified arguments.
        /// </summary>
        /// <param name="auth"></param>
        public AuthController(IAuthenticationHelper auth)
        {
            _auth = auth;
        }
        #endregion

        #region Endpoints
        /// <summary>
        /// Authenticate the user and return a JWT token.
        /// </summary>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(LoginModel login)
        {
            var user = _auth.Validate(login.Username, login.Password);

            return new JsonResult(new TokenModel(user)
            {
                AccessToken = await _auth.AuthenticateAsync(user)
            });
        }

        /// <summary>
        /// Logout and delete cookie and token.
        /// </summary>
        /// <returns></returns>
        [HttpPut("logout")]
        public async Task<IActionResult> LogoutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return new JsonResult(new { Success = true });
        }
        #endregion
    }
}
