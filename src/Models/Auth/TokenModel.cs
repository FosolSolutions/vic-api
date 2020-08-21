using Vic.Data.Entities;

namespace Vic.Api.Models.Auth
{
    /// <summary>
    /// TokenModel class, provides a model to represent a successful authentication.
    /// </summary>
    public class TokenModel
    {
        #region Properties
        /// <summary>
        /// get/set - The user's display name.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// get/set - The JWT token.
        /// </summary>
        public string AccessToken { get; set; }
        #endregion

        #region Constructors
        public TokenModel() { }

        public TokenModel(User user)
        {
            this.DisplayName = user.DisplayName ?? user.Username;
        }
        #endregion
    }
}
