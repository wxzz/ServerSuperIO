using System;
using ServerSuperIO.Device;

namespace ServerSuperIO.Communicate.NET
{
    [Serializable]
    public class SocketParameter
    {
        public SocketParameter()
        {
            RemoteIP = "127.0.0.1";
            RemotePort = 6688;
            WorkMode = WorkMode.TcpServer;
        }

        public SocketParameter(string remoteIP, int remotePort, WorkMode workMode)
        {
            RemoteIP = remoteIP;
            RemotePort = remotePort;
            WorkMode = workMode;
        }

        public string RemoteIP { get; set; }

        public int RemotePort { get; set; }

        public WorkMode WorkMode { get; set; }
    }
}
