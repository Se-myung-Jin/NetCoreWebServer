using WebProtocol;

namespace GameServer
{
    public interface IService
    {
        ProtocolId ProtocolId { get; }
        Task<ProtocolRes> ProcessAsync(HttpContext context, Protocol protocol);
    }
}
