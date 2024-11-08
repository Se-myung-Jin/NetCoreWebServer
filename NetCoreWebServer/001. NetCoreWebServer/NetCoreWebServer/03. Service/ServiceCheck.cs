﻿using WebProtocol;

namespace NetCoreWebServer
{
    [ProtocolHandler]
    public class ServiceCheck : IService
    {
        public ProtocolId ProtocolId { get; } = ProtocolId.Check;

        public async Task<ProtocolRes> ProcessAsync(HttpContext context, Protocol protocol)
        {
            var req = (CheckReq)protocol;
            var res = new CheckRes();

            return res;
        }
    }
}