using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ServerSuperIO.DataCache;
using ServerSuperIO.Communicate;
using ServerSuperIO.Device.Connector;
using ServerSuperIO.Log;
using ServerSuperIO.Protocol;
using ServerSuperIO.Protocol.Filter;
using ServerSuperIO.Server;
using ServerSuperIO.Service.Connector;

namespace ServerSuperIO.Device
{
    public interface IRunDevice: IServerProvider,IVirtualDevice,IDeviceConnector, IServiceConnectorDevice
    {
        #region 函数接口

        /// <summary>
        /// 初始化设备，加载设备驱动的头一件事就是初始化设备
        /// </summary>
        /// <param name="devid"></param>
        void Initialize(string devid);

        /// <summary>
        /// 保存原始的byte数据
        /// </summary>
        /// <param name="dataOrientation"></param>
        /// <param name="data"></param>
        void SaveOriginalBytes(DataOrientation dataOrientation, byte[] data);

        /// <summary>
        /// 获得发送数据的命令，如果命令缓存中没有命令，则调用获得实时数据函数
        /// </summary>
        /// <returns></returns>
        byte[] GetSendBytes();

        /// <summary>
        /// 如果当前命令缓存没有命令，则调用该函数,一般返回获得设备的实时数据命令，
        /// </summary>
        /// <returns></returns>
        byte[] GetConstantCommand();

        /// <summary>
        /// 发送IO数据接口
        /// </summary>
        /// <param name="io"></param>
        /// <param name="senddata"></param>
        int Send(IChannel io, byte[] senddata);

        /// <summary>
        /// 接收数据信息，带过滤器
        /// </summary>
        /// <param name="io"></param>
        /// <param name="receiveFilter"></param>
        /// <returns></returns>
        IList<byte[]> Receive(IChannel io, IReceiveFilter receiveFilter);

        /// <summary>
        /// 同步运行设备（IO）
        /// </summary>
        /// <param name="io">io实例对象</param>
        void Run(IChannel io);

        /// <summary>
        /// 同步运行设备（byte[]）
        /// </summary>
        /// <param name="key"></param>
        /// <param name="channel"></param>
        /// <param name="revData">接收到的数据</param>
        void Run(string key, IChannel channel, IRequestInfo ri);

        /// <summary>
        /// 如果通讯正常，这个函数负责处理数据
        /// </summary>
        /// <param name="info"></param>
        void Communicate(IRequestInfo info);

        /// <summary>
        /// 通讯中断，未接收到数据
        /// </summary>
        void CommunicateInterrupt(IRequestInfo info);

        /// <summary>
        /// 通讯的数据错误或受到干扰
        /// </summary>
        void CommunicateError(IRequestInfo info);

        /// <summary>
        /// 通讯未知，默认状态（一般不用）
        /// </summary>
        void CommunicateNone();

        /// <summary>
        /// 检测通讯状态
        /// </summary>
        /// <param name="revdata"></param>
        /// <returns></returns>
        CommunicateState CheckCommunicateState(byte[] revdata);

        /// <summary>
        /// 报警接口函数
        /// </summary>
        void Alert();

        /// <summary>
        /// 保存解析后的数据
        /// </summary>
        void Save();

        /// <summary>
        /// 展示
        /// </summary>
        void Show();

        /// <summary>
        /// 当通讯实例为NULL的时候，调用该函数
        /// </summary>
        void UnknownIO();

        /// <summary>
        /// 通讯状态改变
        /// </summary>
        /// <param name="comState">改变后的状态</param>
        void CommunicateStateChanged(CommunicateState comState);

        /// <summary>
        /// 通道状态改变
        /// </summary>
        /// <param name="channelState"></param>
        void ChannelStateChanged(ChannelState channelState);


        /// <summary>
        /// 当软件关闭的时间，响应设备退出操作
        /// </summary>
        void Exit();

        /// <summary>
        /// 删除设备的响应接口函数
        /// </summary>
        void Delete();

        /// <summary>
        /// 可以自定义返数据对象，用于与其他组件交互
        /// </summary>
        /// <returns></returns>
        object GetObject();

        /// <summary>
        /// 设备定时器，响应定时任务
        /// </summary>
        void OnRunTimer();

        /// <summary>
        /// 显示上下文菜单
        /// </summary>
        void ShowContextMenu();

        /// <summary>
        /// 监测通道数据
        /// </summary>
        /// <param name="dataOrientation"></param>
        /// <param name="data"></param>
        void ChannelMonitorData(DataOrientation dataOrientation, byte[] data);

