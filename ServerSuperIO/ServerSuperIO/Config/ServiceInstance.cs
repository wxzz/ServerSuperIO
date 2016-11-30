using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ServerSuperIO.Config
{
    public class ServiceInstance
    {
        /// <summary>
        /// 程序集标题
        /// </summary>
        [Category("1.服务实例"),
        DisplayName("ServiceName"),
        Description("服务名称")]
        [XmlAttribute(AttributeName = "ServiceName")]
        public string ServiceName { get; set; }

        [Category("1.服务实例"),
        DisplayName("ServiceKey"),
        Description("服务关键字")]
        [XmlAttribute(AttributeName = "ServiceKey")]
        public string ServiceKey { get; set; }

        [Category("1.服务实例"),
        DisplayName("IsAutoStart"),
        Description("标识服务加载时是否自动起动")]
        [XmlAttribute(AttributeName = "IsAutoStart")]
        public bool IsAutoStart { get; set; }

        /// <summary>
        /// 程序集文件 路径
        /// </summary>
        [Category("1.服务实例"),
        DisplayName("AssemblyFile"),
        Description("程序集文件的路径")]
        [XmlAttribute(AttributeName = "AssemblyFile")]
        public string AssemblyFile { get; set; }

        /// <summary>
        /// 实例，命名空间.类名称
        /// </summary>
        [Category("1.服务实例"),
        DisplayName("AssemblyInstance"),
        Description("服务的实例，包括命令空间和类名称")]
        [XmlAttribute(AttributeName = "AssemblyInstance")]
        public string AssemblyInstance { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Category("1.服务实例"),
        DisplayName("Remarks"),
        Description("备注信息")]
        [XmlAttribute(AttributeName = "Remarks")]
        public string Remarks { get; set; }
    }
}
