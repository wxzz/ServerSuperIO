using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerSuperIO.Communicate.COM
{
    /// <summary>
    /// 打开串口事件
    /// </summary>
    /// <param name="com"></param>
    /// <param name="port"></param>
    /// <param name="baud"></param>
    /// <param name="openSuccess"></param>
    internal delegate void COMOpenHandler(IComSession com, int port, int baud, bool openSuccess);

    /// <summary>
    /// 关闭串口事件
    /// </summary>
    /// <param name="com"></param>
    /// <param name="port"></param>
    /// <param name="baud"></param>
    /// <param name="closeSuccess"></param>
    internal delegate void COMCloseHandler(IComSession com, int port, int baud, bool closeSuccess);

    /// <summary>
    /// 串口错误
    /// </summary>
    /// <param name="com"></param>
    /// <param name="port"></param>
    /// <param name="baud"></param>
    /// <param name="error"></param>
    internal delegate void COMErrorHandler(IComSession com, int port, int baud, string error);

}
