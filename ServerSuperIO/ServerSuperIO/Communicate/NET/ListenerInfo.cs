using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace ServerSuperIO.Communicate.NET
{
    public class ListenerInfo
    {
        public IPEndPoint EndPoint { get; set; }

        public int BackLog { get; set; }
    }
}
