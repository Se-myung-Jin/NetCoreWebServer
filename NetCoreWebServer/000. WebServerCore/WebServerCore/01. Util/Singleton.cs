namespace WebServerCore
{
    public class Singleton<T> where T : class, new()
    {
        public static T Instance { get; protected set; }

        static Singleton()
        {
            Instance = new T();
        }
    }
}