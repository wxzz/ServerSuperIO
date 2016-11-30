using ServerSuperIO.Common;

namespace ServerSuperIO.Communicate
{
    public enum CommunicateState
    {
        [EnumDescription("未知")]
        None=0x00,
        [EnumDescription("通讯中断")]
        Interrupt = 0x01,
        [EnumDescription("通讯干扰")]
        Error = 0x02,
        [EnumDescription("通讯正常")]
        Communicate = 0x03
    }
}
