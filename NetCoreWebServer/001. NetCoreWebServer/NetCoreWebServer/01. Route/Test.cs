public class Test : IRoute
{
    public async Task Invoke(HttpContext context)
    {
        await using (var writer = new StreamWriter(context.Response.Body))
        {
            await writer.WriteLineAsync("Welcome " + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.ffff"));
        }
    }
}
