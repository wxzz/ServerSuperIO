using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerSuperIO.Base;
using ServerSuperIO.Service.Connector;

namespace ServerSuperIO.Service
{
    /// <summary>
    ///     这个接口的UpdateDevice与设备的DeviceObjectChangedHandler事件和OnDeviceObjectChangedHandler函数关联
    /// </summary>
    public interface IService : IServiceConnector,IPlugin
    {
        /// <summary>
        ///     服务Key,要求唯一
        /// </summary>
        string ServiceKey { get; }

        /// <summary>
        ///     服务名称
        /// </summary>
        string ServiceName { get;}


        /// <summary>
        /// 是否自动起动
        /// </summary>
        bool IsAutoStart { get; set; }

        /// <summary>
        /// 单击事件
        /// </summary>
        void OnClick();

        /// <summary>
        ///     更新设备
        /// </summary>
        /// <param name="devCode">设备</param>
        /// <param name="obj">设备对象</param>
        void UpdateDevice(string devCode, object obj);

        /// <summary>
        ///     移除设备
        /// </summary>
        /// <param name="devcode">设备ID</param>
        void RemoveDevice(string devCode);

        /// <summary>
        ///     启动服务
        /// </summary>
        void StartService();

        /// <summary>
        ///     释放服务
        /// </summary>
        void StopService();

        /// <summary>
        /// 服务日志
        /// </summary>
        event ServiceLogHandler ServiceLog;

        /// <summary>
        /// 是否被释放
        /// </summary>
        bool IsDisposed { get; }
    }
}
