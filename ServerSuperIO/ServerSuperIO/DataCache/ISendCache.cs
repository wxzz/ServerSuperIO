using System;
namespace ServerSuperIO.DataCache
{
    public interface ISendCache
    {
        /// <summary>
        /// 增加命令
        /// </summary>
        /// <param name="cmd"></param>
        void Add(ISendCommand cmd);

        /// <summary>
        /// 增加命令
        /// </summary>
        /// <param name="cmdkey"></param>
        /// <param name="cmdbytes"></param>
        void Add(string cmdkey, byte[] cmdbytes);

        /// <summary>
        /// 增加命令
        /// </summary>
        /// <param name="cmdkey"></param>
        /// <param name="cmdbytes"></param>
        /// <param name="priority"></param>
        void Add(string cmdkey, byte[] cmdbytes, Priority priority);

        /// <summary>
        /// 清空命令缓冲区
        /// </summary>
        void Clear();

        /// <summary>
        /// 命令总数 
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 获得命令
        /// </summary>
        /// <returns></returns>
        byte[] Get();

        /// <summary>
        /// 获得命令
        /// </summary>
        /// <param name="priority"></param>
        /// <returns></returns>
        byte[] Get(Priority priority);

        /// <summary>
        /// 获得命令
        /// </summary>
        /// <param name="cmdkey"></param>
        /// <returns></returns>
        byte[] Get(string cmdkey);

        /// <summary>
        /// 删除命令
        /// </summary>
        /// <param name="cmdkey"></param>
        void Remove(string cmdkey);
    }
}
