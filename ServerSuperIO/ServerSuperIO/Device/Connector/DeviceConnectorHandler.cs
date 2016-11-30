using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerSuperIO.Device.Connector
{
    public delegate void DeviceConnectorHandler(object source, DeviceConnectorArgs args);

    public class DeviceConnectorArgs:EventArgs
    {
        public DeviceConnectorArgs(IFromDevice fromDevice, IDeviceToDevice toDevice)
        {
            FromDevice = fromDevice;
            DeviceToDevice = toDevice;
        }

        public IFromDevice FromDevice { get; private set; }

        public IDeviceToDevice DeviceToDevice { get; private set; }
    }
}
