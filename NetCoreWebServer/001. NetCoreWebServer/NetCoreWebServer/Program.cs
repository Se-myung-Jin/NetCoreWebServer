using Microsoft.AspNetCore;

namespace NetCoreWebServer
{
    public class Program
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static async Task Main(string[] args)
        {
            ServerAgent.Initialize("NetCoreWebServer");

            Config.Load("NetCoreWebServer.config");

            Service.Initialize();
            Redis.Initialize();

            await RefreshManager.Instance.InitializeAsync(typeof(RefreshableMaintenanceAttribute));

            logger.Info("NetCoreWebServer Starting");
            await CreateWebHostBuilder(args).Build().RunAsync();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls(Config.Instance.ServerUrl);
    }
}
