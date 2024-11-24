using NLog.Config;

namespace WebServerCore
{
    public class NLogEx
    {
        private static bool SetConfiguration(string filePath, short retryParentDirectory = 5)
        {
            var path = FileEx.SearchParentDirectory(filePath, retryParentDirectory);
            if (path == null)
            {
                throw new FileNotFoundException(filePath);
            }

            NLog.LogManager.Configuration = new XmlLoggingConfiguration(path);

            return true;
        }

        public static bool SetAppConfiguration(string filePath, short retryParentDirectory = 5)
        {
            var baseDirectory = Path.GetDirectoryName(AppContext.BaseDirectory);
            filePath = Path.Combine(baseDirectory, filePath);

            return SetConfiguration(filePath, retryParentDirectory);
        }
    }
}
