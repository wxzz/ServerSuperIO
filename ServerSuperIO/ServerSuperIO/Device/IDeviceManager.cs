using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerSuperIO.Base;
using ServerSuperIO.Communicate;
using ServerSuperIO.Communicate.NET;
using ServerSuperIO.Log;

namespace ServerSuperIO.Device
{
    public interface IDeviceManager<TKey, TValue>:ILoggerProvider,IEnumerable where TValue : IRunDevice
    {

        TValue this[int index] { get; }

        TValue this[string key] { get; }

        /// <summary>
        /// 新建设备的ID，且唯一
        /// </summary>
        /// <returns></returns>
        string BuildDeviceID();

        /// <summary>
        /// 增加设备
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        bool AddDevice(TKey key, TValue val);

        /// <summary>
        /// 删除设备
        /// </summary>
        /// <param name="key"></param>
        bool RemoveDevice(TKey key);

        /// <summary>
        /// 移除所有设备
        /// </summary>
        void RemoveAllDevice();

        /// <summary>
        /// 获得值集合
        /// </summary>
        /// <returns></returns>
        ICollection<TValue> GetValues();

        /// <summary>
        /// 获得关键字集合
        /// </summary>
        /// <returns></returns>
        ICollection<TKey> GetKeys();

        /// <summary>
        /// 获得高优先运行设备
        /// </summary>
        /// <param name="vals"></param>
        /// <returns></returns>
        TValue GetPriorityDevice(TValue[] vals);

        /// <summary>
        /// 获得单个设备
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        TValue GetDevice(TKey key);

        /// <summary>
        /// 以deviceCode获得设备
        /// </summary>
        /// <param name="deviceCode"></param>
        /// <returns></returns>
        TValue GetDeviceFromCode(string deviceCode);

        /// <summary>
        /// 获得设备数组
        /// </summary>
        /// <param name="para">IP或串口号</param>
        /// <param name="ioType">通讯类型</param>
        /// <returns></returns>
        TValue[] GetDevices(string para, CommunicateType ioType);

        /// <summary>
        /// 获得指定IP和工作模式的网络设备
        /// </summary>
        /// <param name="remoteIP"></param>
        /// <param name="workMode"></param>
        /// <returns></returns>
        TValue[] GetDevices(string remoteIP, WorkMode workMode);


        /// <summary>
        /// 获得指定工作模式的网络设备
        /// </summary>
        /// <param name="workMode"></param>
        /// <returns></returns>
        TValue[] GetDevices(WorkMode workMode);

        /// <summary>
        /// 获得设备数组
        /// </summary>
        /// <param name="ioType"></param>
        /// <returns></returns>
        TValue[] GetDevices(CommunicateType ioType);

        /// <summary>
        /// 按设备类型获得设备
        /// </summary>
        /// <param name="devType"></param>
        /// <returns></returns>
        TValue[] GetDevices(Device.DeviceType devType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool ContainDevice(TKey key);

        /// <summary>
        /// 根据输入参数，判断是否包括设备
        /// </summary>
        /// <param name="para">IP或串口号</param>
        /// <param name="ioType">设备通讯类型</param>
        /// <returns></returns>
        bool ContainDevice(string para, CommunicateType ioType);

        /// <summary>
        /// 获得可用设备数
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 获得设备的计数器的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        int GetCounter(TKey key);

        /// <summary>
        /// 设置计数器的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        int SetCounter(TKey key, int val);
    }
}
