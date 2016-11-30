using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerSuperIO.Common;

namespace ServerSuperIO.Protocol.Filter
{
    public class FixedEndReceiveFliter : IReceiveFilter
    {
        public FixedEndReceiveFliter(byte[] endBytes)
        {
            EndBytes = endBytes;
        }

        private byte[] EndBytes { set; get; }

        public IList<byte[]> Filter(byte[] receiveBuffer, int offset, int length, ref int lastByteOffset)
        {
            List<int> ends = new List<int>();
            int maxIndex = offset + length - EndBytes.Length;
            int loopIndex = offset;
            while (loopIndex <= maxIndex)
            {
                if (receiveBuffer.Mark(offset, length, loopIndex, EndBytes))
                {
                    ends.Add(loopIndex);
                    loopIndex += EndBytes.Length;
                }
                else
                {
                    loopIndex++;
                }
            }

            int startOffset = offset;
            List<byte[]> listBytes = new List<byte[]>();
            for (int i = 0; i < ends.Count; i++)
            {
                int count = ends[i] + EndBytes.Length - startOffset;
                byte[] data = new byte[count];
                Buffer.BlockCopy(receiveBuffer, startOffset, data, 0, data.Length);
                listBytes.Add(data);

                startOffset = ends[i] + EndBytes.Length;
                lastByteOffset = startOffset - 1;
            }

            return listBytes;
        }
    }
}
