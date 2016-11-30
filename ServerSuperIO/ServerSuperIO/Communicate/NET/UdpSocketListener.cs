using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using ServerSuperIO.Config;

namespace ServerSuperIO.Communicate.NET
{
    public class UdpSocketListener : SocketListenerBase
    {
        private Socket _ListenSocket;

        private SocketAsyncEventArgs _AcceptSAE;

        public UdpSocketListener(ListenerInfo info)
            : base(info)
        {

        }

        public override bool Start(IServerConfig config)
        {
            try
            {
                _ListenSocket = new Socket(this.EndPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
                _ListenSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                _ListenSocket.Bind(this.EndPoint);

                _AcceptSAE = new SocketAsyncEventArgs();

                _AcceptSAE.Completed += new EventHandler<SocketAsyncEventArgs>(eventArgs_Completed);
                _AcceptSAE.RemoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

                int receiveBufferSize = config.NetReceiveBufferSize <= 0 ? 2048 : config.NetReceiveBufferSize;
                var buffer = new byte[receiveBufferSize];
                _AcceptSAE.SetBuffer(buffer, 0, buffer.Length);

                _ListenSocket.ReceiveFromAsync(_AcceptSAE);

                return true;
            }
            catch (Exception e)
            {
                OnError(e);
                return false;
            }
        }

        void eventArgs_Completed(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success)
            {
                var errorCode = (int)e.SocketError;

                //The listen socket was closed
                if (errorCode == 995 || errorCode == 10004 || errorCode == 10038)
                    return;

                OnError(new SocketException(errorCode));
            }

            if (e.LastOperation == SocketAsyncOperation.ReceiveFrom)
            {
                try
                {
                    byte[] revData=new byte[e.BytesTransferred];
                    Buffer.BlockCopy(e.Buffer,e.Offset,revData,0,revData.Length);

                    OnNewClientAcceptedAsync(_ListenSocket, new object[]{revData,e.RemoteEndPoint});
                }
                catch (Exception exc)
                {
                    OnError(exc);
                }

                try
                {
                    _ListenSocket.ReceiveFromAsync(e);
                }
                catch (Exception exc)
                {
                    OnError(exc);
                }
            }
        }

        public override void Stop()
        {
            if (_ListenSocket == null)
                return;

            lock (this)
            {
                if (_ListenSocket == null)
                    return;

                _AcceptSAE.Completed -= new EventHandler<SocketAsyncEventArgs>(eventArgs_Completed);
                _AcceptSAE.Dispose();
                _AcceptSAE = null;

                try
                {
                    _ListenSocket.Shutdown(SocketShutdown.Both);
                }
                catch { }

                try
                {
                    _ListenSocket.Close();
                }
                catch { }

                _ListenSocket = null;
            }

            OnStopped();
        }
    }
}
