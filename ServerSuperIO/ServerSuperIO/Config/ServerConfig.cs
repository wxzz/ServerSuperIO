using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using ServerSuperIO.Communicate;
using ServerSuperIO.Communicate.COM;
using ServerSuperIO.Communicate.NET;

namespace ServerSuperIO.Config
{
    [Serializable]
    public class ServerConfig : IServerConfig
    {
        public ServerConfig() : this("Server-" + DateTime.Now.ToString("yyyyMMddHHmmss"))
        {

        }

        public ServerConfig(string serverName)
        {
            ServerSession = Guid.NewGuid().ToString();
            ServerName = serverName;
            if (String.IsNullOrEmpty(ServerName))
            {
                ServerName = "Server-" + DateTime.Now.ToString("yyyyMMddHHmmss");
            }
            ComReadBufferSize = 1024;
            ComWriteBufferSize = 1024;
            ComReadTimeout = 1000;
            ComWriteTimeout = 1000;
            ComLoopInterval = 1000;
            NetReceiveBufferSize = 1024;
            NetSendBufferSize = 1024;
            NetReceiveTimeout = 1024;
            NetSendTimeout = 1024;
            NetLoopInterval = 1000;
            MaxConnects = 1000;
            KeepAlive = 5000;
            ControlMode = ControlMode.Loop;
            ListenPort = 6699;
            BackLog = 1000;
            CheckSameSocketSession = true;
            SocketMode = SocketMode.Tcp;
            DeliveryMode = DeliveryMode.DeviceIP;
            StartReceiveDataFliter = false;
            ClearSocketSession = false;
            ClearSocketSessionInterval = 10;
            ClearSocketSessionTimeOut = 30;
            StartCheckPackageLength = false;
            ParallelInterval = 2000;
        }

        #region 全局
        [Category("1.全局"),
         DisplayName("ServerSession"),
         Description("标识服务的唯ID，一般为Guid"),
         ReadOnly(true)]
        [XmlElement(ElementName = "ServerSession")]
        public string ServerSession { get; set; }

        [Category("1.全局"),
        DisplayName("ServerName"),
        Description("标识服务的标题名称")]
        [XmlElement(ElementName = "ServerName")]
        public string ServerName { get; set; }

        [Category("1.全局"),
        DisplayName("DeliveryMode"),
        Description("接收数据后的分布策略，包括：按设备IP分发（DeviceIP）、按设备编码分发（DeviceCode）")]
        [XmlElement(ElementName = "DeliveryMode")]
        public DeliveryMode DeliveryMode { get; set; }

        [Category("1.全局"),
         DisplayName("ControlMode"),
         Description("调度设备驱动和IO实例的策略，包括:循环模式（Loop）、并发模式（Parallel）、自主模式（Self）和单例模式（Singleton）")]
        [XmlElement(ElementName = "ControlMode")]
        public ControlMode ControlMode { get; set; }

        [Category("1.全局"),
         DisplayName("StartReceiveDataFliter"),
         Description("标识接收数据后是否按协议过滤器的规划过滤数据，不启用则直接返回数据")]
        [XmlElement(ElementName = "StartReceiveDataFliter")]
        public bool StartReceiveDataFliter { get; set; }

        [Category("1.全局"),
        DisplayName("StartCheckPackageLength"),
        Description("标识是否检测数据长度，如果开启，那么会调用协议驱动的GetPackageLength接口，直到接收返回的数据长度的数据")]
        [XmlElement(ElementName = "StartCheckPackageLength")]
        public bool StartCheckPackageLength { get; set; }
        #endregion

        #region 串口
        [Category("2.串口"),
         DisplayName("ComReadBufferSize"),
         Description("设置一次接收数据的字节数组最大值")]
        [XmlElement(ElementName = "ComReadBufferSize")]
        public int ComReadBufferSize { get; set; }

        [Category("2.串口"),
        DisplayName("ComWriteBufferSize"),
        Description("设置一次发送数据的字节数组最大值")]
        [XmlElement(ElementName = "ComWriteBufferSize")]
        public int ComWriteBufferSize { get; set; }

        [Category("2.串口"),
        DisplayName("ComReadTimeout"),
        Description("设置一次读取数据的超时时间")]
        [XmlElement(ElementName = "ComReadTimeout")]
        public int ComReadTimeout { get; set; }

