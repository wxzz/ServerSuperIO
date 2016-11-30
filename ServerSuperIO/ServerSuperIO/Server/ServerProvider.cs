using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerSuperIO.Server
{
    public abstract class ServerProvider:IServerProvider
    {
        protected ServerProvider()
        {
            Server = null;
        }

        public IServer Server { get; private set; }

        public virtual void Setup(IServer appServer)
        {
            Server = appServer;
        }
    }
}
