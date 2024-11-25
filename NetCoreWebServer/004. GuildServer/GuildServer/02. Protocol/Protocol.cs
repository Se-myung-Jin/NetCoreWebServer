using MessagePack;

namespace WebProtocol
{
    public enum ProtocolId
    {
        None = 0,
        Check = 1,
    }

    [MessagePackObject]
    [Union((int)ProtocolId.Check, typeof(CheckReq))]
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
    [Union((int)ProtocolId.Check, typeof(CheckRes))]
    public abstract class ProtocolRes
    {
        [Key(0)] public ProtocolId ProtocolId { get; set; }
        [Key(1)] public Result Result { get; set; }

        protected const int BaseKey = 2;

        public ProtocolRes() { }
        public ProtocolRes(ProtocolId protocolId) { ProtocolId = protocolId; }
    }

    [MessagePackObject]
    public class CheckReq : Protocol
    {
        [Key(BaseKey + 0)] public string Name { get; set; }
        [Key(BaseKey + 1)] public int Number { get; set; }

        public CheckReq() : base(ProtocolId.Check) { }
    }

    [MessagePackObject]
    public class CheckRes : ProtocolRes
    {
        [Key(BaseKey + 0)] public bool IsOk { get; set; }

        public CheckRes() : base(ProtocolId.Check) { }
    }
}