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
using ServerSuperIO.Log;
using ServerSuperIO.Service;
using ServerSuperIO.Show;

namespace ServerSuperIO.Server
{
    public abstract class ComServer : ServerBase
    {
        internal ComServer(IServerConfig config,IDeviceContainer deviceContainer = null, ILogContainer logContainer = null)
            : base(config, deviceContainer,logContainer)
        {
           
        }

        public override void Start()
        {
            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        public override void ChangeDeviceComInfo(string devid, int oldCom, int oldBaud, int newCom, int newBaud)
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

        internal override void ComChannel_COMError(IComSession com, int port, int baud, string error)
        {
            OnChannelChanged(port.ToString(), CommunicateType.COM, ChannelState.None);

            Logger.Error(true, String.Format("{0}-{1},{2}", port.ToString(), baud.ToString(), error));
        }

        internal override void ComChannel_COMClose(IComSession com, int port, int baud, bool closeSuccess)
        {
            if (closeSuccess)
            {
                OnChannelChanged(port.ToString(), CommunicateType.COM, ChannelState.Close);
            }
            OnComClosed(port, baud, closeSuccess);
        }

        internal override void ComChannel_COMOpen(IComSession com, int port, int baud, bool openSuccess)
        {
            if (openSuccess)
            {
                OnChannelChanged(port.ToString(), CommunicateType.COM, ChannelState.Open);
            }

            OnComOpened(port, baud, openSuccess);
        }

        protected override void BindDeviceHandler(IRunDevice dev, DeviceType devType, bool isBind)
        {
            if(devType != DeviceType.Virtual)
            {
                if (isBind)
                {
                    if (dev.CommunicateType == CommunicateType.COM)
                    {
                        dev.ComParameterExchange += ComParameterExchange;
                    }
                }
                else
                {
                    if (dev.CommunicateType == CommunicateType.COM)
                    {
                        dev.ComParameterExchange -= ComParameterExchange;
                    }
                }
            }

            base.BindDeviceHandler(dev,devType,isBind);
        }

        private void ComParameterExchange(object source, ComParameterExchangeArgs e)
        {
            ChangeDeviceComInfo(e.DeviceID, e.OldCOM, e.OldBaud, e.NewCOM, e.NewBaud);
        }
    }
}
