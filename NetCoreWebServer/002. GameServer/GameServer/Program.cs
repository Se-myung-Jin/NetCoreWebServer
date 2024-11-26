using Microsoft.AspNetCore;
using WebServerCore;

namespace GameServer
{
    public class Program
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            logger.Error("UnhandledExceptionHandler Call");
            
            Exception exception = (Exception)args.ExceptionObject;
            logger.Error(exception);
            
            Environment.Exit(-9999);
        }
        
        public static async Task Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledExceptionHandler);

            System.Net.ServicePointManager.DefaultConnectionLimit = int.MaxValue;
            ThreadPool.GetMaxThreads(out int _, out int completionPortThreads);
            ThreadPool.SetMinThreads(512, completionPortThreads);
            
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
