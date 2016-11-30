using ServerSuperIO.Communicate.COM;
using ServerSuperIO.Communicate.NET;
using ServerSuperIO.Device;
using ServerSuperIO.Persistence;

namespace ServerSuperIO.Device
{
    public interface IDeviceParameter : IVirtualDeviceParameter,IXmlPersistence
    {
        /// <summary>
        /// 设备ID，这是系统自动生成的。
        /// </summary>
        string DeviceID { get;set;}

        /// <summary>
        /// 设备编码，手动设置且唯一
        /// </summary>
        string DeviceCode { get; set; }

        /// <summary>
        /// 设备地址，有可能有重复
        /// </summary>
        int DeviceAddr { get;set;}

        /// <summary>
        /// 设备名称
        /// </summary>
        string DeviceName { get;set;}

        /// <summary>
        /// 是否保存原始数据
        /// </summary>
        bool IsSaveOriginBytes { set;get;}

        /// <summary>
        /// 是否报警
        /// </summary>
        bool IsAlert { get;set;}

        /// <summary>
        /// 如果启动报警，是不是提示声音
        /// </summary>
        bool IsAlertSound { get;set;}

        /// <summary>
        /// 串口参数信息
        /// </summary>
        COMParameter COM { get;set; }

        /// <summary>
        /// 网络连接的远程参数
        /// </summary>
        SocketParameter NET { get;set; }

        /// <summary>
        /// 格式化数据
        /// </summary>
        string DataFormat { get;set;}
    }
}