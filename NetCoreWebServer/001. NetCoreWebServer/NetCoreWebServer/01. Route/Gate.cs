using MessagePack;
using System.Buffers;
using WebProtocol;

namespace NetCoreWebServer
{
    public class Gate : IRoute
    {
        public async Task Invoke(HttpContext context)
        {
            var httpRequest = context.Request;
            var httpResponse = context.Response;

            ProtocolId protocolId = ProtocolId.None;
            ProtocolReq request = null;

            try
            {
                var length = (int)httpRequest.ContentLength;
                byte[] buffer = ArrayPool<byte>.Shared.Rent(length);

                var requestBody = httpRequest.Body;
                var stream = new MemoryStream(buffer);
                var reader = new BinaryReader(stream);

                await requestBody.CopyToAsync(stream);
                stream.Position = 0;

                request = MessagePackSerializer.Deserialize<ProtocolReq>(stream);
                protocolId = request.Protocol.ProtocolId;

                var response = await Service.ProcessAsync(context, request.Protocol);
                await SendResponseAsync(httpResponse, response);
            }
            catch (Exception ex)
            {

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
