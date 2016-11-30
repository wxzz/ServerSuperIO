using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerSuperIO.Device.Connector
{
    public class DeviceConnectorToken
    {
        public DeviceConnectorToken(string fromDeviceID, Func<IFromDevice, IDeviceToDevice, object> deviceConnectorAsync)
        {
            FromDeviceID = fromDeviceID;
            DeviceConnectorAsync = deviceConnectorAsync;
        }

        public Func<IFromDevice, IDeviceToDevice, object> DeviceConnectorAsync { get; private set; }

        public string FromDeviceID { get; private set; }
    }
}
