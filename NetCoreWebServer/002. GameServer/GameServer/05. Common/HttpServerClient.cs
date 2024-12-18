using WebProtocol;
using WebServerCore;
using MessagePack;

namespace GameServer
{
    public class AsyncOut<T>
    {
        private readonly bool success;
        private readonly T value;
        public readonly Exception Exception;
        
        public AsyncOut(T value)
        {
            success = true;
            value = value;
        }

        public AsyncOut(Exception ex)
        {
            success = false;
            Exception = ex;
        }
    }

    public class HttpServerClient
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly Http Http = new Http(TimeSpan.FromSeconds(30));
            
        public static async Task<AsyncOut<T>> RequestAsync<T>(string url, Protocol protocol) where T: ProtocolRes
        {
            try
            {
                ProtocolReq req = new ProtocolReq(protocol);
                
                var bytes = MessagePackSerializer.Serialize(req);
                    
                var result = await Http.RequestAsync(HttpMethod.Post, url, body: bytes);
                if ((int)result.StatusCode != 200)
                {
                    Logger.Error($"HttpServerClient Request StatusCode = {result.StatusCode}");
                    return new AsyncOut<T>(new Exception($"HttpServerClient Request StatusCode = {result.StatusCode}"));
                }
                    
                var value = (T)MessagePackSerializer.Deserialize<ProtocolRes>(result.Response);
                if (value == null)
                {
                    var message = $"HttpServerClient Response null, Url: {url}, protocol: {protocol}";
                        
                    Logger.Error(message);
                    return new AsyncOut<T>(new Exception(message));
                }
                    
                return new AsyncOut<T>(value);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                Logger.Error($"HttpServerClient.RequestAsync Url: {url} Protocol: {protocol?.ProtocolId}");
                return new AsyncOut<T>(e);
            }
        }
    }
}