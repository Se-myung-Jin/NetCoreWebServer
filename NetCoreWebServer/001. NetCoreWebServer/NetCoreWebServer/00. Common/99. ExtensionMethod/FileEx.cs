namespace NetCoreWebServer
{
    public class FileEx
    {
        public static string SearchParentDirectory(string filePath, short retryParentDirectory = 5)
        {
            if (File.Exists(filePath))
            {
                return filePath;
            }

            var dir = Path.GetDirectoryName(filePath);
            var fileName = Path.GetFileName(filePath);

            for (int i = 1; i <= retryParentDirectory; i++)
            {
                dir = Path.Combine(dir, "..");

                dir = Path.GetFullPath(dir);

                filePath = Path.Combine(dir, fileName);

                if (File.Exists(filePath))
                {
                    return filePath;
                }
            }

            return null;
        }
    }
}
