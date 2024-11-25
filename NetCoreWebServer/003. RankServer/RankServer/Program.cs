using Microsoft.AspNetCore;
using WebServerCore;

namespace RankServer
{
    public class Program
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static async Task Main(string[] args)
        {
            ServerAgent.Initialize("RankServer");

            Config.Load("RankServer.config");

            Redis.Initialize();

            await RefreshManager.Instance.InitializeAsync(typeof(RefreshableAttribute));

            logger.Info("RankServer Starting");
            await CreateWebHostBuilder(args).Build().RunAsync();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls(Config.Instance.ServerUrl);
    }
}
