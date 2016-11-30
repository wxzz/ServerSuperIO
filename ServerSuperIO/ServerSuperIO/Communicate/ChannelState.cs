using ServerSuperIO.Common;

namespace ServerSuperIO.Communicate
{
    public enum ChannelState
    {
        [EnumDescription("未知")]
        None=0x00,
        [EnumDescription("打开")]
        Open = 0x01,
        [EnumDescription("关闭")]
        Close = 0x02,
    }
}
