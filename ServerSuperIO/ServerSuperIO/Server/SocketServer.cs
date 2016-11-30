using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Xml.Xsl;
using ServerSuperIO.Common;
using ServerSuperIO.Communicate;
using ServerSuperIO.Communicate.NET;
using ServerSuperIO.Config;
using ServerSuperIO.Device;
using ServerSuperIO.Log;

namespace ServerSuperIO.Server
{
    public abstract class SocketServer : ComServer
    {
        private List<ISocketListener> _Listeners;
        private BufferManager _BufferManager;
        private SocketAsyncEventArgsProxyPool _SocketAsyncPool;
        private ISocketConnector _SocketConnector;

        private System.Threading.Timer _ClearSocketSessionTimer = null;

        internal SocketServer(IServerConfig config, IDeviceContainer deviceContainer = null, ILogContainer logContainer = null)
            : base(config, deviceContainer, logContainer)
        {

        }

        public override void Start()
        {
            base.Start();
            InitSocketAsyncPool();
            InitListener();
            InitConnector();
            StartClearSessionTimer();
        }

        private void InitListener()
        {
            string hostName = Dns.GetHostName();
            IPAddress[] ipasAddresses = Dns.GetHostAddresses(hostName);
            List<IPAddress> list = new List<IPAddress>(ipasAddresses) { IPAddress.Parse("127.0.0.1") };
            _Listeners = new List<ISocketListener>(list.Count);
            foreach (IPAddress ipa in list)
            {
                ListenerInfo info = new ListenerInfo()
                {
                    BackLog = ServerConfig.BackLog,
                    EndPoint = new IPEndPoint(ipa, ServerConfig.ListenPort)
                };

                ISocketListener socketListener;
                if (this.ServerConfig.SocketMode == SocketMode.Tcp)
                {
                    socketListener = new TcpSocketListener(info);
                }
                else
                {
                    socketListener = new UdpSocketListener(info);
                }

                socketListener.NewClientAccepted += tcpSocketListener_NewClientAccepted;
                socketListener.Error += tcpSocketListener_Error;
                socketListener.Stopped += tcpSocketListener_Stopped;
                socketListener.Start(this.ServerConfig);

                _Listeners.Add(socketListener);
            }
        }

        private void InitSocketAsyncPool()
        {
            int netReceiveBufferSize = ServerConfig.NetReceiveBufferSize;
            if (netReceiveBufferSize <= 0)
                netReceiveBufferSize = 1024;

            _BufferManager = new BufferManager(netReceiveBufferSize * ServerConfig.MaxConnects, netReceiveBufferSize);

            try
            {
                _BufferManager.InitBuffer();
            }
            catch (Exception ex)
            {
                Logger.Error(true, "", ex);
                throw;
            }

            _SocketAsyncPool = new SocketAsyncEventArgsProxyPool();

            SocketAsyncEventArgsEx socketEventArg;
            for (int i = 0; i < ServerConfig.MaxConnects; i++)
            {
                socketEventArg = new SocketAsyncEventArgsEx();
                _BufferManager.SetBuffer(socketEventArg);
                socketEventArg.Initialize();//初始化缓存接口
                _SocketAsyncPool.Push(new SocketAsyncEventArgsProxy(socketEventArg));
            }
        }

        private void InitConnector()
        {
            if (ServerConfig.ControlMode == ControlMode.Loop
                || ServerConfig.ControlMode == ControlMode.Self
                || ServerConfig.ControlMode == ControlMode.Parallel)
            {
                _SocketConnector = new SocketConnector();
                _SocketConnector.NewClientConnected += tcpSocketListener_NewClientAccepted;
                _SocketConnector.Error += tcpSocketListener_Error;
                _SocketConnector.Setup(this);
                _SocketConnector.Start();
            }
        }

        private void StartClearSessionTimer()
        {
            if (this.ServerConfig.ClearSocketSession)
            {
                int interval = ServerConfig.ClearSocketSessionInterval*1000;
                _ClearSocketSessionTimer = new System.Threading.Timer(ClearSocketSession);
                _ClearSocketSessionTimer.Change(interval, interval);
            }
        }

