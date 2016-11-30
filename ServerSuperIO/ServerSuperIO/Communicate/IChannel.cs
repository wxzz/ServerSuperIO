using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ServerSuperIO.Protocol;
using ServerSuperIO.Server;
using ServerSuperIO.Protocol.Filter;

namespace ServerSuperIO.Communicate
{
    public interface IChannel : IServerProvider,IDisposable
    {
        void Initialize();

        /// <summary>
        /// 同步锁
        /// </summary>
        object SyncLock { get; }

        /// <summary>
        /// IO关键字，如果是串口通讯为串口号，如：COM1;如果是网络通讯为IP和端口，例如：127.0.0.1
        /// </summary>
        string Key { get; }

        /// <summary>
        /// 唯一ID
        /// </summary>
        string SessionID { get; }

        /// <summary>
        /// 通道实例，可以是COM，也可以是SOCKET
        /// </summary>
        IChannel Channel { get; }

        /// <summary>
        /// 读IO，带过滤器
        /// </summary>
        /// <param name="receiveFilter"></param>
        /// <returns></returns>
        IList<byte[]> Read(IReceiveFilter receiveFilter);

        /// <summary>
        /// 写IO
        /// </summary>
        int Write(byte[] data);

        /// <summary>
        /// 关闭
        /// </summary>
        void Close();

        /// <summary>
        /// IO类型
        /// </summary>
        CommunicateType CommunicationType { get; }

        /// <summary>
        /// 是否被释放了
        /// </summary>
        bool IsDisposed { get; }
    }
}