        [Category("2.串口"),
        DisplayName("ComWriteTimeout"),
        Description("设置一次发送数据的超时时间")]
        [XmlElement(ElementName = "ComWriteTimeout")]
        public int ComWriteTimeout { get; set; }

        [Category("2.串口"),
        DisplayName("ComLoopInterval"),
        Description("轮询模式下，发送和接收数据中间的等待时间，串口通讯不支持其他控制模式")]
        [XmlElement(ElementName = "ComLoopInterval")]
        public int ComLoopInterval { get; set; }
        #endregion

        #region 网络
        [Category("3.网络"),
         DisplayName("NetReceiveBufferSize"),
         Description("设置一次接收数据的字节数组最大值")]
        [XmlElement(ElementName = "NetReceiveBufferSize")]
        public int NetReceiveBufferSize { get; set; }

        [Category("3.网络"),
        DisplayName("NetSendBufferSize"),
        Description("设置一次发送数据的字节数组最大值")]
        [XmlElement(ElementName = "NetSendBufferSize")]
        public int NetSendBufferSize { get; set; }

        [Category("3.网络"),
        DisplayName("NetReceiveTimeout"),
        Description("设置一次读取数据的超时时间")]
        [XmlElement(ElementName = "NetReceiveTimeout")]
        public int NetReceiveTimeout { get; set; }

        [Category("3.网络"),
         DisplayName("NetSendTimeout"),
         Description("设置一次发送数据的超时时间")]
        [XmlElement(ElementName = "NetSendTimeout")]
        public int NetSendTimeout { get; set; }

        [Category("3.网络"),
        DisplayName("NetLoopInterval"),
        Description("轮询模式下，发送和接收数据中间的等待时间")]
        [XmlElement(ElementName = "NetLoopInterval")]
        public int NetLoopInterval { get; set; }

        [Category("3.网络"),
        DisplayName("MaxConnects"),
        Description("允许客户端最大的连接数，超取最大值，自动关闭远程连接")]
        [XmlElement(ElementName = "MaxConnects")]
        public int MaxConnects { get; set; }

        [Category("3.网络"),
        DisplayName("KeepAlive"),
        Description("检测死连接、半连接的一种机制")]
        [XmlElement(ElementName = "KeepAlive")]
        public uint KeepAlive { get; set; }

        [Category("3.网络"),
        DisplayName("ListenPort"),
        Description("侦听接收数据的端口")]
        [XmlElement(ElementName = "ListenPort")]
        public int ListenPort { get; set; }

        [Category("3.网络"),
        DisplayName("BackLog"),
        Description("定队列中最多可容纳的等待接受的传入连接数")]
        [XmlElement(ElementName = "BackLog")]
        public int BackLog { get; set; }

        [Category("3.网络"),
        DisplayName("CheckSameSocketSession"),
        Description("对一个固定的设备，只允许有一个有效连接，重复IP多次连接，将断开之前的连接")]
        [XmlElement(ElementName = "CheckSameSocketSession")]
        public bool CheckSameSocketSession { get; set; }

        [Category("3.网络"),
        DisplayName("SocketMode"),
        Description("标识设备是TcpServer、TcpClient模式，如果标识TcpClient模式，会主动连接远程IP和端口")]
        [XmlElement(ElementName = "SocketMode")]
        public SocketMode SocketMode { get; set; }


        [Category("3.网络"),
        DisplayName("ClearSocketSession"),
        Description("标识是否清理连接，如果一个连接在一定时间范围内没有接收到数据，将主动断开连接")]
        [XmlElement(ElementName = "ClearSocketSession")]
        public bool ClearSocketSession { get; set; }

        [Category("3.网络"),
        DisplayName("ClearSocketSessionInterval"),
        Description("如果标识清理连接，那么在此标识清理连接间隔时间")]
        [XmlElement(ElementName = "ClearSocketSessionInterval")]
        public int ClearSocketSessionInterval { get; set; }

        [Category("3.网络"),
        DisplayName("ClearSocketSessionTimeOut"),
        Description("如果标识清理连接，那么在此标识多长时间没有接收到数据进行清理")]
        [XmlElement(ElementName = "ClearSocketSessionTimeOut")]
        public int ClearSocketSessionTimeOut { get; set; }

        [Category("3.网络"),
        DisplayName("ParallelInterval"),
        Description("并发模式下，设置间隔多长时间集中发送请求数据")]
        [XmlElement(ElementName = "ParallelInterval")]
        public int ParallelInterval { get; set; }

        #endregion
    }
}
