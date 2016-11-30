using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerSuperIO.Common;

namespace ServerSuperIO.Protocol.Filter
{
    public class FixedHeadReceiveFliter : IReceiveFilter
    {
        public FixedHeadReceiveFliter(byte[] headBytes)
        {
            HeadBytes = headBytes;
        }

        private byte[] HeadBytes { set; get; }

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
            List<int> heads=new List<int>();
            int maxIndex = offset + length - HeadBytes.Length;
            int loopIndex = offset;
            while (loopIndex <= maxIndex)
            {
                if (receiveBuffer.Mark(offset, length, loopIndex, HeadBytes))
                {
                    heads.Add(loopIndex);
                    loopIndex += HeadBytes.Length;
                }
                else
                {
                    loopIndex++;
                }
            }

            List<byte[]> listBytes=new List<byte[]>();
            for(int i=0;i<heads.Count-1;i++)
            {
                int count = heads[i + 1] - heads[i];
                byte[] data=new byte[count];
                Buffer.BlockCopy(receiveBuffer,heads[i],data,0,data.Length);
                listBytes.Add(data);
                lastByteOffset = heads[i] + count - 1;
            }

            return listBytes;
        }
    }
}
