using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerSuperIO.Common;

namespace ServerSuperIO.Protocol.Filter
{
    public class FixedHeadAndLengthReceiveFliter : IReceiveFilter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="headBytes"></param>
        /// <param name="length">包括SpliterBytes的长度</param>
        public FixedHeadAndLengthReceiveFliter(byte[] headBytes,int length)
        {
            HeadBytes = headBytes;
            Length = length;
        }

        /// <summary>
        /// 分割符字节数组
        /// </summary>
        private byte[] HeadBytes { set; get; }

        /// <summary>
        /// 数据长度
        /// </summary>
        private int Length { set;get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="receiveBuffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="lastByteOffset"></param>
        /// <returns></returns>
        public IList<byte[]> Filter(byte[] receiveBuffer, int offset, int length, ref int lastByteOffset)
        {
            int available = length;
            if (available >= Length)
            {
                IList<byte[]> listBytes = new List<byte[]>();
                int curMaxIndex = available + offset - HeadBytes.Length;
                int loopIndex = offset;
                while (loopIndex <= curMaxIndex)
                {
                    if (receiveBuffer.Mark(offset, available, loopIndex, HeadBytes))
                    {
                        byte[] data=new byte[Length];
                        Buffer.BlockCopy(receiveBuffer, loopIndex, data,0,data.Length);
                        listBytes.Add(data);

                        lastByteOffset = loopIndex + Length - 1;//排除中间有干扰而插入的数据。           
                        loopIndex += Length;               //下标移到截取数据最后数据位的下一个下标
                    }
                    else
                    {
                        loopIndex++;
                    }
                }

                return listBytes;
            }
            else
            {
                return null;
            }
        }
    }
}
