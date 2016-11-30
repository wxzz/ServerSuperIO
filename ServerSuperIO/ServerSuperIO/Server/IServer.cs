using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Forms;
using ServerSuperIO.Communicate;
using ServerSuperIO.Communicate.COM;
using ServerSuperIO.Communicate.NET;
using ServerSuperIO.Config;
using ServerSuperIO.Device;
using ServerSuperIO.Log;
using ServerSuperIO.Service;
using ServerSuperIO.Show;

namespace ServerSuperIO.Server
{
    public interface IServer:IDisposable,ILoggerProvider,IServerConfigProvider
    {
        #region 函数
        /// <summary>
        /// 服务名称
        /// </summary>
        string ServerName { get; }

        /// <summary>
        /// 开始服务
        /// </summary>
        void Start();

        /// <summary>
        /// 停止服务
        /// </summary>
        void Stop();

     /// <summary>
     /// 
     /// </summary>
     /// <param name="dev"></param>
     /// <returns>返回设备ID</returns>
        string AddDevice(IRunDevice dev);

        /// <summary>
        /// 删除设备
        /// </summary>
        /// <param name="devid"></param>
        void RemoveDevice(string devid);

        /// <summary>
        /// 增加数据显示视图
        /// </summary>
        /// <param name="graphicsShow"></param>
        /// <param name="window"></param>
        /// <returns></returns>
        bool AddGraphicsShow(IGraphicsShow graphicsShow,IWin32Window window);

        /// <summary>
        /// 删除显示
        /// </summary>
        /// <param name="showKey"></param>
        /// <returns></returns>
        bool RemoveGraphicsShow(string showKey);

        /// <summary>
        /// 增加服务
        /// </summary>
        /// <param name="service"></param>
        bool AddService(IService service);

        /// <summary>
        /// 删除服务
        /// </summary>
        /// <param name="serviceKey"></param>
        /// <returns></returns>
        bool RemoveService(string serviceKey);

        /// <summary>
        /// 改变设备的串口信息
        /// </summary>
        /// <param name="devid"></param>
        /// <param name="oldCom"></param>
        /// <param name="oldBaud"></param>
        /// <param name="newCom"></param>
        /// <param name="newBaud"></param>
       
        void ChangeDeviceComInfo(string devid, int oldCom, int oldBaud, int newCom, int newBaud);
        #endregion

        #region 属性
        /// <summary>
        /// 设备管理器
        /// </summary>
        IDeviceManager<string, IRunDevice> DeviceManager { get; }

        /// <summary>
        /// 通道管理器
        /// </summary>
        IChannelManager<string, IChannel> ChannelManager { get; }

        /// <summary>
        /// 控制管理器
        /// </summary>
        IControllerManager<string, IController> ControllerManager { get; }

        /// <summary>
        /// 服务管理
        /// </summary>
        IServiceManager<string,IService> ServiceManager { get; }

        /// <summary>
        /// 显示管理
        /// </summary>
        IGraphicsShowManager<string, IGraphicsShow> GraphicsShowManager { get; }

        #endregion

        #region 事件
        /// <summary>
        /// 增加设备完成事件
        /// </summary>
        event AddDeviceCompletedHandler AddDeviceCompleted;

        /// <summary>
        /// 删除设备完成事件
        /// </summary>
        event DeleteDeviceCompletedHandler DeleteDeviceCompleted;

        ///// <summary>
        ///// 串口打开事件
        ///// </summary>
        event ComOpenedHandler ComOpened;

        ///// <summary>
        ///// 串口打开事件
        ///// </summary>
        event ComClosedHandler ComClosed;

        ///// <summary>
        ///// 串口异常
        ///// </summary>
        event SocketConnectedHandler SocketConnected;

        /// <summary>
        /// 关闭Socket
        /// </summary>
        event SocketClosedHandler SocketClosed;
        #endregion
    }
}
