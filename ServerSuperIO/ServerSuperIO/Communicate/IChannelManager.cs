using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerSuperIO.Log;

namespace ServerSuperIO.Communicate
{
    public interface IChannelManager<TKey,TValue>:ILoggerProvider where TValue:IChannel
    {
        /// <summary>
        /// 同步对象
        /// </summary>
        object SyncLock { get; }

        /// <summary>
        /// 增加IO通道
        /// </summary>
        /// <param name="io"></param>
        bool AddChannel(TKey key, TValue channel);

        /// <summary>
        /// 获得可使用的IO通道
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        TValue GetChannel(TKey key);

        /// <summary>
        /// 获得指定的通道
        /// </summary>
        /// <param name="ioPara1"></param>
        /// <param name="comType"></param>
        /// <returns></returns>
        TValue GetChannel(string ioPara1, CommunicateType comType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ioPara1"></param>
        /// <param name="ioPara2"></param>
        /// <param name="comType"></param>
        /// <returns></returns>
        TValue GetChannel(string ioPara1, int ioPara2, CommunicateType comType);

        /// <summary>
        /// 获得指定IO类型的通道
        /// </summary>
        /// <param name="ioType"></param>
        /// <returns></returns>
        ICollection<TValue> GetChannels(CommunicateType ioType);

            /// <summary>
        /// 获得值集合
        /// </summary>
        /// <returns></returns>
        ICollection<TValue> GetValues();

        /// <summary>
        /// 获得值集合
        /// </summary>
        /// <returns></returns>
        ICollection<TKey> GetKeys();

        /// <summary>
        /// 判断该Key是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool ContainChannel(TKey key);

        /// <summary>
        /// 关闭IO通道
        /// </summary>
        /// <param name="key"></param>
        bool RemoveChannel(TKey key);

        /// <summary>
        /// 关闭所有IO
        /// </summary>
        void RemoveAllChannel();

        /// <summary>
        /// 
        /// </summary>
        int ChannelCount { get; }
    }
}
