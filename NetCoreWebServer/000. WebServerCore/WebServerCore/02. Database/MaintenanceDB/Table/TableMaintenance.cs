using Dapper;
using MySqlConnector;

namespace WebServerCore
{
    [RefreshableMaintenance]
    public class TableMaintenance
    {
        public int Seq { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool Enable { get; set; }

        private static NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly TimeSpan refreshTimeLimit = TimeSpan.FromSeconds(30);
        private static DateTime refreshedTime = DateTime.MinValue;

        private static Dictionary<string, TableMaintenance> maintenanceByVersionDic;

        public static async Task RefreshAsync(bool enforced = false)
        {
            DateTime now = DateTime.Now;
            if (!enforced && (now - refreshedTime) < refreshTimeLimit) return;

            refreshedTime = now;

            var dic = new Dictionary<string, TableMaintenance>();

            var sql = "select `seq`, `name`, `version`, `startTime`, `endTime`, `enable` from `maintenance`;";
            await using (var conn = new MySqlConnection(ServerCoreConfig.Instance.MaintenanceDB))
            {
                var list = await conn.QueryAsync<TableMaintenance>(sql);
                foreach (var item in list)
                {
                    log.Warn($"item: {item.Seq}, {item.Name}, {item.Version}, {item.StartTime}, {item.EndTime}, {item.Enable}");
                    dic.Add($"{item.Name}:{item.Version}", item);
                }
            }

            maintenanceByVersionDic = dic;
        }

        public static TableMaintenance Find(string name, string version)
        {
            if (maintenanceByVersionDic.TryGetValue($"{name}:{version}", out var value) == false)
            {
                log.Error($"TableMaintenance: Not existed name={name}, version={version}");
            }

            return value;
        }
    }
}