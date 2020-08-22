namespace Vic.Api.Helpers.Mail
{
    public class MailOptions
    {
        #region Properties
        public string Host { get; set; }
        public int Port { get; set; }
        public string FromEmail { get; set; }
        public string ContactEmail { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        #endregion
    }
}
