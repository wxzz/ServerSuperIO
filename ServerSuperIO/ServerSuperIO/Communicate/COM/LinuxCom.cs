using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerSuperIO.Communicate.COM
{
    /// <summary>
    /// 标识linux下串口类型
    /// </summary>
    public enum LinuxComType
    {
        /// <summary>
        /// 标识串口为USB类型
        /// </summary>
        Usb,
        /// <summary>
        /// 标识串口为System系统类型
        /// </summary>
        System
    }

    /// <summary>
    /// 配置Linux串口信息
    /// </summary>
    public class LinuxCom
    {
        /// <summary>
        /// Linux下串口号
        /// </summary>
        public int LinuxPort { set; get; }

        /// <summary>
        /// Linux下串口类型
        /// </summary>
        public LinuxComType LinuxComType { set; get; }
    }
}
