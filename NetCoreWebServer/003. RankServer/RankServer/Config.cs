﻿using WebServerCore;

namespace RankServer
{
    public class Config : Singleton<Config>
    {
        public string ServerUrl { get; set; }
        public string RankDB { get; set; }
        public string RedisConnector { get; set; }

        public static void Load(string path)
        {
            Instance = JsonAppConfig.Load<Config>(path);
        }
    }
}
