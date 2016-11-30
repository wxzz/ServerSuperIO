using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerSuperIO.Common;

namespace ServerSuperIO.Protocol.Filter
{
    public class FixedHeadAndEndReceiveFliter : IReceiveFilter
    {
        public FixedHeadAndEndReceiveFliter(byte[] headBytes, byte[] endBytes)
        {
            HeadBytes = headBytes;
            EndBytes = endBytes;
        }

        private class HeadAndEndMarked
        {
            public HeadAndEndMarked()
            {
                HeadIndex = -1;
                EndIndex = -1;
            }
            public int HeadIndex { set; get; }
            public int EndIndex { set; get; }
        }

        private byte[] HeadBytes { set; get; }

        private byte[] EndBytes { set; get; }

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
            int lastIndex = -1;
            int maxOffset = offset + length;
            int loopIndex = offset;
            List<byte[]> listBytes = new List<byte[]>();
            HeadAndEndMarked marked = null;
            while (loopIndex < maxOffset)
            {
                if (marked == null)
                {
                    marked = new HeadAndEndMarked();
                }

                if (marked.HeadIndex == -1)
                {
                    if (loopIndex <= maxOffset - HeadBytes.Length)
                    {
                        if (receiveBuffer.Mark(offset, length, loopIndex, HeadBytes))
                        {
                            marked.HeadIndex = loopIndex;
                            loopIndex += HeadBytes.Length;
                        }
                        else
                        {
                            loopIndex++;
                        }
                    }
                    else
                    {
                        loopIndex++;
                    }
                }
                else if (marked.HeadIndex >= 0 && marked.EndIndex == -1)
                {
                    if (loopIndex <= maxOffset - EndBytes.Length)
                    {
                        if (receiveBuffer.Mark(offset, length, loopIndex, EndBytes))
                        {
                            marked.EndIndex = loopIndex;
                            loopIndex += EndBytes.Length;
                        }
                        else
                        {
                            loopIndex++;
                        }
                    }
                    else
                    {
                        loopIndex++;
                    }
                }

                if (marked.HeadIndex >= 0 && marked.EndIndex > marked.HeadIndex)
                {
                    int count = marked.EndIndex - marked.HeadIndex + EndBytes.Length;
                    byte[] data = new byte[count];
                    Buffer.BlockCopy(receiveBuffer, marked.HeadIndex, data, 0, data.Length);
                    listBytes.Add(data);
                    lastIndex = marked.EndIndex;
                    marked = null;
                }
            }

            if (lastIndex != -1)
            {
                ////--------避免包数据结尾后有冗余数据信息,而占用缓存-------//
                //int getLength = lastIndex + EndBytes.Length;
                //int curLength = offset + length;
                //if (curLength > getLength)
                //{
                //    getLength += curLength - getLength;
                //}
                ////--------------------------------------------------//

                lastByteOffset = lastIndex + EndBytes.Length - 1;
            }
            return listBytes;
        }
    }
}
