using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using ServerSuperIO.Config;

namespace ServerSuperIO.Communicate.NET
{
    public abstract class SocketListenerBase : ISocketListener
    {
        public IPEndPoint EndPoint
        {
            get { return ListenerInfo.EndPoint; }
        }

        public ListenerInfo ListenerInfo { get; private set; }

        protected SocketListenerBase(ListenerInfo info)
        {
            ListenerInfo = info;
        }

        /// <summary>
        /// Starts to listen
        /// </summary>
        /// <param name="config">The server config.</param>
        /// <returns></returns>
        public abstract bool Start(IServerConfig config);

        public abstract void Stop();

        public event NewClientAcceptHandler NewClientAccepted;

        public event ErrorHandler Error;

        protected void OnError(Exception e)
        {
            if (Error != null)
                Error(this, e);
        }

        protected void OnError(string errorMessage)
        {
            OnError(new Exception(errorMessage));
        }

        protected virtual void OnNewClientAccepted(Socket socket, object state)
        {
            if (NewClientAccepted != null)
                NewClientAccepted(this, socket, state);
        }

        protected void OnNewClientAcceptedAsync(Socket socket, object state)
        {
            if (NewClientAccepted != null)
                NewClientAccepted.BeginInvoke(this, socket, state, null, null);
        }

        public event EventHandler Stopped;

        protected void OnStopped()
        {
            if (Stopped != null)
                Stopped(this, EventArgs.Empty);
        }
    }
}
