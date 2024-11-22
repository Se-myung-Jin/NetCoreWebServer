using StackExchange.Redis;

namespace MaintenanceServer
{
    public class Redis
    {
        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static ConnectionMultiplexer connRedis;

        public static void Initialize()
        {
            connRedis = ConnectionMultiplexer.Connect(Config.Instance.RedisConnector);

            Ping();
        }

        private static void Ping()
        {
            var elapsedTime = connRedis.GetDatabase().Ping();

            log.Debug($"Ping for connRedis : {elapsedTime}ms");
        }
    }
}
