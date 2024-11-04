using Microsoft.AspNetCore.Http.Features;

namespace NetCoreWebServer
{
    public class Route
    {
        private readonly RequestDelegate next;

        private readonly Dictionary<string, IRoute> routeDic = new (StringComparer.OrdinalIgnoreCase)
        {
            { "/Test", new Test() },
        };

        public Route(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var syncIOFeature = context.Features.Get<IHttpBodyControlFeature>();
            if (syncIOFeature != null)
            {
                syncIOFeature.AllowSynchronousIO = true;
            }

            if (routeDic.TryGetValue(context.Request.Path.Value, out IRoute route))
            {
                await route.Invoke(context);
            }
            else
            {
                await next.Invoke(context);
            }
        }
    }
}
