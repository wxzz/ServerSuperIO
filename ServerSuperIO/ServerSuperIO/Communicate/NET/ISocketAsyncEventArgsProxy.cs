using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace ServerSuperIO.Communicate.NET
{
    public interface ISocketAsyncEventArgsProxy
    {
        SocketAsyncEventArgsEx SocketReceiveEventArgsEx { get; set; }

        SocketAsyncEventArgs SocketSendEventArgs { get; set; }

        void Initialize(ISocketSession session);

        void Reset();
    }
}
