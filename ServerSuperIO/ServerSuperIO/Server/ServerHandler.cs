using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerSuperIO.Device;

namespace ServerSuperIO.Server
{
    /// <summary>
    /// 增加设备完成
    /// </summary>
    /// <param name="devid"></param>
    /// <param name="scldev"></param>
    public delegate void AddDeviceCompletedHandler(string devid, string devName,bool isSuccess);

    /// <summary>
    /// 删除设备完成
    /// </summary>
    /// <param name="devid"></param>
    /// <param name="scldev"></param>
    public delegate void DeleteDeviceCompletedHandler(string devid, string devName,bool isSuccess);

    /// <summary>
    /// 打开串口
    /// </summary>
    /// <param name="port"></param>
    /// <param name="baud"></param>
    public delegate void ComOpenedHandler(int port, int baud,bool openSuccess);

    /// <summary>
    /// 关闭串口
    /// </summary>
    /// <param name="port"></param>
    /// <param name="baud"></param>
    public delegate void ComClosedHandler(int port, int baud,bool closeSuccess);

    /// <summary>
    /// 网络连接
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="port"></param>
    public delegate void SocketConnectedHandler(string ip, int port);

    /// <summary>
    /// 网络连接关闭
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="port"></param>
    public delegate void SocketClosedHandler(string ip, int port);
}
