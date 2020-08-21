namespace Vic.Api.Helpers.Authentication
{
    public class VicAuthenticationOptions
    {
        #region Properties
        public string Salt { get; set; }
        public string Issuer { get; set; }

        public string Audience { get; set; }
        public string Secret { get; set; }
        public CookieOptions Cookie { get; set; }
        #endregion
    }
}
