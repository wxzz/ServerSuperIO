using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerSuperIO.Device.Connector
{
    public interface IDeviceConnector
    {
        /// <summary>
        /// 支行设备连接器
        /// </summary>
        /// <param name="fromDevice">信息传递的发送端</param>
        /// <param name="toDevice">信息传递的目的端，以及包含的数据信息</param>
        /// <returns></returns>
        object RunDeviceConnector(IFromDevice fromDevice, IDeviceToDevice toDevice);

        /// <summary>
        /// 设备连接器回调函数，在这里写回调的处理结果
        /// </summary>
        /// <param name="obj"></param>
        void DeviceConnectorCallback(object obj);

        /// <summary>
        /// 如果执行方出现异常，则返回给这个函数结果
        /// </summary>
        /// <param name="ex"></param>
        void DeviceConnectorCallbackError(Exception ex);

        /// <summary>
        /// 连接器事件，发起端
        /// </summary>
        event DeviceConnectorHandler DeviceConnector;

        /// <summary>
        /// 确发事件接口
        /// </summary>
        /// <param name="fromDevice"></param>
        /// <param name="toDevice"></param>
        void OnDeviceConnector(IFromDevice fromDevice, IDeviceToDevice toDevice);
    }
}
