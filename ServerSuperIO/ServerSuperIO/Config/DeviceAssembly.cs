using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using ServerSuperIO.Device;

namespace ServerSuperIO.Config
{
    [Serializable]
    public class DeviceAssembly
    {
        public DeviceAssembly()
        {
            AssemblyID = Guid.NewGuid().ToString();
        }

        [Category("1.设备驱动"),
         DisplayName("AssemblyID"),
         Description("标识设备驱动程序集的唯一ID，一般为Guid"),
         ReadOnly(true)]
        [XmlAttribute(AttributeName = "AssemblyID")]
        public string AssemblyID { get; set; }

        /// <summary>
        /// 程序集文件 路径
        /// </summary>
        [Category("1.设备驱动"),
        DisplayName("AssemblyFile"),
        Description("程序集文件的路径")]
        [XmlAttribute(AttributeName = "AssemblyFile")]
        public string AssemblyFile { get; set; }

        /// <summary>
        /// 实例，命名空间.类名称
        /// </summary>
        [Category("1.设备驱动"),
        DisplayName("AssemblyInstance"),
        Description("设备驱动的实例，包括命令空间和类名称")]
        [XmlAttribute(AttributeName = "AssemblyInstance")]
        public string AssemblyInstance { get; set; }

        /// <summary>
        /// 程序集标题
        /// </summary>
        [Category("1.设备驱动"),
        DisplayName("Caption"),
        Description("设备的标题")]
        [XmlAttribute(AttributeName = "Caption")]
        public string Caption { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        [Category("1.设备驱动"),
        DisplayName("DeviceType"),
        Description("设备类型，包括：Common和Virtual")]
        [XmlAttribute(AttributeName = "DeviceType")]
        public DeviceType DeviceType { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Category("1.设备驱动"),
        DisplayName("Remarks"),
        Description("备注信息")]
        [XmlAttribute(AttributeName = "Remarks")]
        public string Remarks { get; set; }
    }
}
