using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Server.Kestrel.Core;
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
            if (exception != null)
            {
                logger.Error(exception);
            }
            
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

            Console.CancelKeyPress += (_, args) =>
            {
                args.Cancel = true;
                logger.Error("Received CancelKeyPress");
            };

            System.Runtime.Loader.AssemblyLoadContext.Default.Unloading += (ctx) =>
            {
                logger.Error($"Received AssemblyLoadContext.Default.Unloading");
            };

            AppDomain.CurrentDomain.ProcessExit += (_, _) =>
            {
                logger.Error("Received ProcessExit");
            };

            try
            {
                await CreateWebHostBuilder(args).Build().RunAsync();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            logger.Error("Application Close");
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, options) =>
                {
                    foreach (var s in options.Sources)
                    {
                        if (s is FileConfigurationSource)
                        {
                            logger.Debug($"ReloadOnChange: {((FileConfigurationSource)s).Path}");
                            ((FileConfigurationSource)s).ReloadOnChange = false;
                        }
                    }
                })
                .UseKestrel((hostingContext, options) =>
                {
                    options.Configure(hostingContext.Configuration.GetSection("Kestrel"), reloadOnChange: false);
                    options.Limits.MinRequestBodyDataRate = new MinDataRate(Config.BytesPerSecond, TimeSpan.FromSeconds(Config.GracePeriodSecond));
                })
                .UseStartup<Startup>()
                .UseUrls(Config.Instance.ServerUrl);
    }
}
