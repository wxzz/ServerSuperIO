using System;
using ServerSuperIO.DataCache;

namespace ServerSuperIO.DataCache
{
    public class SendCommand : ServerSuperIO.DataCache.ISendCommand 
    {
        private byte[] _Bytes = new byte[] { };

        /// <summary>
        /// 命令
        /// </summary>
        public byte[] Bytes
        {
            get { return _Bytes; }
        }

        private string _Key = String.Empty;
        /// <summary>
        /// 命令名称
        /// </summary>
        public string Key
        {
            get { return _Key; }
        }

        private Priority _Priority = Priority.Normal;
        /// <summary>
        /// 发送优先级，暂时不用
        /// </summary>
        public Priority Priority
        {
            get { return _Priority; }
        }

        /// <summary>
        /// 设备命令
        /// </summary>
        /// <param name="cmdkeys">命令名称</param>
        /// <param name="cmdbytes">命令字节数组</param>
        public SendCommand(string cmdkey, byte[] cmdbytes)
        {
            this._Key = cmdkey;
            this._Bytes = cmdbytes;
            this._Priority = Priority.Normal;
        }

        /// <summary>
        /// 设备命令
        /// </summary>
        /// <param name="cmdkeys">命令名称</param>
        /// <param name="cmdbytes">命令字节数组</param>
        public SendCommand(string cmdkey, byte[] cmdbytes,Priority priority)
        {
            this._Key = cmdkey;
            this._Bytes = cmdbytes;
            this._Priority = priority;
        }
    }
}
