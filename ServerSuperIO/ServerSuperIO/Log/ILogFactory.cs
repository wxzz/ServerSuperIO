using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerSuperIO.Log
{
    public interface ILogFactory
    {
        ILog GetLog(string name, ILogContainer logContainer);
    }
}
