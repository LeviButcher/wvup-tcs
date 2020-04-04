using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace tcs_service
{
    /// The Start point of the application
    public class Program
    {
        /// The Start point of the application
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        /// Launches the Web App
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
