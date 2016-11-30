using ServerSuperIO.Common;

namespace ServerSuperIO.Device
{
    public enum DeviceType : byte
    {
        [EnumDescription("普通设备")]
        Common = 0x00,
        [EnumDescription("虚拟设备")]
        Virtual = 0x01,
    }
}
