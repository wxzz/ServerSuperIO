using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerSuperIO.Base;
using ServerSuperIO.Communicate;
using ServerSuperIO.Communicate.NET;
using ServerSuperIO.Log;

namespace ServerSuperIO.Device
{
    public class DeviceManager : IDeviceManager<string, IRunDevice>
    {
        private Manager<string, IRunDevice> _Devices;
        private Manager<string, int> _Counter;

        /// <summary>
        /// 构造函数
        /// </summary>
        public DeviceManager(ILog log=null)
        {
            Logger = log;
            _Devices = new Manager<string, IRunDevice>();
            _Counter = new Manager<string, int>();
        }

        public IRunDevice this[int index]
        {
            get
            {
                if (index >= 0 && index <= _Devices.Count - 1)
                {
                    return _Devices.Values.ToArray()[index];
                }
                else
                {
                    return null;
                }
            }
        }

        public IRunDevice this[string key]
        {
            get { return _Devices[key]; }
        }

        /// <summary>
        /// 创建新的设备ID
        /// </summary>
        /// <returns></returns>
        public string BuildDeviceID()
        {
            //int maxID = this._Devices.Max((kv) => kv.Value.DeviceParameter.DeviceID);
            //return (++maxID);
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// 增加一个新设备
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool AddDevice(string key, IRunDevice val)
        {
            return _Devices.TryAdd(key, val);
        }

        /// <summary>
        /// 删除一个设备
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool RemoveDevice(string key)
        {
            bool del = false;

            IRunDevice val;
            if (_Devices.TryRemove(key, out val))
            {
                if (!val.IsDisposed)
                {
                    try
                    {
                        val.Dispose();
                    }
                    catch (Exception ex)
                    {
                        if (Logger != null)
                        {
                            Logger.Error(true, "", ex);
                        }
                    }
                }

                del = true;
            }

            if (this._Counter.ContainsKey(key))
            {
                int counter;
                this._Counter.TryRemove(key, out counter);
            }
            return del;
        }

        /// <summary>
        /// 移除全部设备
        /// </summary>
        public void RemoveAllDevice()
        {
            Parallel.ForEach(_Devices, kv =>
            {
                if (!kv.Value.IsDisposed)
                {
                    try
                    {
                        kv.Value.Dispose();
                    }
                    catch (Exception ex)
                    {
                        if (Logger != null)
                        {
                            Logger.Error(true, "", ex);
                        }
                    }
                }
            });

            this._Devices.Clear();
            this._Counter.Clear();
        }

        /// <summary>
        /// 获得设备
        /// </summary>
        /// <returns></returns>
        public ICollection<IRunDevice> GetValues()
        {
            return _Devices.Values;
        }

        /// <summary>
        /// 获得关键字
        /// </summary>
        /// <returns></returns>
        public ICollection<string> GetKeys()
        {
            return _Devices.Keys;
        }

        /// <summary>
        /// 获得优先级别高的设备
        /// </summary>
        /// <param name="vals"></param>
        /// <returns></returns>
        public IRunDevice GetPriorityDevice(IRunDevice[] vals)
        {
            IRunDevice dev = null;
            foreach (IRunDevice run in vals)
            {
                if (run.DevicePriority == DevicePriority.Priority)
                {
                    dev = run;
                    break;
                }
            }
            return dev;
        }

        /// <summary>
        /// 获得设备
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IRunDevice GetDevice(string key)
        {
            IRunDevice val;
            if (_Devices.TryGetValue(key, out val))
            {
                return val;
            }
            else
            {
                return null;
            }
        }

        public IRunDevice GetDeviceFromCode(string deviceCode)
        {
            return this._Devices.Values.FirstOrDefault(d => d.DeviceParameter.DeviceCode == deviceCode);
        }

        /// <summary>
        /// 获得设备列表
        /// </summary>
        /// <param name="iopara1"></param>
        /// <param name="ioType"></param>
        /// <returns></returns>
        public IRunDevice[] GetDevices(string iopara1, CommunicateType ioType)
        {
            List<IRunDevice> list = new List<IRunDevice>();
            foreach (KeyValuePair<string, IRunDevice> kv in _Devices)
            {
                if ((kv.Value.CommunicateType == ioType && String.CompareOrdinal(kv.Value.DeviceParameter.NET.RemoteIP, iopara1)==0) 
                    || (kv.Value.CommunicateType == ioType && String.CompareOrdinal(kv.Value.DeviceParameter.COM.Port.ToString(), iopara1) == 0))
                {
                   list.Add(kv.Value);
                }
            }
            return list.ToArray();
        }

        /// <summary>
        /// 获得设备
        /// </summary>
        /// <param name="remoteIP"></param>
        /// <param name="workMode"></param>
        /// <returns></returns>
        public IRunDevice[] GetDevices(string remoteIP, WorkMode workMode)
        {
            List<IRunDevice> list = new List<IRunDevice>();
            foreach (KeyValuePair<string, IRunDevice> kv in _Devices)
            {
                if (kv.Value.CommunicateType == CommunicateType.NET)
                {
                    if (kv.Value.DeviceParameter.NET.WorkMode == workMode
                        &&
                       String.CompareOrdinal(kv.Value.DeviceParameter.NET.RemoteIP, remoteIP) == 0)
                    {
                        list.Add(kv.Value);
                    }
                }
            }
            return list.ToArray();
        }

        /// <summary>
        /// 获得设备列表
        /// </summary>
        /// <param name="workMode"></param>
        /// <returns></returns>
        public IRunDevice[] GetDevices(WorkMode workMode)
        {
            List<IRunDevice> list = new List<IRunDevice>();
            foreach (KeyValuePair<string, IRunDevice> kv in _Devices)
            {
                if (kv.Value.CommunicateType == CommunicateType.NET)
                {
                    if (kv.Value.DeviceParameter.NET.WorkMode == workMode)
                    {
                        list.Add(kv.Value);
                    }
                }
            }
            return list.ToArray();
        }

        /// <summary>
        /// 获得设备列表
        /// </summary>
        /// <param name="ioType"></param>
        /// <returns></returns>
        public IRunDevice[] GetDevices(CommunicateType ioType)
        {
            List<IRunDevice> list = new List<IRunDevice>();
            foreach (KeyValuePair<string, IRunDevice> kv in _Devices)
            {
                if (kv.Value.CommunicateType == ioType)
                {
                    list.Add(kv.Value);
                }
            }
            return list.ToArray();
        }

        /// <summary>
        /// 获得设备列表 
        /// </summary>
        /// <param name="devType"></param>
        /// <returns></returns>
        public IRunDevice[] GetDevices(DeviceType devType)
        {
            List<IRunDevice> list = new List<IRunDevice>();
            foreach (KeyValuePair<string, IRunDevice> kv in _Devices)
            {
                if (kv.Value.DeviceType == devType)
                {
                    list.Add(kv.Value);
                }
            }
            return list.ToArray();
        }

        /// <summary>
        /// 判断设备是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainDevice(string key)
        {
            return _Devices.ContainsKey(key);
        }

        /// <summary>
        /// 判断设备是否存在
        /// </summary>
        /// <param name="iopara1"></param>
        /// <param name="ioType"></param>
        /// <returns></returns>
        public bool ContainDevice(string iopara1, Communicate.CommunicateType ioType)
        {
            bool exist = false;  //初始为不存在设备
            foreach (KeyValuePair<string, IRunDevice> kv in _Devices)
            {
                if ((kv.Value.CommunicateType == ioType && String.CompareOrdinal(kv.Value.DeviceParameter.NET.RemoteIP, iopara1) == 0)
                    || (kv.Value.CommunicateType == ioType && String.CompareOrdinal(kv.Value.DeviceParameter.COM.Port.ToString(), iopara1) == 0))
                {
                    exist = true;
                    break;
                }
            }
            return exist;
        }

        /// <summary>
        /// 设备总数
        /// </summary>
        public int Count
        {
            get { return this._Devices.Count; }
        }

        /// <summary>
        /// 获得设备计数器
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int GetCounter(string key)
        {
            return _Counter.GetOrAdd(key, 0);
        }

        /// <summary>
        /// 设置设备计数器
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public int SetCounter(string key, int val)
        {
            return _Counter.AddOrUpdate(key, 0, (k, oldValue) => val);
        }

        public ILog Logger { get; private set; }

        public IEnumerator GetEnumerator()
        {
           return this._Devices.GetEnumerator();
        }
    }
}
