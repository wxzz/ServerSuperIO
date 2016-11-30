using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ServerSuperIO.Communicate.NET
{
    public interface ISocketSession:IChannel
    {
        #region 属性
        /// <summary>
        /// 远程网络连接的IP
        /// </summary>
        string RemoteIP { get;  }

        /// <summary>
        /// 远程网络连接的Port
        /// </summary>
        int RemotePort { get;  }

        /// <summary>
        /// Socket实例
        /// </summary>
        Socket Client { get; }

        /// <summary>
        /// 远程点
        /// </summary>
        IPEndPoint RemoteEndPoint { get; }

        ///// <summary>
        ///// 异步代理
        ///// </summary>
        ISocketAsyncEventArgsProxy SocketAsyncProxy { get; }

        /// <summary>
        /// 关闭连接
        /// </summary>
        event CloseSocketHandler CloseSocket;

        /// <summary>
        /// 接收数据
        /// </summary>
        event SocketReceiveDataHandler SocketReceiveData;

        ///// <summary>
        ///// 接收数据
        ///// </summary>
        void TryReceive();

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="type">true:异步，false:同步</param>
        void TrySend(byte[] data,bool type);

        /// <summary>
        /// 链接的时间 
        /// </summary>
        DateTime StartTime { get; }

        /// <summary>
        /// 接收有效数据的时间
        /// </summary>
        DateTime LastActiveTime { get; set; }

        #endregion
    }
}
