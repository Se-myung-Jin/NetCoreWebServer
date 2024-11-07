using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace NetCoreWebServer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Config.Load("NetCoreWebServer.config");

            Service.Initialize();

            await CreateWebHostBuilder(args).Build().RunAsync();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls(Config.Instance.ServerUrl);
    }
}
