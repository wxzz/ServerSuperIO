using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServerSuperIO.Protocol.Filter;
using ServerSuperIO.Server;

namespace ServerSuperIO.Communicate
{
    public abstract class BaseChannel : ServerProvider,IChannel
    {
        private object _SyncLock = new object();
        public abstract CommunicateType CommunicationType { get; }
        public abstract bool IsDisposed { get; }
        public abstract string Key { get; }
        public abstract string SessionID { get; protected set; }
        public object SyncLock {
            get { return _SyncLock;}
        }

        /// <summary>
        /// 读包长数据
        /// </summary>
        /// <param name="dataLength"></param>
        /// <param name="readTimeout"></param>
        /// <returns></returns>
        internal byte[] ReceivePackageData(int dataLength,int readTimeout)
        {
            byte[] bigData = null;
            CancellationTokenSource cts = new CancellationTokenSource();
            Task<byte[]> task = ReadAsync(dataLength, cts);
            task.Wait(readTimeout);
            if (task.IsCompleted)
            {
                bigData = task.Result;
            }
            else
            {
                cts.Cancel(true);
                if (!task.IsFaulted)
                {
                    bigData = task.Result;
                }
            }
            return bigData;
        }

        public abstract void Initialize();

        public abstract IChannel Channel { get; }
        public abstract void Close();
        public abstract void Dispose();
        
        public abstract int Write(byte[] data);

        /// <summary>
        /// 过滤读取数据
        /// </summary>
        /// <param name="receiveFilter"></param>
        /// <returns></returns>
        public abstract IList<byte[]> Read(IReceiveFilter receiveFilter);


        /// <summary>
        /// 异步读取只写长度的数据
        /// </summary>
        /// <param name="dataLength"></param>
        /// <param name="cts"></param>
        /// <returns></returns>
        protected abstract Task<byte[]> ReadAsync(int dataLength, CancellationTokenSource cts);
    }
}
