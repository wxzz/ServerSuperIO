using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using ServerSuperIO.DataCache;
using ServerSuperIO.Protocol;
using ServerSuperIO.Protocol.Filter;

namespace ServerSuperIO.Communicate.NET
{
    public class SocketAsyncEventArgsEx : SocketAsyncEventArgs, ISocketAsyncEventArgsEx
    {
        private IReceiveCache _ReceiveCache = null;
        public void Initialize()
        {
            _ReceiveCache=new ReceiveCache(this.Buffer,this.Offset,this.Count);
        }

        public int Capacity
        {
            get { return _ReceiveCache.Capacity; }
        }

        public int DataLength
        {
            get { return _ReceiveCache.DataLength; }

            set { _ReceiveCache.DataLength = value; }
        }

        public int InitOffset
        {
            get { return _ReceiveCache.InitOffset; }
        }

        public int CurrentOffset
        {
            get { return _ReceiveCache.CurrentOffset; }
        }

        public int NextOffset
        {
            get { return _ReceiveCache.NextOffset; }
        }

        public byte[] ReceiveBuffer
        {
            get { return _ReceiveCache.ReceiveBuffer; }
        }

        public void Add(byte[] data)
        {
            _ReceiveCache.Add(data);
        }

        public byte[] Get()
        {
            return _ReceiveCache.Get();
        }

        public IList<byte[]> Get(IReceiveFilter filter)
        {
            return _ReceiveCache.Get(filter);
        }

        public void Reset()
        {
            _ReceiveCache.Reset();
        }
    }
}
