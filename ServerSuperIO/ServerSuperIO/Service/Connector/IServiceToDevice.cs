using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerSuperIO.Service.Connector
{
    /// <summary>
    /// 发送给指定的设备
    /// </summary>
    public interface IServiceToDevice
    {
        string DeviceCode { get; }

        string Text { get; }

        byte[] DataBytes { get; }

        object Object { get; }
    }
}
