#pragma warning disable 1591

namespace tcs_service.Helpers {
    public class DbConfig {
        public string ConnectionString { get; set; }
    }

    public class BannerConfig {
        public string User { get; set; }
        public string Api { get; set; }
        public string Password { get; set; }
    }
}