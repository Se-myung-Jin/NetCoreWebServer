using MessagePack;

namespace WebProtocol
{
    public enum ProtocolId
    {
        Error = -1,
        None = 0,
        CheckMaintenance = 1,
    }

    [MessagePackObject]
    [Union((int)ProtocolId.CheckMaintenance, typeof(CheckMaintenanceReq))]
    public abstract class Protocol
    {
        [Key(0)] public ProtocolId ProtocolId { get; set; }

        protected const int BaseKey = 1;

        public Protocol() { }

        public Protocol(ProtocolId protocolId)
        {
            ProtocolId = protocolId;
        }
    }

    [MessagePackObject]
    public class ProtocolReq
    {
        [Key(0)] public Protocol Protocol { get; set; }

        public ProtocolReq() { }

        public ProtocolReq(Protocol protocol)
        {
            Protocol = protocol;
        }
    }

    [MessagePackObject]
    [Union((int)ProtocolId.CheckMaintenance, typeof(CheckMaintenanceRes))]
    [Union((int)ProtocolId.Error, typeof(ErrorRes))]
    public abstract class ProtocolRes
    {
        [Key(0)] public ProtocolId ProtocolId { get; set; }
        [Key(1)] public Result Result { get; set; } = Result.Ok;

        protected const int BaseKey = 2;

        public ProtocolRes() { }
        public ProtocolRes(ProtocolId protocolId) { ProtocolId = protocolId; }
    }

    [MessagePackObject]
    public class CheckMaintenanceReq : Protocol
    {
        [Key(BaseKey + 0)] public string ServerName { get; set; }
        [Key(BaseKey + 1)] public string Version { get; set; }

        public CheckMaintenanceReq() : base(ProtocolId.CheckMaintenance) { }
    }

    [MessagePackObject]
    public class CheckMaintenanceRes : ProtocolRes
    {
        [Key(BaseKey + 0)] public string ServerUrl { get; set; }

        public CheckMaintenanceRes() : base(ProtocolId.CheckMaintenance) { }
    }

    [MessagePackObject]
    public class ErrorRes : ProtocolRes
    {

        public ErrorRes() : base(ProtocolId.Error) { }
    }
}