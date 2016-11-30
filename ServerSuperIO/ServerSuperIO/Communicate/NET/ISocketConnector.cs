using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerSuperIO.Server;

namespace ServerSuperIO.Communicate.NET
{
    internal interface ISocketConnector : IServerProvider, IDisposable
    {
        void Start();

        void Stop();

        event NewClientAcceptHandler NewClientConnected;

        event ErrorHandler Error;
    }
}
