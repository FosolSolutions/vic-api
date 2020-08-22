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
using Vic.Api.Models.Auth;
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

        public User FindUser(string username)
        {
            return _context.Users.FirstOrDefault(u => u.Username == username) ?? throw new InvalidOperationException("Unable to authenticate user.");
        }

        public User Validate(string username, string password)
        {
            var user = FindUser(username);

            var hash = HashPassword(password);
            if (user.Password != hash) throw new InvalidOperationException("Unable to authenticate user.");

            return user;
        }

        public async Task<TokenModel> AuthenticateAsync(User user)
        {
            var claims = new[] 
            { 
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName),
                new Claim("display_name", user.DisplayName)
            };
            var accessToken = GenerateJwtToken(GeneratePrincipal(JwtBearerDefaults.AuthenticationScheme, claims), _options.AccessTokenExpiresIn);
            var refreshToken = GenerateJwtToken(GeneratePrincipal(JwtBearerDefaults.AuthenticationScheme, new Claim(ClaimTypes.Name, user.Username)), _options.RefreshTokenExpiresIn);

            //_httpContext.HttpContext.Response.Cookies.Append(_options.Cookie.Name, token);
            //await _httpContext.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            return await Task.FromResult(new TokenModel(accessToken, _options.AccessTokenExpiresIn, refreshToken, _options.RefreshTokenExpiresIn, _options.DefaultScope));
        }

        private ClaimsPrincipal GeneratePrincipal(string authenticationScheme, params Claim[] claims)
        {
            var identity = new ClaimsIdentity(claims, authenticationScheme);
            return new ClaimsPrincipal(identity);
        }

        private string GenerateJwtToken(ClaimsPrincipal user, TimeSpan expiresIn)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_options.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _options.Issuer,
                Audience = _options.Audience,
                Subject = user.Identity as ClaimsIdentity,
                Expires = DateTime.UtcNow.Add(expiresIn),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        #endregion
    }
}
