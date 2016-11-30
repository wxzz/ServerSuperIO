using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using ServerSuperIO.Config;

namespace ServerSuperIO.Communicate.NET
{
    public interface ISocketListener
    {
        IPEndPoint EndPoint { get; }

        ListenerInfo ListenerInfo { get; }

        bool Start(IServerConfig config);

        void Stop();

        event NewClientAcceptHandler NewClientAccepted;

        event ErrorHandler Error;

        event EventHandler Stopped;
    }
}
