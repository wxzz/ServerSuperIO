using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ServerSuperIO.Communicate
{
    public interface IRequestInfo
    {
        /// <summary>
        /// 关键字，一般为IP地址或是串口号。
        /// </summary>
        string Key { get; }

        /// <summary>
        /// 一般是命令数据。
        /// </summary>
        byte[] Data { get; }

        /// <summary>
        /// IO通道
        /// </summary>
        IChannel Channel { get;}

        /// <summary>
        /// 启动StartCheckPackageLength之后，二次接收的数据。
        /// </summary>
        byte[] BigData { get; }
    }
}
