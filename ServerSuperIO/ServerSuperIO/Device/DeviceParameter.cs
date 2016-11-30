using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerSuperIO.Common;
using ServerSuperIO.Communicate.COM;
using ServerSuperIO.Communicate.NET;
using ServerSuperIO.Persistence;

namespace ServerSuperIO.Device
{
    [Serializable]
    public abstract class DeviceParameter:XmlPersistence,IDeviceParameter
    {
        protected DeviceParameter()
        {
            DeviceID = String.Empty;
            DeviceAddr = -1;
            DeviceName=String.Empty;
            IsSaveOriginBytes = false;
            IsAlert = false;
            IsAlertSound = false;
            COM = new COMParameter(1, 9600);
            NET = new SocketParameter("127.0.0.1", 4001, WorkMode.TcpServer);
            DataFormat = "0.000";
            VirtualFormat = String.Empty;
        }

        /// <summary>
        /// 设备ID
        /// </summary>
        public string DeviceID { get; set; }

        /// <summary>
        /// 设备地址
        /// </summary>
        public int DeviceAddr { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// 是否保存原始数据
        /// </summary>
        public bool IsSaveOriginBytes { get; set; }

        /// <summary>
        /// 是否报警
        /// </summary>
        public bool IsAlert { get; set; }

        /// <summary>
        /// 报警，是否有声音提示
        /// </summary>
        public bool IsAlertSound { get; set; }

        /// <summary>
        /// 串口参数
        /// </summary>
        public COMParameter COM { get; set; }

        /// <summary>
        /// 网络参数
        /// </summary>
        public SocketParameter NET { get; set; }

        /// <summary>
        /// 统一数据格式
        /// </summary>
        public string DataFormat { get; set; }

        /// <summary>
        /// 虚拟设备的时候，设置计算公式
        /// </summary>
        public string VirtualFormat { get; set; }

        /// <summary>
        /// 保存参数的路径
        /// </summary>
        public override string SavePath
        {
            get
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "ServerSuperIO/Parameter";
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                return String.Format("{0}/{1}.xml", path, this.DeviceID.ToString());
            }
        }

        /// <summary>
        /// 设备编码，手动设置且唯一
        /// </summary>
        public string DeviceCode { get; set; }
    }
}
