using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerSuperIO.Service.Connector
{
    public delegate void ServiceConnectorHandler(object source, ServiceConnectorArgs args);

    public class ServiceConnectorArgs : EventArgs
    {
        public ServiceConnectorArgs(IFromService fromDevice, IServiceToDevice toDevice)
        {
            FromService = fromDevice;
            ServiceToDevice = toDevice;
        }

        public IFromService FromService { get; private set; }

        public IServiceToDevice ServiceToDevice { get; private set; }
    }
}
