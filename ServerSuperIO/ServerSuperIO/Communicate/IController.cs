using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerSuperIO.Communicate;
using ServerSuperIO.Server;

namespace ServerSuperIO.Communicate
{
    public interface IController : IServerProvider,IDisposable
    {
        /// <summary>
        /// 当然是否工作
        /// </summary>
        bool IsWorking { set; get; }

        /// <summary>
        /// IO控制器的关键字
        /// </summary>
        string Key { get; }

        /// <summary>
        /// 启动服务
        /// </summary>
        void StartController();

        /// <summary>
        /// 停止服务
        /// </summary>
        void StopController();

        /// <summary>
        /// IO控制器类型
        /// </summary>
        CommunicateType ControllerType { get; }

        /// <summary>
        /// 是否释放资源
        /// </summary>
        bool IsDisposed { get; }
    }
}
