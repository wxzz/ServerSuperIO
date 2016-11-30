using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using ServerSuperIO.Common;
using ServerSuperIO.Protocol;
using ServerSuperIO.Server;

namespace ServerSuperIO.Communicate.NET
{
    public abstract class SocketSession : BaseChannel,ISocketSession
    {
        private bool _IsDisposed = false;
        protected SocketSession(Socket socket, IPEndPoint remoteEndPoint, ISocketAsyncEventArgsProxy proxy)
        {
            SessionID = Guid.NewGuid().ToString();

            string[] temp = remoteEndPoint.ToString().Split(':');
            if (temp.Length >= 2)
            {
                RemoteIP = temp[0];
                RemotePort = Convert.ToInt32(temp[1]);
            }
            RemoteEndPoint = remoteEndPoint;
            Client = socket;
            SocketAsyncProxy = proxy;

            StartTime=DateTime.Now;
            LastActiveTime = StartTime;
        }

        ~SocketSession()
        {
            Dispose(false);
        }

        ///// <summary>
        ///// 同步对象
        ///// </summary>
        //private object _SyncLock = new object();

        /// <summary>
        /// 远程IP
        /// </summary>
        public string RemoteIP { get; private set; }

        /// <summary>
        /// 远程端口
        /// </summary>
        public int RemotePort { get; private set; }

        /// <summary>
        /// Socket实例
        /// </summary>
        public Socket Client { get; private set; }

        /// <summary>
        /// 远程点
        /// </summary>
        public IPEndPoint RemoteEndPoint { get; private set; }

        /// <summary>
        /// 开始时间 
        /// </summary>
        public DateTime StartTime { get; private set; }

        /// <summary>
        /// 最后接收有效数据的时间。
        /// </summary>
        public DateTime LastActiveTime { get; set; }

        /// <summary>
        /// 代理实例
        /// </summary>
        public ISocketAsyncEventArgsProxy SocketAsyncProxy { get; private set; }

        public event CloseSocketHandler CloseSocket;

        protected virtual void OnCloseSocket()
        {
            if (CloseSocket != null)
            {
                CloseSocket(this, this);
            }
        }

        public event SocketReceiveDataHandler SocketReceiveData;

        protected virtual void OnSocketReceiveData(IReceivePackage dataPackage)
        {
            if (SocketReceiveData != null)
            {
                SocketReceiveData(this, this, dataPackage);
            }
        }

        public abstract void TryReceive();

        public void TrySend(byte[] data, bool type)
        {
            if (type)
            {
                SendAsync(data);
            }
            else
            {
                SendSync(data);
            }
        }

        /// <summary>
        /// 异步发送
        /// </summary>
        /// <param name="data"></param>
        protected abstract void SendAsync(byte[] data);
        /// <summary>
        /// 同步发送
        /// </summary>
        /// <param name="data"></param>
        protected abstract void SendSync(byte[] data);
        /// <summary>
        /// 初始化
        /// </summary>
        //public abstract void Initialize();

        /// <summary>
        /// 事件完成接口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected abstract void SocketEventArgs_Completed(object sender, SocketAsyncEventArgs e);

        //public object SyncLock
        //{
        //    get { return _SyncLock; }
        //}


        /// <summary>
        /// 关键字
        /// </summary>
        public override string Key
        {
            get { return this.RemoteIP; }
        }

        /// <summary>
        /// 唯一ID
        /// </summary>
        public override string SessionID { get; protected set; }

        /// <summary>
        /// 通道实例
        /// </summary>
        public override IChannel Channel
        {
            get { return (IChannel)this; }
        }

        //public abstract byte[] Read();

        //public abstract IList<byte[]> Read(IReceiveFilter receiveFilter);

        //public abstract int Write(byte[] data);

        public override void Close()
        {
            Dispose(true);
        }

        public override CommunicateType CommunicationType
        {
            get { return CommunicateType.NET; }
        }

        public override bool IsDisposed
        {
            get { return _IsDisposed; }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._IsDisposed)
            {
                if (disposing)
                {
                    Client.SafeClose();
                    Client.Dispose();
                    Client = null;

                    SocketAsyncProxy.Reset();
                }

                _IsDisposed = true;
            }
        }
    }
}