        /// <summary>
        /// 获得报警状态
        /// </summary>
        /// <returns></returns>
        string GetAlertState();

        ///// <summary>
        ///// 显示IO监视器的窗体
        ///// </summary>
        //void ShowMonitorDialog();

        ///// <summary>
        ///// 在IO监视器上显示byte[]数据
        ///// </summary>
        ///// <param name="data"></param>
        ///// <param name="desc"></param>
        //void ShowMonitorData(byte[] data, string desc);
        #endregion

        #region 属性接口
        /// <summary>
        /// 是否监测Channel数据
        /// </summary>
        bool IsChannelMonitor { set; get; }

        /// <summary>
        /// 监测Channel数据接口
        /// </summary>
        IChannelMonitor ChannelMonitor { set; get; }

        /// <summary>
        /// 默认程序集ID，用于存储临时对象
        /// </summary>
        object Tag { set; get; }

        /// <summary>
        /// 同步对象，用于IO互拆
        /// </summary>
        object SyncLock { get; }

        /// <summary>
        /// 实时数据持久接口
        /// </summary>
        IDeviceDynamic DeviceDynamic { get; }

        /// <summary>
        /// 设备参数持久接口
        /// </summary>
        IDeviceParameter DeviceParameter { get; }

        /// <summary>
        /// 协议驱动
        /// </summary>
        IProtocolDriver Protocol { get; }

        /// <summary>
        /// 是否开启时钟，标识是否调用OnRunTimer接口函数。
        /// </summary>
        bool IsRunTimer { set; get;}

        /// <summary>
        /// 时钟间隔值，标识定时调用DeviceTimer接口函数的周期
        /// </summary>
        int RunTimerInterval { set; get; }

        /// <summary>
        /// 设备的类型
        /// </summary>
        DeviceType DeviceType { get;  }

        /// <summary>
        /// 设备编号
        /// </summary>
        string ModelNumber { get;}

        /// <summary>
        /// 设备运行权限级别，如果运行级别高的话，则优先发送和接收数据。
        /// </summary>
        DevicePriority DevicePriority { get;set;}

        /// <summary>
        /// 设备的通讯类型
        /// </summary>
        CommunicateType CommunicateType { get;set;}

        /// <summary>
        /// 标识是否运行设备，如果为false，调用运行设备接口时直接返回
        /// </summary>
        bool IsRunDevice{ get;set;}

        /// <summary>
        /// 是否释放资源
        /// </summary>
        bool IsDisposed { get; }
        #endregion

        #region 事件接口

        /// <summary>
        /// 发送数据事件
        /// </summary>
        event SendDataHandler SendData;

        /// <summary>
        /// 发送数据事件，对SendDataHandler事件的封装
        /// </summary>
        /// <param name="senddata"></param>
        void OnSendData(byte[] senddata);

        /// <summary>
        /// 设备日志输出事件
        /// </summary>
        event DeviceRuningLogHandler DeviceRuningLog;

        /// <summary>
        /// 运行监视器显示日志事件，对DeviceRuningLogHandler事件的封装
        /// </summary>
        void OnDeviceRuningLog(string statetext);

        /// <summary>
        /// 串口参数改变事件
        /// </summary>
        event ComParameterExchangeHandler ComParameterExchange;

        /// <summary>
        /// 串口参数改变事件，对COMParameterExchangeHandler事件的封装
        /// </summary>
        void OnComParameterExchange(int oldcom, int oldbaud, int newcom, int newbaud);

        /// <summary>
        /// 设备数据对象改变事件
        /// </summary>
        event DeviceObjectChangedHandler DeviceObjectChanged;
        /// <summary>
        /// 数据驱动事件，对DeviceObjectChangedHandler事件的封装
        /// </summary>
        void OnDeviceObjectChanged(object obj);

        /// <summary>
        /// 删除设备事件
        /// </summary>
        event DeleteDeviceHandler DeleteDevice;
        /// <summary>
        /// 删除设备事件，对DeleteDeviceHandler事件的封装
        /// </summary>
        void OnDeleteDevice();

        /// <summary>
        /// 更新设备运行器事件
        /// </summary>
        event UpdateContainerHandler UpdateContainer;
        /// <summary>
        /// 更新设备运行事件，对UpdateContainerHandler事件的封装
        /// </summary>
        void OnUpdateContainer(object obj);
        #endregion
    }
}
