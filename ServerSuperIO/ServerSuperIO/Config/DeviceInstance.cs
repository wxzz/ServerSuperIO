using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml;
using ServerSuperIO.Communicate;
using ServerSuperIO.Device;
using System.Xml.Serialization;
using ServerSuperIO.Communicate.NET;

namespace ServerSuperIO.Config
{
    [Serializable]
    public class DeviceInstance
    {
        public DeviceInstance()
        {
            DeviceID = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// 设备ID，一般为Guid
        /// </summary>
        [Category("1.挂载设备"),
        DisplayName("DeviceID"),
        Description("标识设备的唯ID，一般为Guid"),
        ReadOnly(true)]
        [XmlAttribute(AttributeName = "DeviceID")]
        public string DeviceID { get; set; }

        /// <summary>
        /// 通讯类型
        /// </summary>
        [Category("1.挂载设备"),
        DisplayName("CommunicateType"),
        Description("通讯类型，包括：COM和NET")]
        [XmlAttribute(AttributeName = "CommunicateType")]
        public CommunicateType CommunicateType { get; set; }


        [Category("1.挂载设备"),
         DisplayName("AssemblyID"),
         Description("标识设备驱动程序集的唯一ID，一般为Guid")]
        [XmlAttribute(AttributeName = "AssemblyID")]
        public string AssemblyID { get; set; }


        [Category("1.挂载设备"),
       DisplayName("DeviceName"),
       Description("设备名称，一般与Caption一致")]
        [XmlAttribute(AttributeName = "DeviceName")]
        public string DeviceName { set; get; }

        [Category("1.挂载设备"),
        DisplayName("IoParameter1"),
        Description("通讯参数1，如果是COM通讯则代表串口号，如果是NET通讯则代表IP地址")]
        [XmlAttribute(AttributeName = "IoParameter1")]
        public string IoParameter1 { set; get; }

        [Category("1.挂载设备"),
        DisplayName("IoParameter2"),
        Description("通讯参数2，如果是COM通讯则代表波特率，如果是NET通讯则代表端口号")]
        [XmlAttribute(AttributeName = "IoParameter2")]
        public int IoParameter2 { set; get; }

        [Category("1.挂载设备"),
        DisplayName("DeviceAddr"),
        Description("设备地址，指的是设备内部设置的地址信息，一般与DeviceCode一致")]
        [XmlAttribute(AttributeName = "DeviceAddr")]
        public int DeviceAddr { set; get; }

        [Category("1.挂载设备"),
        DisplayName("DeviceCode"),
        Description("设备编号，作为识别设备的唯一编号")]
        [XmlAttribute(AttributeName = "DeviceCode")]
        public string DeviceCode { set; get; }

        [Category("1.挂载设备"),
        DisplayName("WorkMode"),
        Description("工作模式，设置设备在网络通讯下是TcpServer模式，还是TcpClient模式")]
        [XmlAttribute(AttributeName = "WorkMode")]
        public WorkMode WorkMode { set; get; }
    }
}
