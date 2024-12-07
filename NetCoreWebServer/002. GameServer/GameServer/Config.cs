using System.Text.Json.Serialization;
using WebServerCore;

namespace GameServer
{
    public class Config : Singleton<Config>
    {
        public string ServerUrl { get; set; }
        public string GameDB { get; set; }
        public string RedisConnector { get; set; }
        [JsonIgnore] public static short BytesPerSecond { get; set; } = 100;
        [JsonIgnore] public static short GracePeriodSecond { get; set; } = 10;

        public static void Load(string path)
        {
            Instance = JsonAppConfig.Load<Config>(path);
        }
    }
}
