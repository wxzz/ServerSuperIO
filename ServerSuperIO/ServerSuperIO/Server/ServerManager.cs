using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ServerSuperIO.Config;
using ServerSuperIO.Device;
using ServerSuperIO.Log;

namespace ServerSuperIO.Server
{
    public class ServerManager : IServerManager
    {
        private IList<IServer> _Servers;
        public ServerManager()
        {
            _Servers=new List<IServer>();
        }

        public IServer this[int index]
        {
            get
            {
                if (index >= 0 && index <= _Servers.Count - 1)
                {
                    return _Servers[index];
                }
                else
                {
                    return null;
                }
            }
        }

        public IServer CreateServer(IServerConfig config, IDeviceContainer deviceContainer = null, ILogContainer logContainer = null)
        {
            try
            {
                return new Server(config,deviceContainer,logContainer);
            }
            catch
            {
                throw;
            }
        }

        public void AddServer(IServer server)
        {
            if (_Servers.FirstOrDefault(s => s.ServerName == server.ServerName) == null)
            {
                _Servers.Add(server);
            }
            else
            {
                throw new EqualException("ServerName已经存在");
            }
        }

        public IServer GetServer(string serverName)
        {
            return _Servers.FirstOrDefault(s => s.ServerName == serverName);
        }

        public bool ContainsServer(string serverName)
        {
            if (GetServer(serverName) == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public IServer[] GetServers()
        {
            return _Servers.ToArray();
        }

        public void RemoveServer(string serverName)
        {
            IServer server=_Servers.FirstOrDefault(s => s.ServerName == serverName);
            if (server != null)
            {
                try
                {
                    server.Stop();
                    server.Dispose();
                }
                finally
                {
                    _Servers.Remove(server);
                }
            }
        }

        public void RemoveAllServer()
        {
            Parallel.ForEach(_Servers.ToArray(), s =>
            {
                try
                {
                    s.Stop();
                }
                catch
                {
                    // ignored
                }
            });

            this._Servers.Clear();
        }

        public int Count
        {
            get
            {
                return _Servers.Count;
            }
        }

        public IEnumerator GetEnumerator()
        {
            return _Servers.GetEnumerator();
        }
    }
}
