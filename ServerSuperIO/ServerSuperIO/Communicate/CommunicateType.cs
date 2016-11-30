using ServerSuperIO.Common;

namespace ServerSuperIO.Communicate
{
    public enum CommunicateType : byte
    {
        [EnumDescription("网络")]
        NET,
        [EnumDescription("串口")]
        COM,
    }
}
