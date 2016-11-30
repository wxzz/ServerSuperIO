using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerSuperIO.Device;

namespace ServerSuperIO.Communicate.NET
{
    internal interface ISocketController : IController
    {
        /// <summary>
        /// 发送数据事件
        /// </summary>
        /// <param name="dev"></param>
        /// <param name="data"></param>
        void Send(IRunDevice dev, byte[] data);

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="socketSession"></param>
        /// <param name="dataPackage"></param>
        void Receive(ISocketSession socketSession, IReceivePackage dataPackage);

        ///// <summary>
        ///// 关闭Socket事件
        ///// </summary>
        //event CloseSocketHandler CloseSocket;
    }
}
