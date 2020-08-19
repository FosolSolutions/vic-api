namespace Synology.Options
{
    /// <summary>
    /// SynologyOptions class, provides a way to configure the connection to the Synology API.
    /// </summary>
    public class SynologyOptions
    {
        #region Properties
        /// <summary>
        /// get/set - The URL to the Synology host.
        /// </summary>
        public string HostUrl { get; set; }

        /// <summary>
        /// get/set - The username to authenticate with.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// get/set - The password to authenticate with.
        /// </summary>
        public string Password { get; set; }
        #endregion
    }
}
