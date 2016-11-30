using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ServerSuperIO.Base;
using ServerSuperIO.Communicate;
using ServerSuperIO.Communicate.COM;
using ServerSuperIO.Communicate.NET;
using ServerSuperIO.Config;
using ServerSuperIO.Device;
using ServerSuperIO.Service;
using ServerSuperIO.Show;

namespace ServerSuperIO.Server
{
    public abstract class AppServer : AppServerBase
    {
        private Manager<string, IGraphicsShow> _Shows;
        private Manager<string, IAppService> _Services;

        protected AppServer(string serverName, IConfig config)
            : base(serverName, config)
        {
            _Shows = new Manager<string, IGraphicsShow>();
            _Services = new Manager<string, IAppService>();
        }

        public override void Start()
        {
            base.Start();
        }

        public override void Stop()
        {
            if (_Shows != null)
            {
                foreach (KeyValuePair<string, IGraphicsShow> show in _Shows)
                {
                    try
                    {
                        show.Value.Close();
                    }
                    catch
                    {
                    }
                }
                _Shows.Clear();
            }

            if (_Services != null)
            {
                foreach (KeyValuePair<string, IAppService> service in _Services)
                {
                    try
                    {
                        service.Value.StopService();
                    }
                    catch
                    {
                    }

                }
                _Services.Clear();
            }

            base.Stop();
        }

        public override void AddDevice(IRunDevice dev)
        {
            if (dev == null)
            {
                throw new ArgumentNullException("设备驱动", "参数为空");
            }

            if (dev.DeviceParameter.DeviceID != dev.DeviceDynamic.DeviceID)
            {
                throw new NotEqualException("参数和动态数据的设备ID不相等");
            }

            int devid = -1;
            if (dev.DeviceParameter.DeviceID == -1 || dev.DeviceDynamic.DeviceID == -1)
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

            if (Config.ControlMode == ControlMode.Singleton)
            {
                IRunDevice[] netDevices = DeviceManager.GetDevices(CommunicateType.NET);
                if (dev.CommunicateType == CommunicateType.NET && netDevices.Length >= 1)//如果是Singleton模式只能有一个网络设备驱动
                {
                    throw new IndexOutOfRangeException("当为Singleton模式时，不能增加多个网络设备驱动");
                }
            }

            string desc = String.Empty;
            if (this.DeviceManager.AddDevice(dev.DeviceParameter.DeviceID, dev))
            {
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

                desc += ",成功";
                OnAddDeviceCompleted(dev.DeviceParameter.DeviceID, dev.DeviceParameter.DeviceName, true);
            }
            else
            {
                desc += ",失败";
                OnAddDeviceCompleted(dev.DeviceParameter.DeviceID, dev.DeviceParameter.DeviceName, false);
            }

            this.Logger.Info(true, desc);
        }

        public override void RemoveDevice(int devid)
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

                    //--------------删除动态显示的实时数据----------------------//
                    foreach (IGraphicsShow show in this._Shows.Values)
                    {
                        show.RemoveDevice(dev.DeviceParameter.DeviceID);
                    }

