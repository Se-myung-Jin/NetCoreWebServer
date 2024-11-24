using MessagePack;
using WebProtocol;

namespace TestClient
{
    public class HttpAgent
    {
        private static Http http = new Http(TimeSpan.FromSeconds(7));

        public static async Task<T> RequestLobbyAsync<T>(string lobbyUrl, Protocol protocol) where T : ProtocolRes, new()
        {
            return await RequestAsync<T>(lobbyUrl, protocol);
        }

        private static async Task<T> RequestAsync<T>(string url, Protocol protocol) where T : ProtocolRes, new()
        {
            try
            {
                if (null == url || 0 >= url.Length)
                {
                    return new T { Result = Result.UnknownProtocol };
                }

                var packet = new ProtocolReq { Protocol = protocol };
                var bytes = MessagePackSerializer.Serialize(packet);
                var result = await http.RequestAsync(HttpMethod.Post, url, body: bytes);

                return (T)MessagePackSerializer.Deserialize<ProtocolRes>(result.Data);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
