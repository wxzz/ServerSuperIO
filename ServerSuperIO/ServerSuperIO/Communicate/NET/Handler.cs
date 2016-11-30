using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace ServerSuperIO.Communicate.NET
{
    /// <summary>
    /// 侦听错误事件
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    public delegate void ErrorHandler(object source, Exception e);

    /// <summary>
    /// 接收客户端事件
    /// </summary>
    /// <param name="source"></param>
    /// <param name="client"></param>
    /// <param name="state"></param>
    public delegate void NewClientAcceptHandler(object source, Socket client, object state);

    /// <summary>
    /// 关闭Socket事件
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    public delegate void CloseSocketHandler(object source, ISocketSession socketSession);

    /// <summary>
    /// 接收数据
    /// </summary>
    /// <param name="source"></param>
    /// <param name="socketSession"></param>
    /// <param name="dataPackage"></param>
    public delegate void SocketReceiveDataHandler(object source, ISocketSession socketSession, IReceivePackage dataPackage);
}
