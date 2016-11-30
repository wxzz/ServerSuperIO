using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ServerSuperIO.DataCache;

namespace ServerSuperIO.DataCache
{
    /// <summary>
    /// 线程安全的轻量泛型类提供了从一组键到一组值的映射。
    /// </summary>
    /// <typeparam name="TKey">字典中的键的类型</typeparam>
    /// <typeparam name="TValue">字典中的值的类型</typeparam>
    public class SendCache : ServerSuperIO.DataCache.ISendCache
    {
        #region Fields
        /// <summary>
        /// 内部的 Dictionary 容器
        /// </summary>
        private List<ISendCommand> _CmdCache = new List<ISendCommand>();
        /// <summary>
        /// 用于并发同步访问的 RW 锁对象
        /// </summary>
        private ReaderWriterLock _rwLock = new ReaderWriterLock();
        /// <summary>
        /// 一个 TimeSpan，用于指定超时时间。 
        /// </summary>
        private readonly TimeSpan _lockTimeOut = TimeSpan.FromMilliseconds(100);
        #endregion

        #region Methods
        /// <summary>
        /// 将指定的键和值添加到字典中。
        /// Exceptions：
        ///     ArgumentException - Dictionary 中已存在具有相同键的元素。
        /// </summary>
        /// <param name="key">要添加的元素的键。</param>
        /// <param name="value">添加的元素的值。对于引用类型，该值可以为 空引用</param>
        public void Add(string cmdkey, byte[] cmdbytes)
        {
            this.Add(cmdkey, cmdbytes, Priority.Normal);
        }

        public void Add(string cmdkey, byte[] cmdbytes, Priority priority)
        {
            _rwLock.AcquireWriterLock(_lockTimeOut);
            try
            {
                SendCommand cmd = new SendCommand(cmdkey, cmdbytes,priority);
                this._CmdCache.Add(cmd);
            }
            finally { _rwLock.ReleaseWriterLock(); }
        }

        public void Add(ISendCommand cmd)
        {
            _rwLock.AcquireWriterLock(_lockTimeOut);
            try
            {
                if (cmd == null) return;

                this._CmdCache.Add(cmd);
            }
            finally { _rwLock.ReleaseWriterLock(); }
        }

        /// <summary>
        /// 删除命令
        /// </summary>
        /// <param name="cmdkey"></param>
        public void Remove(string cmdkey)
        {
            if (_CmdCache.Count <= 0)
            {
                return;
            }

            _rwLock.AcquireWriterLock(_lockTimeOut);
            try
            {
                ISendCommand cmd = this._CmdCache.FirstOrDefault(c => c.Key == cmdkey);
                if(cmd!=null)
                {
                    this._CmdCache.Remove(cmd);
                }
            }
            finally { _rwLock.ReleaseWriterLock(); }
        }

        /// <summary>
        /// 中移除所有的键和值。
        /// </summary>
        public void Clear()
        {
            if (this._CmdCache.Count > 0)
            {
                _rwLock.AcquireWriterLock(_lockTimeOut);
                try
                {
                    this._CmdCache.Clear();
                }
                finally { _rwLock.ReleaseWriterLock(); }
            }
        }

        /// <summary>
        /// 按优先级获得命令
        /// </summary>
        /// <param name="priority"></param>
        /// <returns></returns>
        public byte[] Get(Priority priority)
        {
            if (this._CmdCache.Count <= 0)
            {
                return new byte[] {};
            }

            _rwLock.AcquireReaderLock(_lockTimeOut);
            try
            {
                byte[] data = new byte[] { };
                if (priority == Priority.Normal)
                {
                    data = this._CmdCache[0].Bytes;
                    this._CmdCache.RemoveAt(0);
                }
                else if(priority==Priority.High)
                {
                    ISendCommand cmd = this._CmdCache.FirstOrDefault(c => c.Priority == Priority.High);
                    if (cmd != null)
                    {
                        data = cmd.Bytes;
                        this._CmdCache.Remove(cmd);
                    }
                }
                return data;
            }
            finally
            {
                _rwLock.ReleaseReaderLock();
            }
        }

        public byte[] Get(string cmdkey)
        {
            if (this._CmdCache.Count <= 0)
            {
                return new byte[] { };
            }

            _rwLock.AcquireReaderLock(_lockTimeOut);
            try
            {
                ISendCommand cmd=this._CmdCache.FirstOrDefault(c => c.Key == cmdkey);
                if (cmd == null)
                {
                    return new byte[] { };
                }
                else
                {
                    byte[] data = cmd.Bytes;
                    this._CmdCache.Remove(cmd);
                    return data;
                }
            }
            finally
            {
                _rwLock.ReleaseReaderLock();
            }
        }

        /// <summary>
        /// 顺序获得命令
        /// </summary>
        /// <returns></returns>
        public byte[] Get()
        {
            return Get(Priority.Normal);
        }

        public int Count
        {
            get { return _CmdCache.Count; }
        }
        #endregion
    }
}
