using Dapper;
using MySqlConnector;

namespace NetCoreWebServer
{
    public class ServerUrl
    {
        public int Seq { get; private set; }
        public string Name { get; private set; }
        public string Version { get; private set; }
        public string Url { get; private set; }

        public static async Task<List<ServerUrl>> FetchAllAsync()
        {
            var sql = "select `seq`, `name`, `version`, `url` from `serverUrl`;";

            await using (var conn = new MySqlConnection(Config.Instance.MaintenanceDB))
            {
                return (await conn.QueryAsync<ServerUrl>(sql)).AsList();
            }
        }
    }
}
