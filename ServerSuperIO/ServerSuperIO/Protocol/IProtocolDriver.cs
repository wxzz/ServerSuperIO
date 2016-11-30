using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using ServerSuperIO.Communicate;
using ServerSuperIO.DataCache;
using ServerSuperIO.Device;
using ServerSuperIO.Protocol.Filter;

namespace ServerSuperIO.Protocol
{
    public interface IProtocolDriver
    {
        /// <summary>
        /// 初始化协议驱动
        /// </summary>
        /// <param name="cmdAsmType">带命令驱动的程序集类型</param>
        /// <param name="receiveFilter"></param>
        void InitDriver(Type cmdAsmType, IReceiveFilter receiveFilter);

        /// <summary>
        /// 获得协议命令
        /// </summary>
        /// <param name="cmdName"></param>
        /// <returns></returns>
        IProtocolCommand GetProcotolCommand(string cmdName);

        /// <summary>
        /// 驱动命令
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmdName"></param>
        /// <param name="t"></param>
        void DriverCommand<T>(string cmdName,T t);


  /// <summary>
  /// 
  /// </summary>
  /// <typeparam name="T1"></typeparam>
  /// <typeparam name="T2"></typeparam>
  /// <param name="cmdName"></param>
  /// <param name="t1"></param>
  /// <param name="t2"></param>
        void DriverCommand<T1,T2>(string cmdName, T1 t1,T2 t2);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmdName"></param>
        /// <param name="data"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        dynamic DriverAnalysis<T>(string cmdName, byte[] data, T t);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="cmdName"></param>
        /// <param name="data"></param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        dynamic DriverAnalysis<T1, T2>(string cmdName, byte[] data, T1 t1, T2 t2);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="code"></param>
        /// <param name="cmdName"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        byte[] DriverPackage<T>(string code, string cmdName, T t);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="code"></param>
        /// <param name="cmdName"></param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        byte[] DriverPackage<T1, T2>(string code, string cmdName, T1 t1, T2 t2);

        /// <summary>
        /// 数据校验
        /// </summary>
        /// <param name="data">输入接收到的数据</param>
        /// <returns>true:校验成功 false:校验失败</returns>
        bool CheckData(byte[] data);

        /// <summary>
        /// 获得校验数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        byte[] GetCheckData(byte[] data);

        /// <summary>
        /// 获得命令集全，如果命令和命令参数
        /// </summary>
        /// <param name="data">输入接收到的数据</param>
        /// <returns>返回命令集合</returns>
        byte[] GetCommand(byte[] data);

        /// <summary>
        /// 获得地址
        /// </summary>
        /// <param name="data">输入接收到的数据</param>
        /// <returns>返回地址</returns>
        int GetAddress(byte[] data);

        /// <summary>
        /// 获得ID信息，是该传感器的唯一标识。2016-07-29新增加（wxzz)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        string GetCode(byte[] data);

        /// <summary>
        /// 获得应该接收的数据长度，如果当前接收的数据小于这个返回值，那么继续接收数据，直到大于等于这个返回长度。
        /// </summary>
        /// <param name="data">接收的数据</param>
        /// <param name="channel">IO通道</param>
        /// <param name="readTimeout">返回读数据超时</param>
        /// <returns></returns>
        int GetPackageLength(byte[] data,IChannel channel,ref int readTimeout);

        /// <summary>
        /// 协议头
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        byte[] GetHead(byte[] data);

        /// <summary>
        /// 协议尾
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        byte[] GetEnd(byte[] data);

        /// <summary>
        /// 命令缓存器，把要发送的命令数据放到这里，框架会自动提取数据进行发送
        /// </summary>
        ISendCache SendCache { get; }

        /// <summary>
        /// 协议过滤器
        /// </summary>
        IReceiveFilter ReceiveFilter { set;get; }
    }
}
