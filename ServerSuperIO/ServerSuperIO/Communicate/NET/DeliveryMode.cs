using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerSuperIO.Common;

namespace ServerSuperIO.Communicate.NET
{ /// <summary>
  /// 分发数据模式
  /// </summary>
    public enum DeliveryMode
    {
        [EnumDescription("设备IP分发数据")]
        DeviceIP,
        [EnumDescription("设备编码分发数据")]
        DeviceCode
    }
}
