using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using ServerSuperIO.Communicate.COM;

namespace ServerSuperIO.Config
{
    [Serializable]
    public class GlobalConfig
    {
        public GlobalConfig()
        {
            Caption = "";
            Ver = "";
            Copyright = "";
            LinuxComList=new List<LinuxCom>();
            DeviceAssemblyList = new List<DeviceAssembly>();
            ServerInstanceList = new List<ServerInstance>();
        }


        [DisplayName("1.Caption"),
        Description("软件名称")]
        [XmlElement(ElementName = "Caption")]
        public string Caption { set; get; }

        [DisplayName("2.Ver"),
         Description("软件版本")]
        [XmlElement(ElementName = "Ver")]
        public string Ver { get; set; }

        [DisplayName("3.Copyright"),
        Description("软件版权")]
        [XmlElement(ElementName = "Copyright")]
        public string Copyright { set; get; }

        [DisplayName("4.DeviceAssemblyList"),
        Description("当前可用的设备驱动集合")]
        [XmlArray(ElementName = "DeviceAssemblyList")]
        public List<DeviceAssembly> DeviceAssemblyList{ get; set; }

        [DisplayName("5.ServerInstanceList"),
        Description("配置服务实例，包括服务配置信息和挂载的设备驱动")]
        [XmlArray(ElementName = "ServerInstanceList")]
        public List<ServerInstance> ServerInstanceList { get; set; }

        [DisplayName("6.LinuxCom"),
  Description("Linux下串口配置，标识当前串口号对应是USB串口硬件，还是System系统串串口硬件")]
        [XmlArray(ElementName = "LinuxComList")]
        public List<LinuxCom> LinuxComList { set; get; }
    }
}
