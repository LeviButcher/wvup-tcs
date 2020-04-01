namespace tcs_service.Helpers
{
    /// <summary>Environment variables for the database</summary>
    public class DbConfig
    {
        /// <summary>ConnectionString to database</summary>
        public string ConnectionString { get; set; }
    }

    /// <summary>Environment variables for accessing Banner Api</summary>
    public class BannerConfig
    {
        /// <summary>UserName for banner api</summary>
        public string User { get; set; }

        /// <summary>Url to the banner api</summary>
        public string Api { get; set; }

        /// <summary>Password for the user to the banner api</summary>
        public string Password { get; set; }
    }

    /// <summary>Environment variables for the general application</summary>
    public class AppSettings
    {
        /// <summary>The Secret Passphrase used for encrypting passwords</summary>
        public string Secret { get; set; }
    }
}