using WebProtocol;

namespace RankServer
{
    [ProtocolHandler]
    public class ServiceCheck : IService
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public ProtocolId ProtocolId { get; } = ProtocolId.AllServerUrl;

        public async Task<ProtocolRes> ProcessAsync(HttpContext context, Protocol protocol)
        {
            var req = (AllServerUrlReq)protocol;
            var res = new AllServerUrlRes();

            logger.Debug($"req : {req.ProtocolId.ToString()}");

            res.RankServerUrl = "Hello World";

            return res;
        }
    }
}