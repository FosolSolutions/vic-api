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
        [HttpPost("token")]
        public async Task<IActionResult> TokenAsync(LoginModel login)
        {
            var user = _auth.Validate(login.Username, login.Password);

            return new JsonResult(await _auth.AuthenticateAsync(user));
        }

        /// <summary>
        /// Refresh the access token if the refresh token is valid.
        /// </summary>
        /// <returns></returns>
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshTokenAsync()
        {
            var username = User.Identity.Name;
            var user = _auth.FindUser(username);
            return new JsonResult(await _auth.AuthenticateAsync(user));
        }
        #endregion
    }
}
