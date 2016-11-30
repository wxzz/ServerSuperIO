using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSuperIO.Protocol.Filter
{
    public class FixedLengthReceiveFliter : IReceiveFilter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="length">固定长度</param>
        public FixedLengthReceiveFliter(int length)
        {
            Length = length;
        }

        /// <summary>
        /// 固定长度
        /// </summary>
        private int Length { set; get; }

        /// <summary>
        /// 过滤数据信息
        /// </summary>
        /// <param name="receiveBuffer">缓冲区</param>
        /// <param name="offset">偏移量</param>
        /// <param name="length">有效数据长度</param>
        /// <param name="lastByteOffset">取出最后一个字节的偏移量</param>
        /// <returns>没有数据返回null</returns>
        public IList<byte[]> Filter(byte[] receiveBuffer, int offset, int length,ref int lastByteOffset)
        {
            int available = length;
            if (available >= Length)
            {
                int num = (int)(available / Length);
                IList<byte[]> listBytes=new List<byte[]>();
                for (int i = 0; i < num; i++)
                {
                    byte[] data=new byte[Length];
                    Buffer.BlockCopy(receiveBuffer,i* Length + offset,data,0,data.Length);
                    listBytes.Add(data);
                    lastByteOffset = offset + ((i+1)*Length) - 1;//最后一下字节的偏移量
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
