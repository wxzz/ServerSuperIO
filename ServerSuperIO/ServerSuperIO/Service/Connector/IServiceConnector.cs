using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerSuperIO.Service.Connector
{
    public interface IServiceConnector
    {
        /// <summary>
        /// 设备连接器回调函数，在这里写回调的处理结果
        /// </summary>
        /// <param name="obj"></param>
        void ServiceConnectorCallback(object obj);

        /// <summary>
        /// 如果执行方出现异常，则返回给这个函数结果
        /// </summary>
        /// <param name="ex"></param>
        void ServiceConnectorCallbackError(Exception ex);

        /// <summary>
        /// 连接器事件，发起端
        /// </summary>
        event ServiceConnectorHandler ServiceConnector;

        /// <summary>
        /// 确发事件接口
        /// </summary>
        /// <param name="fromService"></param>
        /// <param name="toDevice"></param>
        void OnServiceConnector(IFromService fromService, IServiceToDevice toDevice);
    }
}
