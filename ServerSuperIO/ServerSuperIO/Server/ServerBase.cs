using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ServerSuperIO.Base;
using ServerSuperIO.Common;
using ServerSuperIO.Communicate;
using ServerSuperIO.Communicate.COM;
using ServerSuperIO.Communicate.NET;
using ServerSuperIO.Config;
using ServerSuperIO.Device;
using ServerSuperIO.Device.Connector;
using ServerSuperIO.Log;
using ServerSuperIO.Service;
using ServerSuperIO.Service.Connector;
using ServerSuperIO.Show;

namespace ServerSuperIO.Server
{
    public abstract class ServerBase : IServer
    {
        private MonitorException _monitorException;
        private IDeviceContainer _deviceContainer;

        internal ServerBase(IServerConfig config, IDeviceContainer deviceContainer = null, ILogContainer logContainer = null)
        {
            if (config == null)
            {
                throw new ArgumentNullException("配制信息", "参数为空");
            }
            ServerConfig = config;

            _deviceContainer = deviceContainer;
            if (_deviceContainer != null)
            {
                _deviceContainer.MouseRightContextMenu += Device_MouseRightContextMenu;
            }

            if (logContainer == null)
            {
                logContainer = new ConsoleContainer();
            }

            Logger = (new LogFactory()).GetLog(ServerName, logContainer);

            _monitorException = new MonitorException();

            ServiceManager = new ServiceManager(Logger);
            GraphicsShowManager = new GraphicsShowManager(Logger);
            DeviceManager = new DeviceManager(Logger);
            ChannelManager = new ChannelManager(Logger);
            ControllerManager = new ControllerManager(Logger);

            IsDisposed = false;
        }

        ~ServerBase()
        {
            Dispose(false);
        }

        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServerName
        {
            get { return ServerConfig.ServerName; }
        }

