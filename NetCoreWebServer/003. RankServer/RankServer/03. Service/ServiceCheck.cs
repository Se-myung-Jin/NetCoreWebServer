using WebProtocol;

namespace RankServer
{
    [ProtocolHandler]
    public class ServiceCheck : IService
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public ProtocolId ProtocolId { get; } = ProtocolId.Check;

        public async Task<ProtocolRes> ProcessAsync(HttpContext context, Protocol protocol)
        {
            var req = (CheckReq)protocol;
            var res = new CheckRes();

            logger.Debug($"req : {req.Name}, {req.Number}, {req.ProtocolId.ToString()}");

            res.IsOk = true;

            return res;
        }
    }
}