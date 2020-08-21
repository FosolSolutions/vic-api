using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vic.Api.Areas.Admin.Models.Users;
using Vic.Api.Helpers.Authentication;
using Vic.Api.Models;
using Vic.Data;
using Vic.Data.Entities;

namespace Vic.Api.Areas.Admin.Controllers
{
    /// <summary>
    /// UsersController class, provides a controller for user endpoints.
    /// </summary>
    [Authorize]
    [ApiController]
    [Area("admin")]
    [Route("[area]/[controller]")]
    public class UsersController : ControllerBase
    {
        #region Variables
        private readonly VicContext _context;
        private readonly IAuthenticationHelper _auth;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a UsersController object, initializes with specified arguments.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="auth"></param>
        public UsersController(VicContext context, IAuthenticationHelper auth)
        {
            _context = context;
            _auth = auth;
        }
        #endregion

        #region Endpoints
        /// <summary>
        /// Get a page of users from the datasource.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get(int page = 1, int quantity = 0)
        {
            if (page < 1) page = 1;
            if (quantity < 0) quantity = 0;

            var query = _context.Users.OrderBy(u => u.LastName).Select(u => new UserModel(u));

            if (page > 1 && quantity > 0)
                query = query.Skip((page - 1) & quantity).Take(quantity);

            var users = query.ToArray();

            return new JsonResult(new PageModel<UserModel>(page, users.Count(), users));
        }

        /// <summary>
        /// Fetch the user from the datasource for the specified 'id'.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            var user = _context.Users.FirstOrDefault(i => i.Id == id) ?? throw new ArgumentOutOfRangeException("User does not exist");

            return new JsonResult(new UserModel(user));
        }

        /// <summary>
        /// Add the new user to the datasource.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost()]
        public IActionResult Add(UserModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            var user = new User()
            {
                Id = Guid.NewGuid(),
                Username = model.Username,
                Email = model.Email,
                Password = _auth.HashPassword(model.Password),
                DisplayName = model.DisplayName,
                FirstName = model.FirstName,
                LastName = model.LastName,
                IsEnabled = model.IsEnabled
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return new JsonResult(new UserModel(user));
        }

        /// <summary>
        /// Update the user for the specified 'id'.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public IActionResult Update(Guid id, UserModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            var user = _context.Users.FirstOrDefault(i => i.Id == id) ?? throw new ArgumentOutOfRangeException(nameof(id), "User does not exist.");

            user.Username = model.Username;
            user.Email = model.Email;
            user.Password = _auth.HashPassword(model.Password);
            user.DisplayName = model.DisplayName;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.IsEnabled = model.IsEnabled;

            _context.Update(user);
            _context.SaveChanges();

            return new JsonResult(new UserModel(user));
        }

        /// <summary>
        /// Fetch the user from the datasource for the specified 'id'.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public IActionResult Remove(Guid id)
        {
            var user = _context.Users.FirstOrDefault(i => i.Id == id) ?? throw new ArgumentOutOfRangeException("User does not exist");
            _context.Users.Remove(user);
            _context.SaveChanges();

            return new JsonResult(new UserModel(user));
        }
        #endregion
    }
}