        /// <summary>
        /// 开始服务
        /// </summary>
        public virtual void Start()
        {
            _monitorException.Monitor();
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        public virtual void Stop()
        {
            Dispose(true);
        }

        /// <summary>
        /// 增加设备
        /// </summary>
        /// <param name="dev"></param>
        /// <returns>设备ID</returns>
        public string AddDevice(IRunDevice dev)
        {
            if (dev == null)
            {
                throw new ArgumentNullException("设备驱动", "参数为空");
            }

            if (dev.DeviceParameter.DeviceID != dev.DeviceDynamic.DeviceID)
            {
                throw new NotEqualException("参数和动态数据的设备ID不相等");
            }

            String devid = String.Empty;
            if (dev.DeviceParameter.DeviceID == String.Empty
                || dev.DeviceDynamic.DeviceID == String.Empty)
            {
                devid = this.DeviceManager.BuildDeviceID();
                dev.DeviceParameter.DeviceID = devid;
                dev.DeviceDynamic.DeviceID = devid;
                try
                {
                    dev.Initialize(devid);
                }
                catch { throw; }
            }

            if (DeviceManager.ContainDevice(devid))
            {
                throw new ArgumentException("已经有相同的设备ID存在");
            }

            if (ServerConfig.ControlMode == ControlMode.Singleton)
            {
                if (dev.CommunicateType == CommunicateType.NET)
                {
                    IRunDevice[] netDevices = DeviceManager.GetDevices(CommunicateType.NET);

                    if (netDevices.Length >= 1) //如果是Singleton模式只能有一个网络设备驱动
                    {
                        throw new IndexOutOfRangeException("当为Singleton模式时，不能增加多个网络设备驱动");
                    }
                }
            }

            string desc = String.Empty;
            if (this.DeviceManager.AddDevice(dev.DeviceParameter.DeviceID, dev))
            {
                dev.Setup(this);

                this.BindDeviceHandler(dev, dev.DeviceType, true);

                if (dev.DeviceType == DeviceType.Virtual)
                {
                    desc = "增加虚拟设备";
                }
                else
                {
                    IController controller = null;
                    if (dev.CommunicateType == CommunicateType.COM)
                    {
                        #region 串口
                        string key = ComUtils.PortToString(dev.DeviceParameter.COM.Port);
                        IChannel channel = ChannelManager.GetChannel(key);
                        if (channel == null)
                        {
                            IComSession comChannel = new ComSession(dev.DeviceParameter.COM.Port, dev.DeviceParameter.COM.Baud);
                            comChannel.Setup(this);
                            comChannel.Initialize();
                            comChannel.COMOpen += ComChannel_COMOpen;
                            comChannel.COMClose += ComChannel_COMClose;
                            comChannel.COMError += ComChannel_COMError;
                            comChannel.Open();
                            channel = (IChannel)comChannel;

                            ChannelManager.AddChannel(key, channel);
                        }

                        controller = ControllerManager.GetController(key);
                        if (controller == null)
                        {
                            controller = new ComController((IComSession)channel);
                            controller.Setup(this);
                            if (ControllerManager.AddController(controller.Key, controller))
                            {
                                controller.StartController();
                            }
                        }
                        else
                        {
                            IComController comController = (IComController)controller;
                            if (comController.ComChannel.GetHashCode() != channel.GetHashCode())
                            {
                                comController.ComChannel = (IComSession)channel;
                            }
                        }

                        desc = String.Format("增加'{0}'串口设备，串口:{1} 波特率:{2}", dev.DeviceParameter.DeviceName, dev.DeviceParameter.COM.Port.ToString(), dev.DeviceParameter.COM.Baud.ToString());
                        #endregion
                    }
                    else if (dev.CommunicateType == CommunicateType.NET)
                    {
                        #region 网络
                        controller = ControllerManager.GetController(SocketController.ConstantKey);
                        if (controller == null)
                        {
                            controller = new SocketController();
                            controller.Setup(this);
                            if (ControllerManager.AddController(controller.Key, controller))
                            {
                                controller.StartController();
                            }
                        }

                        desc = String.Format("增加'{0}'网络设备，IP地址:{1} 端口:{2}", dev.DeviceParameter.DeviceName, dev.DeviceParameter.NET.RemoteIP, dev.DeviceParameter.NET.RemotePort.ToString());
                        #endregion
                    }
                    else
                    {
                        desc = "无法识别设备的通讯类型";
                    }
                }

                if (_deviceContainer != null)
                {
                    _deviceContainer.UpdateDevice(dev.DeviceParameter.DeviceID, dev.GetDeviceContainerItem());
                }

                desc += ",成功";
                OnAddDeviceCompleted(dev.DeviceParameter.DeviceID, dev.DeviceParameter.DeviceName, true);
            }
            else
            {
                desc += ",失败";
                OnAddDeviceCompleted(dev.DeviceParameter.DeviceID, dev.DeviceParameter.DeviceName, false);
            }

            this.Logger.Info(true, desc);

            return devid != String.Empty ? devid : dev.DeviceParameter.DeviceID;
        }

        public void RemoveDevice(string devid)
        {
            IRunDevice dev = DeviceManager.GetDevice(devid);

            if (dev != null)
            {
                string desc = String.Empty;
                string devname = dev.DeviceParameter.DeviceName;
                if (DeviceManager.RemoveDevice(dev.DeviceParameter.DeviceID))
                {
                    if (dev.DeviceType == DeviceType.Virtual)
                    {
                        desc = "删除虚拟设备";
                    }
                    else
                    {
                        #region
                        if (dev.CommunicateType == CommunicateType.COM)
                        {
                            IRunDevice[] comDevices = DeviceManager.GetDevices(dev.DeviceParameter.COM.Port.ToString(), CommunicateType.COM);

                            if (comDevices.Length == 0)
                            {
                                string key = ComUtils.PortToString(dev.DeviceParameter.COM.Port);
                                IController controller = ControllerManager.GetController(key);
                                if (controller != null)
                                {
                                    controller.IsWorking = false;
                                    if (ControllerManager.RemoveController(controller.Key))
                                    {
                                        controller.StopController();
                                        controller.Dispose();

                                        IComSession comChannel = (IComSession)((IComController)controller).ComChannel;
                                        comChannel.Close();
                                        comChannel.COMOpen -= ComChannel_COMOpen;
                                        comChannel.COMClose -= ComChannel_COMClose;
                                        comChannel.COMError -= ComChannel_COMError;

                                        if (ChannelManager.RemoveChannel(comChannel.Key))
                                        {
                                            comChannel.Close();
                                            comChannel.Dispose();
                                        }
                                    }
                                }
                            }

                            desc = String.Format("{0},从串口'{1}'删除", dev.DeviceParameter.DeviceName, dev.DeviceParameter.COM.Port.ToString());
                        }
                        else if (dev.CommunicateType == CommunicateType.NET)
                        {
                            desc = String.Format("{0}-{1},从网络中删除成功", dev.DeviceParameter.DeviceName, dev.DeviceParameter.NET.RemoteIP);
                        }
                        #endregion
                    }

                    GraphicsShowManager.BatchRemoveDevice(dev.DeviceParameter.DeviceID);

                    ServiceManager.BatchRemoveDevice(dev.DeviceParameter.DeviceID);

                    if (_deviceContainer != null)
                    {
                        _deviceContainer.RemoveDevice(dev.DeviceParameter.DeviceID);
                    }

                    dev.DeviceParameter.Delete();
                    dev.DeviceDynamic.Delete();
                    dev.Delete();
                    dev.Dispose();

                    BindDeviceHandler(dev, dev.DeviceType, false);

                    desc += ",成功";
                    OnDeleteDeviceCompleted(dev.DeviceParameter.DeviceID, dev.DeviceParameter.DeviceName, true);
                }
                else
                {
                    desc += ",失败";
                    OnDeleteDeviceCompleted(dev.DeviceParameter.DeviceID, dev.DeviceParameter.DeviceName, false);
                }

                Logger.Info(true, desc);
            }
        }

        protected virtual void BindDeviceHandler(IRunDevice dev, DeviceType deviceType, bool isBind)
        {
            if (deviceType == DeviceType.Virtual)
            {
                #region
                if (isBind)
                {
                    dev.DeviceRuningLog += DeviceRuningLog;
                    dev.UpdateContainer += UpdateContainer;
                    dev.DeviceObjectChanged += DeviceObjectChanged;
                    dev.DeleteDevice += DeleteDevice;
                    dev.DeviceConnector += DeviceConnector;
                }
                else
                {
                    dev.DeviceRuningLog -= DeviceRuningLog;
                    dev.UpdateContainer -= UpdateContainer;
                    dev.DeviceObjectChanged -= DeviceObjectChanged;
                    dev.DeleteDevice -= DeleteDevice;
                    dev.DeviceConnector -= DeviceConnector;
                }
                #endregion
            }
            else
            {
                if (isBind)
                {
                    dev.DeviceRuningLog += DeviceRuningLog;
                    dev.UpdateContainer += UpdateContainer;
                    dev.DeviceObjectChanged += DeviceObjectChanged;
                    dev.SendData += SendData;
                    dev.DeleteDevice += DeleteDevice;
                    dev.DeviceConnector += DeviceConnector;
                }
                else
                {
                    dev.DeviceRuningLog -= DeviceRuningLog;
                    dev.UpdateContainer -= UpdateContainer;
                    dev.DeviceObjectChanged -= DeviceObjectChanged;
                    dev.SendData -= SendData;
                    dev.DeleteDevice -= DeleteDevice;
                    dev.DeviceConnector -= DeviceConnector;
                }
            }
        }

        private void DeviceConnector(object source, Device.Connector.DeviceConnectorArgs e)
        {
            if (e == null) return;

            IRunDevice runDevice = this.DeviceManager.GetDeviceFromCode(e.DeviceToDevice.DeviceCode);
            if (runDevice != null)
            {
                Func<IFromDevice, IDeviceToDevice, object> deviceConnctor = runDevice.RunDeviceConnector;
                DeviceConnectorToken token = new DeviceConnectorToken(e.FromDevice.DeviceID, deviceConnctor);
                deviceConnctor.BeginInvoke(e.FromDevice, e.DeviceToDevice, new AsyncCallback(DeviceConnectorCallback), token);
            }
        }

        private void DeviceConnectorCallback(IAsyncResult ar)
        {
            DeviceConnectorToken token = (DeviceConnectorToken)ar.AsyncState;
            IRunDevice sourceDevice = this.DeviceManager.GetDevice(token.FromDeviceID);
            if (sourceDevice == null) return;
            object obj = null;
            try
            {
                obj = token.DeviceConnectorAsync.EndInvoke(ar);
            }
            catch (Exception ex)
            {
                try
                {
                    sourceDevice.DeviceConnectorCallbackError(ex);
                }
                catch (Exception e)
                {
                    this.Logger.Error(true,"",e);
                }
            }

            try
            {
                sourceDevice.DeviceConnectorCallback(obj);
            }
            catch (Exception ex)
            {
                this.Logger.Error(true, "", ex);
            }
        }

        internal abstract void ComChannel_COMError(IComSession com, int port, int baud, string error);

        internal abstract void ComChannel_COMClose(IComSession com, int port, int baud, bool closesuccess);

        internal abstract void ComChannel_COMOpen(IComSession com, int port, int baud, bool opensuccess);

        public bool AddGraphicsShow(Show.IGraphicsShow graphicsShow, IWin32Window window)
        {
            if (GraphicsShowManager.AddShow(graphicsShow.ShowKey, graphicsShow))
            {
                graphicsShow.MouseRightContextMenu += Device_MouseRightContextMenu;
                graphicsShow.GraphicsShowClosed += GraphicsShow_GraphicsShowClosed;

                try
                {
                    graphicsShow.ShowGraphics(window);
                }
                catch (Exception ex)
                {
                    Logger.Error(true,"",ex);
                }
                
                Logger.Info(true, String.Format("<{0}>显示视图显示成功", graphicsShow.ShowName));
                return true;
            }
            else
            {
                Logger.Info(true, String.Format("<{0}>显示视图显示失败", graphicsShow.ShowName));
                return false;
            }
        }

        public bool RemoveGraphicsShow(string showKey)
        {
            IGraphicsShow gShow=GraphicsShowManager.GetShow(showKey);
            if (gShow!=null)
            {
                GraphicsShowManager.RemoveShow(showKey);
                Logger.Info(true, String.Format("<{0}>显示视图关闭成功", gShow.ShowName));
                return true;
            }
            else
            {
                return false;
            }
        }

        private void GraphicsShow_GraphicsShowClosed(object key)
        {
            RemoveGraphicsShow(key.ToString());
        }

        private void Device_MouseRightContextMenu(string devCode)
        {
            IRunDevice dev = DeviceManager.GetDeviceFromCode(devCode);
            if (dev != null)
            {
                try
                {
                    dev.ShowContextMenu();
                }
                catch (Exception ex)
                {
                    Logger.Error(true, ex.Message);
                }
            }
            else
            {
                Logger.Info(false, "未找到能够显示菜单的设备");
            }
        }

        public bool AddService(IService service)
        {
            if (ServiceManager.AddService(service.ServiceKey, service))
            {
                service.ServiceConnector += ServiceConnector;
                service.ServiceLog += ServiceRuningLog;
                if (service.IsAutoStart)
                {
                    try
                    {
                        service.StartService();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(true, "", ex);
                    }
                }
                Logger.Info(true, String.Format("<{0}>增加服务成功", service.ServiceName));
                return true;
            }
            else
            {
                Logger.Info(true, String.Format("<{0}>增加服务失败", service.ServiceName));
                return false;
            }
        }

        public bool RemoveService(string serviceKey)
        {
            IService service = ServiceManager.GetService(serviceKey);
            if (service != null)
            {
                ServiceManager.RemoveService(serviceKey);
                Logger.Info(true, String.Format("<{0}>关闭服务成功", service.ServiceName));
                return true;
            }
            else
            {
                return false;
            }
        }

        private void ServiceConnector(object source, Service.Connector.ServiceConnectorArgs e)
        {
            if (e == null) return;

            IRunDevice runDevice = this.DeviceManager.GetDeviceFromCode(e.ServiceToDevice.DeviceCode);
            if (runDevice != null)
            {
                Func<IFromService, IServiceToDevice, object> serviceConnctor = runDevice.RunServiceConnector;
                ServiceConnectorToken token = new ServiceConnectorToken(e.FromService.ServiceKey, serviceConnctor);
                serviceConnctor.BeginInvoke(e.FromService, e.ServiceToDevice, new AsyncCallback(ServiceConnectorCallback), token);
            }
        }

        private void ServiceConnectorCallback(IAsyncResult ar)
        {
            ServiceConnectorToken token = (ServiceConnectorToken)ar.AsyncState;

            IService sourceService = ServiceManager.GetService(token.FromServiceKey);

            if (sourceService == null) return;

            object obj = null;
            try
            {
                obj = token.DeviceConnectorAsync.EndInvoke(ar);
            }
            catch (Exception ex)
            {
                try
                {
                    sourceService.ServiceConnectorCallbackError(ex);
                }
                catch (Exception e)
                {
                    this.Logger.Error(true,"",e);
                }
            }

            try
            {
                sourceService.ServiceConnectorCallback(obj);
            }
            catch (Exception ex)
            {
                this.Logger.Error(true, "", ex);
            }
        }

        /// <summary>
        /// 改变设备的串口信息
        /// </summary>
        /// <param name="devid"></param>
        /// <param name="oldCom"></param>
        /// <param name="oldBaud"></param>
        /// <param name="newCom"></param>
        /// <param name="newBaud"></param>
        public abstract void ChangeDeviceComInfo(string devid, int oldCom, int oldBaud, int newCom, int newBaud);

        /// <summary>
        /// 设备管理器
        /// </summary>
        public Device.IDeviceManager<string, Device.IRunDevice> DeviceManager { get; private set; }

        /// <summary>
        /// IO通道管理器
        /// </summary>
        public Communicate.IChannelManager<string, Communicate.IChannel> ChannelManager { get; private set; }

        /// <summary>
        /// 控制管理器
        /// </summary>
        public Communicate.IControllerManager<string, Communicate.IController> ControllerManager { get; private set; }

        /// <summary>
        /// 服务管理类
        /// </summary>
        public IServiceManager<string, IService> ServiceManager { get; private set; }

        /// <summary>
        /// 显示管理
        /// </summary>
        public IGraphicsShowManager<string, IGraphicsShow> GraphicsShowManager { get; private set; }

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
        protected virtual void OnAddDeviceCompleted(string devid, string devName, bool isSuccess)
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
        protected virtual void OnDeleteDeviceCompleted(string devid, string devName, bool isSuccess)
        {
            if (DeleteDeviceCompleted != null)
            {
                DeleteDeviceCompleted(devid, devName, isSuccess);
            }
        }

        private void DeleteDevice(object source, DeleteDeviceArgs e)
        {
            if (e == null)
            {
                return;
            }

            this.RemoveDevice(e.DeviceID);
        }


        private void UpdateContainer(object source, UpdateContainerArgs e)
        {
            if (e == null)
            {
                return;
            }

            if (_deviceContainer != null)
            {
                IRunDevice dev = this.DeviceManager.GetDevice(e.DeviceID);
                if (dev != null)
                {
                    IDeviceContainerItem item = dev.GetDeviceContainerItem();
                    if (item != null)
                    {
                        _deviceContainer.UpdateDevice(e.DeviceID, item);
                    }
                }
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
        protected virtual void OnComOpened(int port, int baud, bool openSuccess)
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
        protected virtual void OnComClosed(int port, int baud, bool closeSuccess)
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
        public Config.IServerConfig ServerConfig { get; private set; }

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
                this.AddDeviceCompleted = null;
                this.DeleteDeviceCompleted = null;
                this.ComClosed = null;
                this.ComOpened = null;
                this.SocketClosed = null;
                this.SocketConnected = null;

                if (disposing)
                {
                    ControllerManager.RemoveAllController();

                    ChannelManager.RemoveAllChannel();

                    ServiceManager.RemoveAllService();

                    foreach (KeyValuePair<string,IGraphicsShow> show in GraphicsShowManager)
                    {
                        try
                        {
                            show.Value.CloseGraphics();
                        }
                        catch (Exception ex)
                        {
                            this.Logger.Error(true, "", ex);
                        }

                        try
                        {
                            show.Value.Dispose();
                        }
                        catch (Exception ex)
                        {
                            this.Logger.Error(true, "", ex);
                        }
                    }

                    
                    foreach (IRunDevice runDev in DeviceManager)
                    {
                        try
                        {
                            runDev.Exit();
                        }
                        catch (Exception ex)
                        {
                            this.Logger.Error(true,"",ex);
                        }
                        try
                        {
                            runDev.Dispose();
                        }
                        catch (Exception ex)
                        {
                            this.Logger.Error(true, "", ex);
                        }
                    }


                    if (_deviceContainer != null)
                    {
                        _deviceContainer.Dispose();
                    }
                }

                _monitorException.UnMonitor();

                IsDisposed = true;

                Logger.InfoFormat(false, "{0}-{1}", ServerName, "已经停止");
            }
        }

        private bool IsDisposed { get; set; }

        protected void OnChannelChanged(string comPara1, CommunicateType comType, ChannelState channelState)
        {
            if (DeviceManager.Count > 0)
            {
                if (ServerConfig.ControlMode == ControlMode.Loop
                    || ServerConfig.ControlMode == ControlMode.Self
                    || ServerConfig.ControlMode == ControlMode.Parallel)
                {

                    IRunDevice[] list = this.DeviceManager.GetDevices(comPara1, comType);
                    if (list != null && list.Length > 0)
                    {
                        foreach (IRunDevice dev in list)
                        {
                            try
                            {
                                dev.DeviceDynamic.ChannelState = channelState;
                                dev.ChannelStateChanged(channelState);
                            }
                            catch (Exception ex)
                            {
                                Logger.Error(true, ex.Message);
                            }
                        }
                    }
                }
            }
        }

        private void SendData(object source, SendDataArgs e)
        {
            if (e == null)
                return;

            if (e.Data == null || e.Data.Length <= 0)
            {
                Logger.Info(false, e.DeviceName + ",要发送的数据为空");
                return;
            }

            IRunDevice dev = DeviceManager.GetDevice(e.DeviceID);

            if (dev != null)
            {
                if (dev.CommunicateType == CommunicateType.COM)
                {
                    Logger.Info(false, e.DeviceName + ",串口通讯设备无法实现自主发送数据");
                }
                else
                {
                    if (ServerConfig.ControlMode == ControlMode.Self)
                    {
                        ISocketController netController = (ISocketController)ControllerManager.GetController(SocketController.ConstantKey);
                        if (netController != null)
                        {
                            netController.Send(dev, e.Data);
                        }
                        else
                        {
                            Logger.Info(false, e.DeviceName + ",无法找到对应的网络控制器");
                        }
                    }
                    else
                    {
                        Logger.Info(false, e.DeviceName + ",只有控制方式为自主模式的时候，设备才能发送数据");
                    }
                }
            }
            else
            {
                Logger.Info(false, e.DeviceName + "无法获得可发送数据的设备");
            }
        }

        private void DeviceObjectChanged(object source, DeviceObjectChangedArgs e)
        {
            if (e == null) return;

            try
            {
                //---------------------实时数据显示--------------------//
                GraphicsShowManager.BatchUpdateDevice(e.DeviceID, e.Object);

                //---------------------服务输出-----------------------//
                ServiceManager.BatchUpdateDevice(e.DeviceID, e.Object);

                //---------------------检测虚拟设备-------------------//
                if (e.DeviceType != DeviceType.Virtual)
                {
                    IRunDevice[] vdevList = DeviceManager.GetDevices(DeviceType.Virtual);
                    foreach (IRunDevice dev in vdevList)
                    {
                        dev.RunVirtualDevice(e.DeviceID, e.Object);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(true, ex.Message);
            }
        }

        private void DeviceRuningLog(object source, DeviceRuningLogArgs e)
        {
            Logger.Info(false, String.Format("{0}>>{1}", e.DeviceName, e.StateDesc));
        }

        private void ServiceRuningLog(string log)
        {
            Logger.Info(false, log);
        }
    }
}
