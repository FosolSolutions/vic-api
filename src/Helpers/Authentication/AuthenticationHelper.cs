using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Vic.Data;
using Vic.Data.Entities;

namespace Vic.Api.Helpers.Authentication
{
    public class AuthenticationHelper: IAuthenticationHelper
    {
        #region Variables
        private readonly VicAuthenticationOptions _options;
        private readonly byte[] _salt;
        private readonly VicContext _context;
        private readonly IHttpContextAccessor _httpContext;
        #endregion

        #region Constructors
        public AuthenticationHelper(IOptions<VicAuthenticationOptions> options, VicContext context, IHttpContextAccessor httpContext)
        {
            _options = options.Value;
            _salt = Encoding.UTF8.GetBytes(_options.Salt);
            _context = context;
            _httpContext = httpContext;
        }
        #endregion

        #region Methods
        public string HashPassword(string password)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: _salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8
                ));
        }

        public User Validate(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username) ?? throw new InvalidOperationException("Unable to authenticate user.");

            var hash = HashPassword(password);
            if (user.Password != hash) throw new InvalidOperationException("Unable to authenticate user.");

            return user;
        }

        public async Task<string> AuthenticateAsync(User user)
        {
            var claims = new[] { new Claim("id", user.Id.ToString()) };
            var identity = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            var token = GenerateJwtToken(principal);

            _httpContext.HttpContext.Response.Cookies.Append(_options.Cookie.Name, token);
            //await _httpContext.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            return await Task.FromResult(token);
        }

        private string GenerateJwtToken(ClaimsPrincipal user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_options.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _options.Issuer,
                Audience = _options.Audience,
                Subject = user.Identity as ClaimsIdentity,
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        #endregion
    }
}
