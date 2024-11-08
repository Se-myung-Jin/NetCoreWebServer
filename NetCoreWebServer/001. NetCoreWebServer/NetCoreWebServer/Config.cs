﻿namespace NetCoreWebServer
{
    public class Config : Singleton<Config>
    {
        public string ServerUrl { get; set; }

        public static void Load(string path)
        {
            Instance = JsonAppConfig.Load<Config>(path);
        }

    }
}
