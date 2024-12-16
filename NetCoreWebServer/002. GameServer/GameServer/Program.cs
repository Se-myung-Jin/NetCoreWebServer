using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using WebServerCore;

namespace GameServer
{
    public class Program
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private static IWebHost webHost;
        private static JobScheduler scheduler = new JobScheduler();

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

            RegisterJobScheduler();

            logger.Info("GameServer Starting");

            Console.CancelKeyPress += (_, args) =>
            {
                args.Cancel = true;
                logger.Error("Received CancelKeyPress");
                webHost?.Dispose();
            };

            System.Runtime.Loader.AssemblyLoadContext.Default.Unloading += (ctx) =>
            {
                logger.Error($"Received AssemblyLoadContext.Default.Unloading");
                webHost?.Dispose();
            };

            AppDomain.CurrentDomain.ProcessExit += (_, _) =>
            {
                logger.Error("Received ProcessExit");
                webHost?.Dispose();
            };

            try
            {
                webHost = CreateWebHostBuilder(args).Build();
                await webHost.RunAsync();
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

        private static void RegisterJobScheduler()
        {
            scheduler.RegisterJob(() => logger.Debug($"Action 1 executed at {DateTime.Now}"), 3000);
            scheduler.RegisterJob(() => logger.Debug($"Action 2 executed at {DateTime.Now}"), 7000);
        }
    }
}
