using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerSuperIO.Config;
using ServerSuperIO.Device;
using ServerSuperIO.Log;

namespace ServerSuperIO.Server
{
    public interface IServerManager:IEnumerable
    {
        IServer this[int index] { get; }

        IServer CreateServer(IServerConfig config, IDeviceContainer deviceContainer = null, ILogContainer logContainer = null);

        void AddServer(IServer server);

        IServer GetServer(string serverName);

        bool ContainsServer(string serverName);

        IServer[] GetServers();

        void RemoveServer(string serverName);

        void RemoveAllServer();

        int Count { get; }
    }
}
