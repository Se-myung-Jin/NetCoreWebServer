namespace NetCoreWebServer
{
    public class Config : Singleton<Config>
    {
        public string ServerUrl { get; set; } = "http://*:5000";
    }
}
