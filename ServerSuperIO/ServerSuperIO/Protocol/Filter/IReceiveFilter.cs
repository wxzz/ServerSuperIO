using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSuperIO.Protocol.Filter
{ 
    public interface IReceiveFilter
    {
        /// <summary>
        /// 过滤数据信息
        /// </summary>
        /// <param name="receiveBuffer">缓冲区</param>
        /// <param name="offset">偏移量</param>
        /// <param name="length">有效数据长度</param>
        /// <param name="lastByteOffset">最后一个字节的偏移量</param>
        /// <returns>没有数据返回null</returns>
        IList<byte[]> Filter(byte[] receiveBuffer, int offset, int length, ref int lastByteOffset);
    }
}
