using System.Text.Json;

namespace NetCoreWebServer
{
    public class JsonAppConfig
    {
        public static T Load<T>(string filePath, short retryParentDirectory = 5)
        {
            var baseDirectory = Path.GetDirectoryName(AppContext.BaseDirectory);
            var path = Path.Combine(baseDirectory, filePath);

            path = FileEx.SearchParentDirectory(path, retryParentDirectory);
            if (path == null)
            {
                return default;
            }

            var json = File.ReadAllText(path);
            if (json == null)
            {
                throw new FileNotFoundException(filePath);
            }

            return JsonSerializer.Deserialize<T>(json);
        }
    }
}
