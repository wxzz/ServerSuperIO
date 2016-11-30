using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ServerSuperIO.Common;
using ServerSuperIO.Communicate;
using ServerSuperIO.Device;
using ServerSuperIO.Protocol;

namespace TestDeviceDriver
{
    internal class DeviceProtocol:ProtocolDriver
    {
        public override bool CheckData(byte[] data)
        {
            if (data[0] == 0x55 && data[1] == 0xaa && data[data.Length - 1] == 0x0d)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override byte[] GetCommand(byte[] data)
        {
            return new byte[] { data[3] };
        }

        public override int GetAddress(byte[] data)
        {
            return data[2];
        }

        public override byte[] GetHead(byte[] data)
        {
            return new byte[] { data[0], data[1] };
        }

        public override byte[] GetEnd(byte[] data)
        {
            return new byte[] { data[data.Length - 1] };
        }

        public override byte[] GetCheckData(byte[] data)
        {
            byte checkSum = 0;
            for (int i = 2; i < data.Length - 2; i++)
            {
                checkSum += data[i];
            }
            return new byte[] { checkSum };
        }

        public override string GetCode(byte[] data)
        {
            byte[] head = new byte[] {0x55, 0xaa};
            int codeIndex = data.Mark(0, data.Length, head);
            if (codeIndex == -1)
            {
                return String.Empty;
            }
            else
            {
                return data[codeIndex + head.Length].ToString();
            }
        }

        public override int GetPackageLength(byte[] data, IChannel channel, ref int readTimeout)
        {
            if (data == null || data.Length <= 0)
                return 0;

            readTimeout = 2000;

            if (CheckData(data))
            {
                try
                {
                    if (data[3] == 0x62) //发送文件请求
                    {
                        int length = BitConverter.ToInt32(new byte[] {data[4], data[5], data[6], data[7]}, 0);

                        if (length <= 1024*1024) //1M
                        {
                            int num = channel.Write(data);
                            if (num > 0)
                            {
                                Console.WriteLine("返回文件请求确认数据");
                                return length;
                            }
                            else
                            {
                                return 0;
                            }
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    else
                    {
                        return 0;
                    }
                }
                catch (Exception)
                {

                    return 0;
                }
            }
            else
            {
                Console.WriteLine("校验错误");
                return 0;
            }
        }
    }
}
