using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using ServerSuperIO.Common;
using ServerSuperIO.Protocol;

namespace ServerSuperIO.DataCache
{
    public class ReceiveCache1 : IReceiveCache
    {
        private object _SyncLock;

        public ReceiveCache1()
        {
            _SyncLock=new object();
            CurrentOffset = 0;
            InitOffset = 0;
            DataLength = 0;
            Capacity = 1024;
            ReceiveBuffer = new byte[Capacity];
        }

        public ReceiveCache1(int capacity)
        {
            _SyncLock = new object();
            CurrentOffset = 0;
            InitOffset = 0;
            DataLength = 0;
            Capacity = capacity;
            ReceiveBuffer = new byte[Capacity];
        }

        public ReceiveCache1(byte[] receiveBuffer, int offset, int capacity)
        {
            _SyncLock = new object();
            CurrentOffset = offset;
            InitOffset = offset;
            DataLength = 0;
            Capacity = capacity;
            ReceiveBuffer = receiveBuffer;
        }

        /// <summary>
        /// 下一下接收数据的起始点
        /// </summary>
        public int NextOffset
        {
            get { return InitOffset + DataLength; }
        }

        /// <summary>
        /// 接收缓冲区
        /// </summary>
        public byte[] ReceiveBuffer { private set; get; }

        /// <summary>
        /// 在不调整的情况下，能容纳多少数
        /// </summary>
        public int Capacity { private set; get; }

        /// <summary>
        /// 有效数据长度
        /// </summary>
        public int DataLength { set; get; }

        /// <summary>
        /// 下标偏移量
        /// </summary>
        public int InitOffset {  get; }

        /// <summary>
        /// 当前下标
        /// </summary>
        public int CurrentOffset { private set; get; } //当前有效数据开始的下标偏移量

        /// <summary>
        /// 增加数据
        /// </summary>
        /// <param name="data"></param>
        public void Add(byte[] data)
        {
            lock (_SyncLock)
            {
                MoveLeft(CurrentOffset- InitOffset);

                int remainLength = Capacity - (DataLength + InitOffset);
                if (data.Length > remainLength)
                {
                    Buffer.BlockCopy(data, 0, ReceiveBuffer, NextOffset, remainLength); //可能丢失数据
                    DataLength += remainLength;
                }
                else
                {
                    Buffer.BlockCopy(data, 0, ReceiveBuffer, NextOffset, data.Length);
                    DataLength += data.Length;
                }
            }
        }

        /// <summary>
        /// 获得数据
        /// </summary>
        /// <returns></returns>
        public byte[] Get()
        {
            if (DataLength <= 0)
            {
                return new byte[] {};
            }

            lock (_SyncLock)
            {
                byte[] data = new byte[DataLength];
                Buffer.BlockCopy(ReceiveBuffer, InitOffset, data, 0, data.Length);
                DataLength = 0;
                CurrentOffset = InitOffset;
                return data;
            }
        }

        /// <summary>
        /// 获得数据
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public IList<byte[]> Get(IReceiveFilter filter)
        {
            if (filter == null)
            {
                throw new NullReferenceException("filter引用为空");
            }

            if (DataLength <= 0)
            {
                return new List<byte[]>();
            }

            lock (_SyncLock)
            {
                int lastByteOffset = InitOffset;
                IList<byte[]> listBytes = filter.Filter(ReceiveBuffer, InitOffset, DataLength, ref lastByteOffset);
                if (listBytes != null 
                    && listBytes.Count > 0
                    && lastByteOffset>InitOffset)
                {
                    CurrentOffset = lastByteOffset + 1;

                    int gets = CurrentOffset - InitOffset ;
                    DataLength -= gets;

                    MoveLeft(gets);
                }
                return listBytes;
            }
        }

        public void Reset()
        {
            CurrentOffset = InitOffset;
            DataLength = 0;
        }

        private void MoveLeft(int gets)
        {
            if (CurrentOffset > InitOffset && CurrentOffset < (InitOffset+Capacity))
            {
                if (DataLength <= 0)
                {
                    Reset();
                }
                else
                {
                    if (Math.Abs(ReceiveBuffer.Move(CurrentOffset, DataLength, InitOffset)) == Math.Abs(gets)) //把数据移动到最开始的下标。
                    {
                        CurrentOffset = InitOffset;
                    }
                }
            }
            else if (CurrentOffset < InitOffset || CurrentOffset > Capacity)
            {
                Reset();
            }
        }
    }
}
