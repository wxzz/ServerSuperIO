using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ServerSuperIO.Common
{
    public static class BinaryExtenstion
    {
        /// <summary>
        /// 在缓存内移动有效数据
        /// </summary>
        /// <param name="buffer">缓存</param>
        /// <param name="currentOffset">有效数据开始下标</param>
        /// <param name="length">有效数据长度</param>
        /// <param name="destOffset">移动的目标下标</param>
        /// <returns>负数：向左移动；0：不移动；正数：向右移动</returns>
        public static int Move(this byte[] buffer, int currentOffset, int length, int destOffset)
        {
            if (buffer == null || buffer.Length == 0)
            {
                return 0;
            }

            if (currentOffset < 0 || currentOffset > buffer.Length - 1)
            {
                throw new IndexOutOfRangeException("currentOffset超出数组范围");
            }

            if (length == 0)
            {
                return 0;
            }

            if (destOffset < 0 || destOffset > buffer.Length - 1)
            {
                throw new IndexOutOfRangeException("destOffset超出数组范围");
            }

            int lastIndex = ((length + currentOffset) - 1);
            if (destOffset >= currentOffset && destOffset <= lastIndex)
            {
                throw new ArgumentOutOfRangeException("destOffset不在有效的范围内");
            }

            int moveOffset = 0;
            if (destOffset < currentOffset)
            {
                moveOffset = destOffset - currentOffset;  //为负数，向左移动
                for (int i = currentOffset; i <= lastIndex; i++)
                {
                    buffer[i + moveOffset] = buffer[i];
                }
            }
            else if (destOffset > lastIndex)
            {
                moveOffset = destOffset - lastIndex;
                for (int i = lastIndex; i >= currentOffset; i--)
                {
                    buffer[i + moveOffset] = buffer[i];
                }
            }
            return moveOffset;
        }

        /// <summary>
        /// 比对数据是事相等
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="startIndex">要比较的buffer开始下标</param>
        /// <param name="markBytes">要比较的目标字节数组</param>
        /// <returns></returns>
        public static bool Mark(this byte[] buffer, int offset, int length, int startIndex, byte[] markBytes)
        {
            if (buffer == null || buffer.Length == 0)
            {
                return false;
            }

            if (offset < 0 || offset > buffer.Length - 1)
            {
                throw new IndexOutOfRangeException("offset超出数组范围");
            }

            if (length == 0)
            {
                return false;
            }

            int maxIndex = offset + length - markBytes.Length; //确定最大下标
            if (startIndex < offset || startIndex > maxIndex)
            {
                throw new ArgumentOutOfRangeException("startIndex不在有效的范围内");
            }

            if (markBytes == null || markBytes.Length == 0)
            {
                throw new NullReferenceException("markBytes引用为空");
            }

            if (length < markBytes.Length) //没有可比对的数据   
            {
                return false;
            }

            bool equal = true;
            for (int i = 0; i < markBytes.Length; i++)
            {
                if (buffer[startIndex + i] != markBytes[i])
                {
                    equal = false;
                }

                if (!equal)
                {
                    break;
                }
            }
            return equal;
        }

        public static int Mark(this byte[] buffer, int offset, int length, byte[] markBytes)
        {
            int indexOffset = -1;
            int count = length - markBytes.Length;
            for (int i = offset; i < count; i++)
            {
                if (Mark(buffer, offset, length, i, markBytes))
                {
                    indexOffset = i;
                    break;
                }
            }
            return indexOffset;
        }
    }
}
