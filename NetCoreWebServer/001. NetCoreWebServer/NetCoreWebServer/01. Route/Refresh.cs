namespace NetCoreWebServer
{
    public class Refresh : IRoute
    {
        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        public async Task Invoke(HttpContext context)
        {
            try
            {
                var httpResponse = context.Response;

                await RefreshManager.Instance.RefreshAllAsync(true);

                await using var writer = new StreamWriter(httpResponse.Body);
                await writer.WriteLineAsync("All Table Refreshed Successfully");
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }
    }
}
