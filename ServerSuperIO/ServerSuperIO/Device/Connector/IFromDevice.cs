using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerSuperIO.Device.Connector
{
    /// <summary>
    /// 来自传递的设备
    /// </summary>
    public interface IFromDevice
    {
        string DeviceID { get; }

        string DeviceCode { get; }

        int DeviceAddr { get; }

        string DeviceName { get; }
    }
}
