using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerSuperIO.Device.Connector
{
    public class DeviceToDevice:IDeviceToDevice
    {
        public DeviceToDevice(string deviceCode, string text, byte[] dataBytes, object obj)
        {
            DeviceCode = deviceCode;
            Text = text;
            DataBytes = dataBytes;
            Object = obj;
        }

        public string DeviceCode { get; private set; }
        public string Text { get; private set; }
        public byte[] DataBytes { get; private set; }
        public object Object { get; private set; }
    }
}
