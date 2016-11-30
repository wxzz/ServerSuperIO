using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml;
using ServerSuperIO.Config;
using System.Xml.Serialization;

namespace ServerSuperIO.Config
{
    [Serializable]
    public class ServerInstance
    {
        public ServerInstance():this(new ServerConfig())
        {
        }

        public ServerInstance(ServerConfig serverConfig)
        {
            ServerConfig = serverConfig;
            DeviceInstanceList = new List<DeviceInstance>();
            ShowInstanceList=new List<ShowInstance>();
            ServiceInstanceList=new List<ServiceInstance>();
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        [XmlElement(ElementName = "ServerConfig")]
        public ServerConfig ServerConfig { get; set; }

        [XmlArray(ElementName = "DeviceInstanceList")]
        public List<DeviceInstance> DeviceInstanceList { get; set; }

        [XmlArray(ElementName = "ShowInstanceList")]
        public List<ShowInstance> ShowInstanceList { get; set; }

        [XmlArray(ElementName = "ServiceInstanceList")]
        public List<ServiceInstance> ServiceInstanceList { get; set; }
    }
}