        private void ClearSocketSession(object state)
        {
            if (Monitor.TryEnter(state))
            {
                try
                {
                    ICollection<IChannel> socketChannels = this.ChannelManager.GetChannels(CommunicateType.NET);

                    if (socketChannels == null || socketChannels.Count<=0)
                        return;

                    DateTime now = DateTime.Now;
  
                    IEnumerable<IChannel> timeoutSessions = socketChannels.Where(c => (now-((ISocketSession)c).LastActiveTime).Seconds>ServerConfig.ClearSocketSessionTimeOut);

                    System.Threading.Tasks.Parallel.ForEach(timeoutSessions, c =>
                    {
                       ISocketSession s = ((ISocketSession) c);
                       Logger.Info(true,String.Format("网络连接超时:{0}, 开始时间: {1}, 最后激活时间:{2}!", now.Subtract(s.LastActiveTime).TotalSeconds, s.StartTime, s.LastActiveTime));
                        RemoveTcpSocketSession(s);
                    });
                }
                catch (Exception ex)
                {
                    this.Logger.Error(true,ex.Message);
                }
                finally
                {
                    Monitor.Exit(state);
                }
            }
        }

        public override void Stop()
        {
            if (_SocketConnector != null)
            {
                _SocketConnector.NewClientConnected -= tcpSocketListener_NewClientAccepted;
                _SocketConnector.Stop();
            }

            if (_Listeners != null && _Listeners.Count > 0)
            {
                foreach (ISocketListener socketListener in _Listeners)
                {
                    socketListener.Stop();
                    socketListener.NewClientAccepted -= tcpSocketListener_NewClientAccepted;
                    socketListener.Error -= tcpSocketListener_Error;
                    socketListener.Stopped -= tcpSocketListener_Stopped;
                }
                _Listeners.Clear();
                _Listeners = null;
            }

            if (_SocketAsyncPool != null)
            {
                _SocketAsyncPool.Clear();
                _SocketAsyncPool = null;
            }

            _BufferManager = null;

            base.Stop();
        }

        private void tcpSocketListener_Stopped(object sender, EventArgs e)
        {
            Logger.Info(true, "网络侦听停止");
        }

        private void tcpSocketListener_Error(object sender, Exception e)
        {
            Logger.Info(true, e.Message);
        }

        private void tcpSocketListener_NewClientAccepted(object sender, System.Net.Sockets.Socket client, object state)
        {
            if (this.ServerConfig.SocketMode == SocketMode.Tcp)
            {
                if (this.ServerConfig.ControlMode == ControlMode.Loop
                    || this.ServerConfig.ControlMode == ControlMode.Self
                    || this.ServerConfig.ControlMode == ControlMode.Parallel)
                {
                    if (ServerConfig.CheckSameSocketSession)
                    {
                        string[] ipInfo = client.RemoteEndPoint.ToString().Split(':');
                        IChannel socketSession = ChannelManager.GetChannel(ipInfo[0], CommunicateType.NET);
                        if (socketSession != null)
                        {
                            RemoveTcpSocketSession((ISocketSession) socketSession);
                        }
                    }
                }

                AddTcpSocketSession(client);
            }
            else if (this.ServerConfig.SocketMode == SocketMode.Udp)
            {
                object[] arr = (object[])state;
                ISocketSession socketSession=new UdpSocketSession(client,(IPEndPoint)arr[1],null);
                Logger.Info(false, String.Format("远程UDP接收到数据>>{0}:{1}", socketSession.RemoteIP, socketSession.RemotePort));

                IList<IRequestInfo> ris=new List<IRequestInfo>();
                ris.Add(new RequestInfo(socketSession.Key, (byte[])arr[0],socketSession));
                IReceivePackage rp=new ReceivePackage(socketSession.RemoteIP,socketSession.RemotePort, ris);
                socketChannel_SocketReceiveData(socketSession, socketSession,rp);
            }
        }

