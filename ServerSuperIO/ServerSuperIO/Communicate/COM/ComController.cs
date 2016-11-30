using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ServerSuperIO.Device;
using ServerSuperIO.Server;

namespace ServerSuperIO.Communicate.COM
{
    internal class ComController : ServerProvider, IComController
    {
        private IComSession _Com = null;

        private Thread _Thread = null; //控制器线程

        private bool _IsDisposed = false;//是否释放资源

        private readonly object _SyncLock=new object();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="port"></param>
        /// <param name="baud"></param>
        public ComController(IComSession com)
            : base()
        {
            if (com == null)
            {
                throw new ArgumentNullException("串口通道实例为空");
            }

            _Com = com;

            IsExited = false;

            IsWorking = false;
        }

        ~ComController()
        {
            Dispose(false);
        }

        /// <summary>
        /// 设置串口通道
        /// </summary>
        public IComSession ComChannel
        {
            get { return _Com;}
            set { _Com = value; }
        }

        /// <summary>
        /// 是否退出
        /// </summary>
        private bool IsExited { get; set; }

        /// <summary>
        /// 关键字
        /// </summary>
        public string Key
        {
            get { return ComUtils.PortToString(_Com.Port); }
        }

        /// <summary>
        /// 是否正在工作
        /// </summary>
        public bool IsWorking { get; set; }

        /// <summary>
        /// 启动服务
        /// </summary>
        public void StartController()
        {
            if (_Thread == null || !_Thread.IsAlive)
            {
                this.IsWorking = true;
                this.IsExited = false;
                this._Thread = new Thread(new ThreadStart(RunController))
                {
                    IsBackground = true,
                    Name = "ComControllerThread"
                };
                this._Thread.Start();
            }
        }

        /// <summary>
        /// 停止控制器
        /// </summary>
        public void StopController()
        {
            Dispose(true);
        }

        /// <summary>
        /// 通讯类型
        /// </summary>
        public CommunicateType ControllerType
        {
            get { return CommunicateType.COM; }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 是否释放资源
        /// </summary>
        public bool IsDisposed
        {
            get { return _IsDisposed; }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (!_IsDisposed)
            {
                if (disposing)
                {
                    
                }

                lock (_SyncLock)
                {
                    if (this._Thread != null && this._Thread.IsAlive)
                    {
                        this.IsExited = true;
                        this._Thread.Join(1500);
                        if (this._Thread.IsAlive)
                        {
                            try
                            {
                                _Thread.Abort();
                            }
                            catch
                            {

                            }
                        }
                    }
                }
               
                _IsDisposed = true;
            }
        }

        /// <summary>
        /// 运行器
        /// </summary>
        private void RunController()
        {
            while (!IsExited)
            {
                if (!IsWorking)
                {
                    System.Threading.Thread.Sleep(100);
                    continue;
                }

                IRunDevice[] devList = this.Server.DeviceManager.GetDevices(_Com.Port.ToString(), CommunicateType.COM);

                if (devList.Length <= 0)
                {
                    System.Threading.Thread.Sleep(100);
                    continue;
                }

                //检测当前控制器的运行优化级
                IRunDevice dev = this.Server.DeviceManager.GetPriorityDevice(devList);

                if (dev != null)//如果有优先级设备，则直接调度设备
                {
                    this.RunDevice(dev);
                }
                else           //如果没有优先级设备，则轮询调度设备
                {
                    for (int i = 0; i < devList.Length; i++)
                    {
                        if (IsExited || !IsWorking)
                        {
                            break;
                        }

                        //---------每次循环都检测优先级，保证及时响应----------//
                        dev = this.Server.DeviceManager.GetPriorityDevice(devList);

                        if (dev != null)
                        {
                            this.RunDevice(dev);
                        }
                        //-------------------------------------------------//

                        this.RunDevice(devList[i]);
                    }
                }
            }
        }

        /// <summary>
        /// 运行调度设备
        /// </summary>
        /// <param name="dev"></param>
        private void RunDevice(IRunDevice dev)
        {
            try
            {
                if (!_Com.IsDisposed)
                {
                    if (!_Com.IsOpen)
                    {
                        lock (_Com.SyncLock)
                        {
                           _Com.Open();
                        }
                    }

                    if (_Com.IsOpen)
                    {
                        dev.Run((IChannel)_Com); //驱动设备运行接口
                    }
                    else
                    {
                        dev.Run((IChannel)null);
                    }
                }
                else
                {
                    dev.Run((IChannel)null);
                }

                if (_Com == null || !_Com.IsOpen)
                {
                    System.Threading.Thread.Sleep(1000);
                }
                //------------------------------------------------//
            }
            catch (Exception ex)
            {
                this.Server.Logger.Error(true,"串口控制器",ex);
            }
        }
    }
}
