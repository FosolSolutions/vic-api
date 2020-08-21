using System;

namespace Vic.Data.Entities
{
    /// <summary>
    /// User class, provides a model to represent a user in the datasource.
    /// </summary>
    public class User : BaseEntity
    {
        #region Properties
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public bool IsEnabled { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a User object.
        /// </summary>
        public User()
        {
            this.Id = Guid.NewGuid();
        }
        #endregion
    }
}
