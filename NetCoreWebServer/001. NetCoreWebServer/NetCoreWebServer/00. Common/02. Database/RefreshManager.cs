using System.Reflection;

namespace NetCoreWebServer
{
    public class RefreshManager
    {
        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static Lazy<RefreshManager> _instance = new Lazy<RefreshManager>(() => new RefreshManager());
        public static RefreshManager Instance => _instance.Value;
        private Dictionary<string, Type> typesToRefresh;

        private RefreshManager()
        {

        }

        public async Task InitializeAsync(Type refreshType = null)
        {
            refreshType = refreshType ?? typeof(RefreshableAttribute);
            var typesToRefresh = new Dictionary<string, Type>();
            var types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var type in types)
            {
                var typeName = type.ToString().ToLower();
                if (typesToRefresh.ContainsKey(typeName)) continue;

                var typeInfo = type.GetTypeInfo();
                var customAttribute = typeInfo.GetCustomAttributes(refreshType, true);
                if (customAttribute.Length > 0)
                {
                    typesToRefresh.Add(typeName, type);
                }
            }

            this.typesToRefresh = typesToRefresh;

            await RefreshAllAsync(false);
        }

        public async Task RefreshAllAsync(bool enforced)
        {
            foreach (var key in typesToRefresh.Keys)
            {
                await RefreshSingleAsync(key, enforced);
            }
        }

        private async Task RefreshSingleAsync(string typeName, bool enforced)
        {
            Type type = null;
            MethodInfo methodInfo;
            ParameterInfo[] parameters;

            try
            {
                typesToRefresh.TryGetValue(typeName.ToLower(), out type);
                if (type == null) return;

                methodInfo = type.GetMethod("RefreshAsync", BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                if (methodInfo == null) return;
                
                parameters = methodInfo.GetParameters();
                if (parameters.Length == 0)
                {
                    await (Task)methodInfo.Invoke(null, null);
                }
                else
                {
                    await (Task)methodInfo.Invoke(null, new object[] { enforced });
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }
    }
}
