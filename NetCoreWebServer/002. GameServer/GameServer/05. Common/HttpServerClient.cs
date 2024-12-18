using WebProtocol;
using WebServerCore;
using MessagePack;

namespace GameServer
{
    public class AsyncOut<T>
    {
        private readonly bool m_success;
        private readonly T m_value;
        public readonly Exception Exception;
        
        public AsyncOut(T value)
        {
            m_success = true;
            m_value = value;
        }

        public AsyncOut(Exception ex)
        {
            m_success = false;
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
                    Logger.Error($"BackGateClient Request StatusCode = {result.StatusCode}");
                    return new AsyncOut<T>(new Exception($"BackGateClient Request StatusCode = {result.StatusCode}"));
                }
                    
                var value = (T)MessagePackSerializer.Deserialize<ProtocolRes>(result.Response);
                if (value == null)
                {
                    var message = $"BackGateClient Response null, Url: {url}, protocol: {protocol}";
                        
                    Logger.Error(message);
                    return new AsyncOut<T>(new Exception(message));
                }
                    
                return new AsyncOut<T>(value);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                Logger.Error($"BackGateClient.RequestAsync Url: {url} Protocol: {protocol?.ProtocolId}");
                return new AsyncOut<T>(e);
            }
        }
    }
}