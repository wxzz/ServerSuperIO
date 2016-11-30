using ServerSuperIO.Common;

namespace ServerSuperIO.DataCache
{
    public enum Priority
    {
        [EnumDescription("正常发送")]
        Normal = 0x00,
        [EnumDescription("优先发送")]
        High
    }
}
