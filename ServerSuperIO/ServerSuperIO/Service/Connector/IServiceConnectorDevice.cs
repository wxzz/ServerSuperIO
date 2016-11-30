using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerSuperIO.Service.Connector
{
    public interface IServiceConnectorDevice
    {
        /// <summary>
        /// 支行设备连接器
        /// </summary>
        /// <param name="fromService"></param>
        /// <param name="toDevice"></param>
        /// <returns></returns>
        object RunServiceConnector(IFromService fromService, IServiceToDevice toDevice);
    }
}
