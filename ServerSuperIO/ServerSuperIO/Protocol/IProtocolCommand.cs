using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ServerSuperIO.Protocol
{
    public interface IProtocolCommand
    {

        /// <summary>
        /// 安装协议驱动
        /// </summary>
        /// <param name="driver"></param>
        void Setup(IProtocolDriver driver);

        /// <summary>
        /// 获得协议驱动
        /// </summary>
        IProtocolDriver ProtocolDriver { get; }

        /// <summary>
        /// 命令名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        void ExcuteCommand<T>(T t);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        void ExcuteCommand<T1, T2>(T1 t1, T2 t2);

        /// <summary>
        /// 解析数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        dynamic Analysis<T>(byte[] data, T t);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="data"></param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        dynamic Analysis<T1,T2>(byte[] data, T1 t1,T2 t2);

        /// <summary>
        /// 打包数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="code"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        byte[] Package<T>(string code, T t);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="code"></param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        byte[] Package<T1, T2>(string code, T1 t1, T2 t2);
    }
}