                    //---------------------删除服务数据-----------------------//
                    foreach (IAppService service in this._Services.Values)
                    {
                        service.RemoveDevice(dev.DeviceParameter.DeviceID);
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

        public override void ChangeDeviceComInfo(int devid, int oldCom, int oldBaud, int newCom, int newBaud)
        {
            IRunDevice dev = DeviceManager.GetDevice(devid);
            if (dev == null)
            {
                Logger.Info(true, String.Format("{0}号设备，改变串口信息不存在", devid.ToString()));
            }
            else
            {
                int oldComPort = oldCom;
                int oldComBaud = oldBaud;
                int newComPort = newCom;
                int newComBaud = newBaud;
                bool success = true;
                if (dev.CommunicateType == CommunicateType.COM)
                {
                    if (oldComPort != newComPort)
                    {
                        #region 对旧串口进行处理
                       
                        //--------------对旧串口进行处理----------------//
                        IRunDevice[] oldComDevList = DeviceManager.GetDevices(oldComPort.ToString(), CommunicateType.COM);

                        int oldComDevCount = oldComDevList.Count((d) => d.GetHashCode() != dev.GetHashCode());//当前串口不等于当前设备的设备数

                        if (oldComDevCount <= 0)//先修改设备的串口参数，该串口没有可用的设备
                        {
                            string oldKey = ComUtils.PortToString(oldComPort);
                            IController oldComController = ControllerManager.GetController(oldKey);
                            if (oldComController != null)
                            {
                                if (ControllerManager.RemoveController(oldComController.Key))
                                {
                                    oldComController.StopController();
                                    oldComController.Dispose();

                                    IComSession comChannel = (IComSession)((IComController)oldComController).ComChannel;
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
                                else
                                {
                                    success = false;
                                }
                            }
                            else
                            {
                                Logger.Info(true, "该设备的串口控制器为空");
                            }
                        }
                        #endregion

                        #region 对新串口进行处理
                        string newKey = ComUtils.PortToString(newComPort);
                        //--------------对新串口进行处理----------------//
                        bool newComControllerExist = ControllerManager.ContainController(newKey);
                        if (!newComControllerExist)
                        {
                            IChannel channel = ChannelManager.GetChannel(newKey);
                            if (channel == null)
                            {
                                IComSession comChannel = new ComSession(newComPort, newComBaud);
                                comChannel.Setup(this);
                                comChannel.Initialize();
                                comChannel.COMOpen += ComChannel_COMOpen;
                                comChannel.COMClose += ComChannel_COMClose;
                                comChannel.COMError += ComChannel_COMError;
                                comChannel.Open();
                                channel = (IChannel)comChannel;

                                ChannelManager.AddChannel(comChannel.Key, channel);
                            }

                            IController controller = ControllerManager.GetController(newKey);
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
                        }

                        if (success)
                        {
                            dev.DeviceParameter.COM.Port = newComPort;
                            Logger.Info(true, String.Format("{0},串口从{1}改为{2},成功", dev.DeviceParameter.DeviceName, oldComPort.ToString(), newComPort.ToString()));
                        }
                        else
                        {
                            Logger.Info(true, String.Format("{0},串口从{1}改为{2},失败", dev.DeviceParameter.DeviceName, oldComPort.ToString(), newComPort.ToString()));
                        }
                        #endregion
                    }
                    else
                    {
                        #region 波特率
                        if (oldComBaud != newComBaud)
                        {
                            IComSession comIO = (IComSession)ChannelManager.GetChannel(ComUtils.PortToString(oldComPort));
                            if (comIO != null)
                            {
                                success = comIO.Settings(newComBaud);
                                if (success)
                                {
                                    dev.DeviceParameter.COM.Baud = newComBaud;
                                    Logger.Info(true, String.Format("{0},串口{1}的波特率从{2}改为{3},成功", dev.DeviceParameter.DeviceName,oldComPort.ToString(), oldComBaud.ToString(), newComBaud.ToString()));
                                }
                                else
                                {
                                    Logger.Info(true, String.Format("{0},串口{1}的波特率从{2}改为{3},失败", dev.DeviceParameter.DeviceName, oldComPort.ToString(), oldComBaud.ToString(), newComBaud.ToString()));
                                }
                            }
                        }
                        #endregion
                    }
                }
                else
                {
                    Logger.Info(true, String.Format("{0},不是串口类型的设备", dev.DeviceParameter.DeviceName));
                }
            }
        }

        public override bool AddGraphicsShow(Show.IGraphicsShow graphicsShow)
        {
            if (!_Shows.ContainsKey(graphicsShow.ThisKey))
            {
                graphicsShow.MouseRightContextMenu += GraphicsShow_MouseRightContextMenu;
                graphicsShow.GraphicsShowClosed += GraphicsShow_GraphicsShowClosed;
                if (_Shows.TryAdd(graphicsShow.ThisKey, graphicsShow))
                {
                    Logger.Info(true, String.Format("<{0}>显示视图显示成功", graphicsShow.ThisName));
                    try
                    {
                        graphicsShow.Show();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(true,"",ex);
                    }
                    return true;
                }
                else
                {
                    Logger.Info(true, String.Format("<{0}>显示视图显示失败", graphicsShow.ThisName));
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public override bool RemoveGraphicsShow(string showKey)
        {
            IGraphicsShow gShow;
            if (_Shows.TryRemove(showKey, out gShow))
            {
                gShow.MouseRightContextMenu -= GraphicsShow_MouseRightContextMenu;
                gShow.GraphicsShowClosed -= GraphicsShow_GraphicsShowClosed;
                gShow.Close();
                gShow.Dispose();

                Logger.Info(true, String.Format("<{0}>显示视图关闭成功", gShow.ThisName));
                return true;
            }
            else
            {
                Logger.Info(true, String.Format("<{0}>显示视图关闭失败", gShow.ThisName));
                return false;
            }
        }

        public override bool AddAppService(IAppService service)
        {
            if (!_Services.ContainsKey(service.ThisKey))
            {
                if (_Services.TryAdd(service.ThisKey, service))
                {
                    try
                    {
                        service.StartService();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(true,"", ex);
                    }
                   
                    Logger.Info(true, String.Format("<{0}>增加服务成功", service.ThisName));
                    return true;
                }
                else
                {
                    Logger.Info(true, String.Format("<{0}>增加服务失败", service.ThisName));
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public override bool RemoveAppService(string serviceKey)
        {
            IAppService service;
            if (_Services.TryRemove(serviceKey, out service))
            {
                service.StopService();
                service.Dispose();
                Logger.Info(true, String.Format("<{0}>关闭服务成功", service.ThisName));
                return true;
            }
            else
            {
                Logger.Info(true, String.Format("<{0}>关闭服务失败", service.ThisName));
                return false;
            }
        }

        private void ComChannel_COMError(IComSession com, int port, int baud, string error)
        {
            OnChannelChanged(ComUtils.PortToString(port), CommunicateType.COM, ChannelState.None);

            Logger.Error(true, String.Format("{0}-{1},{2}", port.ToString(), baud.ToString(), error));
        }

        private void ComChannel_COMClose(IComSession com, int port, int baud, bool closeSuccess)
        {
            if (closeSuccess)
            {
                OnChannelChanged(ComUtils.PortToString(port), CommunicateType.COM, ChannelState.Close);
            }
            OnComClosed(port, baud, closeSuccess);
        }

        private void ComChannel_COMOpen(IComSession com, int port, int baud, bool openSuccess)
        {
            if (openSuccess)
            {
                OnChannelChanged(ComUtils.PortToString(port), CommunicateType.COM, ChannelState.Open);
            }

            OnComOpened(port, baud, openSuccess);
        }

        private void BindDeviceHandler(IRunDevice dev, DeviceType devType, bool isBind)
        {
            if (devType == DeviceType.Virtual)
            {
                #region
                if (isBind)
                {
                    dev.DeviceRuningLog += DeviceRuningLog;
                    dev.DeviceObjectChanged += new DeviceObjectChangedHandler(DeviceObjectChanged);
                }
                else
                {
                    dev.DeviceRuningLog -= new DeviceRuningLogHandler(DeviceRuningLog);
                    dev.DeviceObjectChanged -= new DeviceObjectChangedHandler(DeviceObjectChanged);
                }
                #endregion
            }
            else
            {
                if (isBind)
                {
                    dev.DeviceRuningLog += DeviceRuningLog;
                    dev.DeviceObjectChanged += DeviceObjectChanged;
                    //dev.ReceiveData += ReceiveData;
                    dev.SendData += SendData;
                    if (dev.CommunicateType == CommunicateType.COM)
                    {
                        dev.ComParameterExchange += ComParameterExchange;
                    }
                }
                else
                {
                    dev.DeviceRuningLog -= DeviceRuningLog;
                    dev.DeviceObjectChanged -= DeviceObjectChanged;
                    //dev.ReceiveData -= ReceiveData;
                    dev.SendData -= SendData;
                    if (dev.CommunicateType == CommunicateType.COM)
                    {
                        dev.ComParameterExchange -= ComParameterExchange;
                    }
                }
            }
        }

        private void ComParameterExchange(object source, ComParameterExchangeArgs e)
        {
            ChangeDeviceComInfo(e.DeviceID, e.OldCOM, e.OldBaud, e.NewCOM, e.NewBaud);
        }

        //private void ReceiveData(object source, ReceiveDataArgs e)
        //{
        //    throw new NotImplementedException();
        //}

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
                    if (Config.ControlMode == ControlMode.Self)
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
                ICollection<IGraphicsShow> showList = _Shows.Values;
                foreach (IGraphicsShow show in showList)
                {
                    if (!show.IsDisposed)
                    {
                        show.UpdateDevice(e.DeviceID, e.Object);
                    }
                }
                //---------------------服务输出-----------------------//
                ICollection<IAppService> serviceList = _Services.Values;
                foreach (IAppService app in serviceList)
                {
                    if (!app.IsDisposed)
                    {
                        app.UpdateDevice(e.DeviceID, e.Object);
                    }
                }

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

        private void GraphicsShow_GraphicsShowClosed(object key)
        {
            RemoveGraphicsShow(key.ToString());
        }

        private void GraphicsShow_MouseRightContextMenu(int devid)
        {
            IRunDevice dev = DeviceManager.GetDevice(devid);
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

        protected void OnChannelChanged(string comPara1, CommunicateType comType, ChannelState channelState)
        {
            if (DeviceManager.Count > 0)
            {
                if (Config.ControlMode == ControlMode.Loop
                    || Config.ControlMode == ControlMode.Self
                    || Config.ControlMode == ControlMode.Parallel)
                {

                    IRunDevice[] list = this.DeviceManager.GetDevices(comPara1, comType);
                    if (list != null && list.Length > 0)
                    {
                        foreach (IRunDevice dev in list)
                        {
                            try
                            {
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
    }
}
