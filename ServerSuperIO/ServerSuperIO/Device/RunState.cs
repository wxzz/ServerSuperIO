using ServerSuperIO.Common;

namespace ServerSuperIO.Device
{
    public enum RunState:byte
    {
        [EnumDescription("未知")]
        None = 0x00,
        [EnumDescription("设备运行")]
        Run = 0x01,
        [EnumDescription("设备停止")]
        Stop = 0x02
    }
}
