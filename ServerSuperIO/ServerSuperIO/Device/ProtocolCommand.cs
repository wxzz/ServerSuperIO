using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerSuperIO.Protocol;

namespace ServerSuperIO.Device
{
    public abstract class ProtocolCommand : IProtocolCommand
    {
        protected ProtocolCommand()
        {}
        /// <summary>
        /// 命令名称，唯一
        /// </summary>
        public abstract string Name { get; }


        /// <summary>
        /// 执行命令
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        public abstract void ExcuteCommand<T>(T t);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        public abstract void ExcuteCommand<T1, T2>(T1 t1, T2 t2);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="t"></param>
        /// <returns></returns>

        public abstract dynamic Analysis<T>(byte[] data, T t);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="data"></param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        public abstract dynamic Analysis<T1, T2>(byte[] data, T1 t1, T2 t2);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="code"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public abstract byte[] Package<T>(string code, T t);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="code"></param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        public abstract byte[] Package<T1, T2>(string code, T1 t1, T2 t2);


        /// <summary>
        /// 安装协议驱动
        /// </summary>
        /// <param name="driver"></param>
        public void Setup(IProtocolDriver driver)
        {
            ProtocolDriver = driver;
        }

        /// <summary>
        /// 协议驱动实例
        /// </summary>
        public IProtocolDriver ProtocolDriver { get; private set; }
    }
}
