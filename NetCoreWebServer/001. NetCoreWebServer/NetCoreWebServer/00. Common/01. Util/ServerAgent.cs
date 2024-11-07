using System.Reflection;

namespace NetCoreWebServer
{
    public static class ServerAgent
    {
        public static string Environment => Environment;
        public static string ApplicationName => applicationName;

        private static string environment;
        private static string applicationName;

        private static bool isInitialized;

        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static void Initialize(string appName = null)
        {
            if (isInitialized) return;

            environment = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
            applicationName = string.IsNullOrEmpty(appName) ? Assembly.GetExecutingAssembly().GetName().Name : appName;

            SetupNLog();

            isInitialized = true;
        }

        private static void SetupNLog()
        {
            NLogEx.SetAppConfiguration("NLog.config");

            NLog.GlobalDiagnosticsContext.Set("Environment", environment);
            NLog.GlobalDiagnosticsContext.Set("UserName", System.Environment.UserName);
            NLog.GlobalDiagnosticsContext.Set("ApplicationName", applicationName);
        }
    }
}
