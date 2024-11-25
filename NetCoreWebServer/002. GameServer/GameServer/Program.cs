using Microsoft.AspNetCore;
using WebServerCore;

namespace GameServer
{
    public class Program
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static async Task Main(string[] args)
        {
            ServerAgent.Initialize("GameServer");

            Config.Load("GameServer.config");

            Redis.Initialize();
            Service.Initialize();

            await RefreshManager.Instance.InitializeAsync(typeof(RefreshableAttribute));

            logger.Info("GameServer Starting");
            await CreateWebHostBuilder(args).Build().RunAsync();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls(Config.Instance.ServerUrl);
    }
}
