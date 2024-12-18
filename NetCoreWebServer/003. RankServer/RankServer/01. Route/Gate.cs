using MessagePack;
using System.Buffers;
using WebProtocol;

namespace RankServer
{
    public class Gate : IRoute
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        
        public async Task Invoke(HttpContext context)
        {
            var httpRequest = context.Request;
            var httpResponse = context.Response;

            ProtocolId protocolId = ProtocolId.None;
            ProtocolReq request = null;

            try
            {
                var length = httpRequest.ContentLength ?? 0;
                byte[] inBuffer = new byte[length];

                using (var stream = httpRequest.Body)
                {
                    request = MessagePackSerializer.Deserialize<ProtocolReq>(stream);
                    protocolId = request.Protocol.ProtocolId;
                }
                logger.Debug(() => $"Something Received : {protocolId}");

                var response = await Service.ProcessAsync(context, request.Protocol);
                await SendResponseAsync(httpResponse, response);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public async Task SendResponseAsync(HttpResponse httpResponse, ProtocolRes response)
        {
            var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);

            MessagePackSerializer.Serialize(stream, response);

            byte[] data = stream.ToArray();

            httpResponse.ContentLength = data.Length;
            Stream outStream = httpResponse.Body;
            await outStream.WriteAsync(data, 0, data.Length);
        }
    }
}
