using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ServerSuperIO.Config
{
    public class ShowInstance
    {
        /// <summary>
        /// 程序集标题
        /// </summary>
        [Category("1.视图实例"),
        DisplayName("ShowName"),
        Description("视图名称")]
        [XmlAttribute(AttributeName = "ShowName")]
        public string ShowName { get; set; }

        [Category("1.视图实例"),
        DisplayName("ShowKey"),
        Description("视图关键字")]
        [XmlAttribute(AttributeName = "ShowKey")]
        public string ShowKey { get; set; }

        /// <summary>
        /// 程序集文件 路径
        /// </summary>
        [Category("1.视图实例"),
        DisplayName("AssemblyFile"),
        Description("程序集文件的路径")]
        [XmlAttribute(AttributeName = "AssemblyFile")]
        public string AssemblyFile { get; set; }

        /// <summary>
        /// 实例，命名空间.类名称
        /// </summary>
        [Category("1.视图实例"),
        DisplayName("AssemblyInstance"),
        Description("视图的实例，包括命令空间和类名称")]
        [XmlAttribute(AttributeName = "AssemblyInstance")]
        public string AssemblyInstance { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Category("1.视图实例"),
        DisplayName("Remarks"),
        Description("备注信息")]
        [XmlAttribute(AttributeName = "Remarks")]
        public string Remarks { get; set; }
    }
}
