using WebProtocol;

namespace GameServer
{
    [ProtocolHandler]
    public class ServiceAllServerUrl : IService
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public ProtocolId ProtocolId { get; } = ProtocolId.AllServerUrl;

        public async Task<ProtocolRes> ProcessAsync(HttpContext context, Protocol protocol)
        {
            var req = (AllServerUrlReq)protocol;
            var res = new AllServerUrlRes();

            res.RankServerUrl = "127.0.0.1:9000";
            res.GuildServerUrl = "127.0.0.1:10000";

            return res;
        }
    }
}