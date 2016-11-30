using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerSuperIO.Device.Connector
{
    public class FromDevice:IFromDevice
    {
        public FromDevice(string deviceID, string deviceCode, int deviceAddr, string deviceName)
        {
            DeviceID = deviceID;
            DeviceCode = deviceCode;
            DeviceAddr = deviceAddr;
            DeviceName = deviceName;
        }

        public string DeviceID { get; private set; }
        public string DeviceCode { get; private set; }
        public int DeviceAddr { get; private set; }
        public string DeviceName { get; private set; }
    }
}
