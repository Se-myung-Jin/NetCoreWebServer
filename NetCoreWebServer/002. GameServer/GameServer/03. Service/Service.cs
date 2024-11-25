using System.Reflection;
using WebProtocol;
using WebServerCore;

namespace GameServer
{
    public class Service
    {
        private static bool isInitialized = false;

        private static readonly Dictionary<ProtocolId, IService> ProtocolHandler = new Dictionary<ProtocolId, IService>();

        public static void Initialize()
        {
            if (isInitialized) return;

            isInitialized = true;

            InitializeService();
        }

        private static void InitializeService()
        {
            var assembly = Assembly.GetEntryAssembly();
            if (assembly == null)
            {
                throw new InvalidProtocolException("assembly is null.");
            }

            var types = assembly.GetTypes();

            foreach (var type in types)
            {
                if (type.IsInterface) continue;

                if (type.GetInterfaces().Contains(typeof(IService)) == false) continue;

                var typeInfo = type.GetTypeInfo();
                if (typeInfo.GetCustomAttribute<ProtocolHandlerAttribute>() != null)
                {
                    var instance = (IService)Activator.CreateInstance(type);
                    ProtocolHandler.Add(instance.ProtocolId, instance);
                }
            }
        }

        public static async Task<ProtocolRes> ProcessAsync(HttpContext context, Protocol protocol)
        {
            if (ProtocolHandler.TryGetValue(protocol.ProtocolId, out var handler) == false)
            {

            }

            try
            {
                return await handler.ProcessAsync(context, protocol);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
