using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using ServerSuperIO.Common;
using ServerSuperIO.Config;

namespace ServerSuperIO.Communicate.NET
{
    public class TcpSocketListener : SocketListenerBase
    {
        private int _ListenBackLog;//侦听队列数

        private Socket _ListenSocket;//侦听Socket

        private SocketAsyncEventArgs _AcceptSAE;
        
        public TcpSocketListener(ListenerInfo info) : base(info)
        {
            _ListenBackLog = info.BackLog;
        }

        public override bool Start(IServerConfig config)
        {
            _ListenSocket = new Socket(this.ListenerInfo.EndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                _ListenSocket.Bind(this.ListenerInfo.EndPoint);
                _ListenSocket.Listen(_ListenBackLog);

                _ListenSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                _ListenSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);

                _AcceptSAE = new SocketAsyncEventArgs();
                _AcceptSAE.Completed += new EventHandler<SocketAsyncEventArgs>(AcceptEventArg_Completed);

                if (!_ListenSocket.AcceptAsync(_AcceptSAE))
                    ProcessAccept(_AcceptSAE);

                return true;

            }
            catch (Exception e)
            {
                OnError(e);
                return false;
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

                if (_AcceptSAE != null)
                {
                    _AcceptSAE.Completed -= new EventHandler<SocketAsyncEventArgs>(AcceptEventArg_Completed);
                    _AcceptSAE.Dispose();
                    _AcceptSAE = null;
                }

                try
                {
                    _ListenSocket.SafeClose();
                }
                finally
                {
                    _ListenSocket = null;
                }
            }
        }

        private void AcceptEventArg_Completed(object sender, SocketAsyncEventArgs e)
        {
            ProcessAccept(e);
        }

        private void ProcessAccept(SocketAsyncEventArgs e)
        {
            Socket socket = null;

            if (e.SocketError != SocketError.Success)
            {
                var errorCode = (int)e.SocketError;

                if (errorCode == 995 || errorCode == 10004 || errorCode == 10038)
                    return;

                OnError(new SocketException(errorCode));
            }
            else
            {
                socket = e.AcceptSocket;
            }

            e.AcceptSocket = null;

            bool willRaiseEvent = false;

            try
            {
                willRaiseEvent = _ListenSocket.AcceptAsync(e);
            }
            catch (ObjectDisposedException)
            {
                //The listener was stopped
                //Do nothing
                //make sure ProcessAccept won't be executed in this thread
                willRaiseEvent = true;
            }
            catch (NullReferenceException)
            {
                //The listener was stopped
                //Do nothing
                //make sure ProcessAccept won't be executed in this thread
                willRaiseEvent = true;
            }
            catch (Exception exc)
            {
                OnError(exc);
                //make sure ProcessAccept won't be executed in this thread
                willRaiseEvent = true;
            }

            if (socket != null)
                OnNewClientAccepted(socket, null);

            if (!willRaiseEvent)
                ProcessAccept(e);
        }
    }
}
