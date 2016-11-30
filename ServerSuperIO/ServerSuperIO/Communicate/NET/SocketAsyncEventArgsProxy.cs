using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace ServerSuperIO.Communicate.NET
{
    public class SocketAsyncEventArgsProxy:ISocketAsyncEventArgsProxy
    {
        public SocketAsyncEventArgsProxy(SocketAsyncEventArgsEx saea)
        {
            SocketReceiveEventArgsEx = saea;
            SocketSendEventArgs=new SocketAsyncEventArgs();
        }

        public SocketAsyncEventArgsEx SocketReceiveEventArgsEx { get; set; }

        public SocketAsyncEventArgs SocketSendEventArgs { get; set; }

        public void Initialize(ISocketSession session)
        {
            SocketReceiveEventArgsEx.UserToken = session;
            SocketSendEventArgs.UserToken = session;
        }

        public void Reset()
        {
            SocketReceiveEventArgsEx.UserToken = null;
            SocketSendEventArgs.UserToken = null;
        }
    }
}
