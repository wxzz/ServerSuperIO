using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using ServerSuperIO.Protocol;
using ServerSuperIO.Protocol.Filter;

namespace ServerSuperIO.DataCache
{
    public interface IReceiveCache
    {
        /// <summary>
        /// 增加数据
        /// </summary>
        /// <param name="data"></param>
        void Add(byte[] data);

        /// <summary>
        /// 获得数据
        /// </summary>
        /// <returns></returns>
        byte[] Get();

        /// <summary>
        /// 获得数据
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        IList<byte[]> Get(IReceiveFilter filter);

        /// <summary>
        /// 下一下接收数据的起始点
        /// </summary>
        int NextOffset { get; }

        /// <summary>
        /// 接收缓冲区
        /// </summary>
        byte[] ReceiveBuffer { get; }

        /// <summary>
        /// 初始化下标偏移量
        /// </summary>
        int InitOffset { get; }

        /// <summary>
        /// 当前偏移量
        /// </summary>
        int CurrentOffset { get; }

        /// <summary>
        /// 有效数据长度
        /// </summary>
        int DataLength { set;get; }

        /// <summary>
        /// 在不调整的情况下，能容纳多少数
        /// </summary>
        int Capacity { get; }

        /// <summary>
        /// 置零
        /// </summary>
        void Reset();
    }
}
