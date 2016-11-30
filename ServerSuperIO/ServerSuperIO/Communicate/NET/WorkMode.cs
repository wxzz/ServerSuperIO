using ServerSuperIO.Common;

namespace ServerSuperIO.Communicate.NET
{
    public enum WorkMode
    {
        [EnumDescription("服务端模式")]
        TcpServer=0x00,
        [EnumDescription("客户端模式")]
        TcpClient=0x01
    }
}
