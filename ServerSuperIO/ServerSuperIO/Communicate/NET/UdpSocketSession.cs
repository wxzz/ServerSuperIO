using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServerSuperIO.Common;
using ServerSuperIO.Protocol;
using ServerSuperIO.Protocol.Filter;
using ServerSuperIO.Server;

namespace ServerSuperIO.Communicate.NET
{
    public class UdpSocketSession : SocketSession
    {
        public UdpSocketSession(Socket socket, IPEndPoint remoteEndPoint, ISocketAsyncEventArgsProxy proxy):base(socket,remoteEndPoint,proxy)
        {
            
        }

        public override void TryReceive()
        {
           return;
        }

        protected override void SendAsync(byte[] data)
        {
            SocketAsyncEventArgs e = new SocketAsyncEventArgs();

            e.Completed += new EventHandler<SocketAsyncEventArgs>(SocketEventArgs_Completed);
            e.RemoteEndPoint = RemoteEndPoint;
            e.UserToken = data;

            e.SetBuffer(data,0, data.Length);

            if (Client != null)
            {
                if (!Client.SendToAsync(e))
                {
                    ProcessSend(e);
                }
            }
        }

        private void ProcessSend(SocketAsyncEventArgs e)
        {
            try
            {
                if (e.SocketError == SocketError.Success)
                {
                    byte[] data = (byte[])e.UserToken;

                    if (e.BytesTransferred < data.Length)
                    {
                        e.SetBuffer(data, e.BytesTransferred, data.Length - e.BytesTransferred);
                        bool willRaiseEvent = this.Client.SendToAsync(e);
                        if (!willRaiseEvent)
                        {
                            ProcessSend(e);
                        }
                    }
                    else
                    {
                        CleanAsyncEventArgs(e);
                    }
                }
                else
                {
                    CleanAsyncEventArgs(e);
                    OnCloseSocket();
                }
            }
            catch (SocketException ex)
            {
                CleanAsyncEventArgs(e);
                OnCloseSocket();
                this.Server.Logger.Error(true, ex.Message);
            }
            catch (Exception ex)
            {
                this.Server.Logger.Error(true, ex.Message);
            }
        }

        protected override void SendSync(byte[] data)
        {
            this.Client.SendDataTo(data, 0, data.Length, this.RemoteEndPoint);
        }

        public override void Initialize()
        {
            return;
        }

        protected override void SocketEventArgs_Completed(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.SendTo:
                    ProcessSend(e);
                    break;
                default:
                    this.Server.Logger.Info(false, "不支持接收和发送的操作");
                    break;
            }
        }

        void CleanAsyncEventArgs(SocketAsyncEventArgs e)
        {
            e.UserToken = null;
            e.Completed -= new EventHandler<SocketAsyncEventArgs>(SocketEventArgs_Completed);
            e.Dispose();
        }

        protected override Task<byte[]> ReadAsync(int dataLength, CancellationTokenSource cts)
        {
            return new Task<byte[]>(() => new byte[]{});
        }

        public override IList<byte[]> Read(IReceiveFilter receiveFilter)
        {
            return new List<byte[]>();
        }

        public override int Write(byte[] data)
        {
            return this.Client.SendDataTo(data, 0,data.Length, this.RemoteEndPoint);
        }
    }
}