        private void socketChannel_SocketReceiveData(object source, ISocketSession socketSession, IReceivePackage dataPackage)
        {
            ISocketController netController = (ISocketController)ControllerManager.GetController(SocketController.ConstantKey);
            if (netController != null)
            {
                netController.Receive(socketSession, dataPackage);
            }
            else
            {
                Logger.Info(false, SocketController.ConstantKey + ",无法找到对应的网络控制器");
            }
        }

        private void socketChannel_CloseSocket(object source, ISocketSession socketSession)
        {
            RemoveTcpSocketSession(socketSession);
        }

        private void AddTcpSocketSession(Socket client)
        {
            if (client == null)
                return;

            lock (ChannelManager.SyncLock)
            {
                ISocketAsyncEventArgsProxy socketProxy = this._SocketAsyncPool.Pop();
                if (socketProxy == null)
                {
                    AsyncUtil.AsyncRun(client.SafeClose);
                    Logger.Info(false, "已经到达最大连接数");
                    return;
                }

                ISocketSession socketSession = new TcpSocketSession(client,(IPEndPoint)client.RemoteEndPoint, socketProxy);
                socketSession.Setup(this);
                socketSession.Initialize();

                if (ChannelManager.AddChannel(socketSession.SessionID, socketSession))
                {
                    socketSession.CloseSocket += socketChannel_CloseSocket;
                    if (ServerConfig.ControlMode == ControlMode.Self
                        || ServerConfig.ControlMode == ControlMode.Parallel
                        || ServerConfig.ControlMode == ControlMode.Singleton)
                    {
                        socketSession.SocketReceiveData += socketChannel_SocketReceiveData;
                        AsyncUtil.AsyncRun(socketSession.TryReceive);
                    }

                    OnSocketConnected(socketSession.RemoteIP, socketSession.RemotePort);

                    OnChannelChanged(socketSession.RemoteIP, CommunicateType.NET, ChannelState.Open);

                    Logger.Info(false, String.Format("增加远程连接>>{0}:{1} 成功", socketSession.RemoteIP, socketSession.RemotePort));
                }
                else
                {
                    ISocketAsyncEventArgsProxy proxy = socketSession.SocketAsyncProxy;
                    proxy.Reset();
                    if (proxy.SocketReceiveEventArgsEx.InitOffset != proxy.SocketReceiveEventArgsEx.Offset)
                    {
                        proxy.SocketReceiveEventArgsEx.SetBuffer(proxy.SocketReceiveEventArgsEx.InitOffset, ServerConfig.NetReceiveBufferSize);
                    }
                    _SocketAsyncPool.Push(proxy);
                    socketSession.Close();
                    socketSession = null;
                    Logger.Info(true, String.Format("增加远程连接>>{0}:{1} 失败", socketSession.RemoteIP, socketSession.RemotePort));
                }
            }
        }

        private void RemoveTcpSocketSession(ISocketSession socketSession)
        {
            if (socketSession == null)
                return;

            lock (ChannelManager.SyncLock)
            {
                if (ChannelManager.ContainChannel(socketSession.SessionID))
                {
                    string ip = socketSession.RemoteIP;
                    int port = socketSession.RemotePort;
                    if (ChannelManager.RemoveChannel(socketSession.SessionID))
                    {
                        ISocketAsyncEventArgsProxy proxy = socketSession.SocketAsyncProxy;

                        if (proxy.SocketReceiveEventArgsEx.InitOffset != proxy.SocketReceiveEventArgsEx.Offset)
                        {
                            proxy.SocketReceiveEventArgsEx.SetBuffer(proxy.SocketReceiveEventArgsEx.InitOffset, ServerConfig.NetReceiveBufferSize);
                        }

                        _SocketAsyncPool.Push(proxy);

                        socketSession.Close();

                        OnSocketClosed(socketSession.RemoteIP, socketSession.RemotePort);

                        OnChannelChanged(socketSession.RemoteIP, CommunicateType.NET, ChannelState.Close);

                        socketSession = null;

                        Logger.Info(false, String.Format("远程连接断开>>{0}:{1} 成功", ip, port));
                    }
                    else
                    {
                        Logger.Info(true, String.Format("远程连接断开>>{0}:{1} 失败", ip, port));
                    }
                }
            }
        }
    }
}
