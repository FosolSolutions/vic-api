using System;
using System.ComponentModel.DataAnnotations;
using Vic.Data.Entities;

namespace Vic.Api.Areas.Admin.Models.Users
{
    public class UserModel
    {
        #region Properties
        public Guid Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Email { get; set; }

        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public bool IsEnabled { get; set; }
        #endregion

        #region Constructors
        public UserModel() { }

        public UserModel(User user)
        {
            this.Id = user.Id;
            this.Username = user.Username;
            this.Email = user.Email;
            this.FirstName = user.FirstName;
            this.LastName = user.LastName;
            this.DisplayName = user.DisplayName;
            this.IsEnabled = user.IsEnabled;
        }
        #endregion
    }
}
