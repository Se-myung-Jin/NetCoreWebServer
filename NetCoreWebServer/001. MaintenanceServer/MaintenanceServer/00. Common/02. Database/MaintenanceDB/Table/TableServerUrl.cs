using Dapper;
using MySqlConnector;

namespace MaintenanceServer
{
    [RefreshableMaintenance]
    public class TableServerUrl
    {
        public int Seq { get; private set; }
        public string Name { get; private set; }
        public string Version { get; private set; }
        public string Url { get; private set; }

        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private static TimeSpan refreshTimeLimit = TimeSpan.FromSeconds(30);
        private static DateTime latestRefreshTime = DateTime.MinValue;

        private static Dictionary<string, TableServerUrl> urlByVersionDic = new Dictionary<string, TableServerUrl>();

        public static async Task RefreshAsync(bool enforced = false)
        {
            DateTime now = DateTime.UtcNow;
            if (!enforced && now - latestRefreshTime < refreshTimeLimit) return;

            latestRefreshTime = now;

            var newUrlByVersionDic = new Dictionary<string, TableServerUrl>();

            var sql = "select `seq`, `name`, `version`, `url` from `serverUrl`;";
            using (var conn = new MySqlConnection(Config.Instance.MaintenanceDB))
            {
                var list = await conn.QueryAsync<TableServerUrl>(sql);

                foreach (var item in list)
                {
                    logger.Warn($"item: {item.Seq}, {item.Name}, {item.Version}, {item.Url}");
                    newUrlByVersionDic.Add($"{item.Name}:{item.Version}", item);
                }
            }

            urlByVersionDic = newUrlByVersionDic;
        }

        public static TableServerUrl Find(string name, string version)
        {
            if (urlByVersionDic.TryGetValue($"{name}:{version}", out var url) == false)
            {
                logger.Error($"TableServerUrl not existed. name: {name}, version: {version}");
            }

            return null;
        }
    }
}
