using WebProtocol;
using WebServerCore;

namespace MaintenanceServer
{
    [ProtocolHandler]
    public class ServiceCheckMaintenance : IService
    {
        public ProtocolId ProtocolId { get; } = ProtocolId.CheckMaintenance;

        public async Task<ProtocolRes> ProcessAsync(HttpContext context, Protocol protocol)
        {
            var req = (CheckMaintenanceReq)protocol;
            var res = new CheckMaintenanceRes();

            if (MaintenanceManager.IsMaintenance(req.ServerName, req.Version))
            {
                res.Result = Result.MaintenanceNow;
                
                return res;
            }

            var url = TableServerUrl.Find(req.ServerName, req.Version);
            if (url == null)
            {
                res.Result = Result.NotRegistServerUrl;

                return res;
            }

            res.ServerUrl = url.Url;

            return res;
        }
    }
}