using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerSuperIO.Common;
using ServerSuperIO.Communicate;
using ServerSuperIO.Config;
using ServerSuperIO.Device;
using ServerSuperIO.Log;

namespace ServerSuperIO.Server
{
    public abstract class AppServerBase:IServer
    {
        private MonitorException _monitorException;

        protected AppServerBase(string serverName,IConfig config)
        {
            if (String.IsNullOrEmpty(serverName) || config == null)
            {
                throw new ArgumentNullException("服务名称或配制信息","参数为空");
            }

            _monitorException=new MonitorException();

            IsDisposed = false;

            ServerName = serverName;
            Config = config;

            Logger = (new LogFactory()).GetLog(serverName);
            DeviceManager=new DeviceManager();
            ChannelManager=new ChannelManager();
            ControllerManager=new ControllerManager();
        }

        ~AppServerBase()
        {
            Dispose(false);
        }

        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServerName { get; private set; }

        /// <summary>
        /// 开始服务
        /// </summary>
        public virtual void Start()
        {
            _monitorException.Setup(this);
            _monitorException.Monitor();
            Logger.InfoFormat(false,"{0}-{1}",ServerName,"启动服务");
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        public virtual void Stop()
        {
            Logger.InfoFormat(false, "{0}-{1}", ServerName, "停止服务...");
            Dispose(true);
        }

        /// <summary>
        /// 增加设备
        /// </summary>
        /// <param name="dev"></param>
        /// <returns>设备ID</returns>
        public abstract void AddDevice(Device.IRunDevice dev);

        /// <summary>
        /// 删除设备
        /// </summary>
        /// <param name="devid"></param>
        /// <returns></returns>
        public abstract void RemoveDevice(int devid);

        /// <summary>
        /// 增加显示视图
        /// </summary>
        /// <param name="graphicsShow"></param>
        /// <returns></returns>
        public abstract bool AddGraphicsShow(Show.IGraphicsShow graphicsShow);

        /// <summary>
        /// 删除显示
        /// </summary>
        /// <param name="showKey"></param>
        /// <returns></returns>
        public abstract bool RemoveGraphicsShow(string showKey);

        /// <summary>
        /// 增加服务实例
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public abstract bool AddAppService(Service.IAppService service);

        /// <summary>
        /// 删除服务
        /// </summary>
        /// <param name="serviceKey"></param>
        /// <returns></returns>
        public abstract bool RemoveAppService(string serviceKey);

        /// <summary>
        /// 改变设备的串口信息
        /// </summary>
        /// <param name="devid"></param>
        /// <param name="oldCom"></param>
        /// <param name="oldBaud"></param>
        /// <param name="newCom"></param>
        /// <param name="newBaud"></param>
        public abstract void ChangeDeviceComInfo(int devid, int oldCom, int oldBaud, int newCom, int newBaud);

        /// <summary>
        /// 设备管理器
        /// </summary>
        public Device.IDeviceManager<int, Device.IRunDevice> DeviceManager { get; private set; }

        /// <summary>
        /// IO通道管理器
        /// </summary>
        public Communicate.IChannelManager<string, Communicate.IChannel> ChannelManager { get; private set; }

        /// <summary>
        /// 控制管理器
        /// </summary>
        public Communicate.IControllerManager<string, Communicate.IController> ControllerManager { get; private set; }

        /// <summary>
        /// 增加设备完成事件
        /// </summary>
        public event AddDeviceCompletedHandler AddDeviceCompleted;

        /// <summary>
        /// 增加设备完成事件
        /// </summary>
        /// <param name="devid"></param>
        /// <param name="devName"></param>
        /// <param name="isSuccess"></param>
        protected virtual void OnAddDeviceCompleted(int devid, string devName,bool isSuccess)
        {
            if (AddDeviceCompleted != null)
            {
                AddDeviceCompleted(devid, devName, isSuccess);
            }
        }

        /// <summary>
        /// 删除设备完成事件
        /// </summary>
        public event DeleteDeviceCompletedHandler DeleteDeviceCompleted;

        /// <summary>
        /// 删除设备完成事件
        /// </summary>
        /// <param name="devid"></param>
        /// <param name="devName"></param>
        /// <param name="isSuccess"></param>
        protected virtual void OnDeleteDeviceCompleted(int devid, string devName,bool isSuccess)
        {
            if (DeleteDeviceCompleted != null)
            {
                DeleteDeviceCompleted(devid, devName, isSuccess);
            }
        }

        /// <summary>
        /// 串口打开事件
        /// </summary>
        public event ComOpenedHandler ComOpened;

        /// <summary>
        /// 串口打开事件
        /// </summary>
        /// <param name="port"></param>
        /// <param name="baud"></param>
        /// <param name="openSuccess"></param>
        protected virtual void OnComOpened(int port, int baud,bool openSuccess)
        {
            if (ComOpened != null)
            {
                ComOpened(port, baud, openSuccess);
            }
        }

        /// <summary>
        /// 串口关闭事件
        /// </summary>
        public event ComClosedHandler ComClosed;

        /// <summary>
        /// 串口关闭事件
        /// </summary>
        /// <param name="port"></param>
        /// <param name="baud"></param>
        /// <param name="closeSuccess"></param>
        protected virtual void OnComClosed(int port, int baud,bool closeSuccess)
        {
            if (ComClosed != null)
            {
                ComClosed(port, baud, closeSuccess);
            }
        }

        /// <summary>
        /// 网络连接事件
        /// </summary>
        public event SocketConnectedHandler SocketConnected;

        /// <summary>
        /// 网络连接事件
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        protected virtual void OnSocketConnected(string ip, int port)
        {
            if (SocketConnected != null)
            {
                SocketConnected(ip, port);
            }
        }

        /// <summary>
        /// 网络断开事件
        /// </summary>
        public event SocketClosedHandler SocketClosed;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        protected virtual void OnSocketClosed(string ip, int port)
        {
            if (SocketClosed != null)
            {
                SocketClosed(ip, port);
            }
        }

        /// <summary>
        /// 日志实例
        /// </summary>
        public Log.ILog Logger { get; private set; }

        /// <summary>
        /// 配置信息
        /// </summary>
        public Config.IConfig Config { get; private set; }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    ControllerManager.RemoveAllController();
                    ChannelManager.RemoveAllChannel();

                    ICollection<IRunDevice> devs = DeviceManager.GetValues();
                    foreach (IRunDevice dev in devs)
                    {
                        dev.Exit();
                    }

                    DeviceManager.RemoveAllDevice();
                }

                _monitorException.UnMonitor();

                IsDisposed = true;

                Logger.InfoFormat(false, "{0}-{1}", ServerName, "已经停止");
            }
        }

        private bool IsDisposed { get; set; }
    }
}
