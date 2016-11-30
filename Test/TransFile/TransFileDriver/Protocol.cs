using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerSuperIO.Common;
using ServerSuperIO.Communicate;
using ServerSuperIO.Device;

namespace TransFileDriver
{
    public class Protocol: ProtocolDriver
    {
        public static byte[] Head {
            get
            {
                return new byte[] {0x35,0x35};
            }
        }
        public static byte[] End
        {
            get
            {
                return new byte[] { 0x33, 0x33 };
            }
        }

        public override bool CheckData(byte[] data)
        {
            byte[] head = Head;
            byte[] end = End;
            if (data[0] == head[0] && data[1] == head[1] && data[data.Length - 2] == end[0] && data[data.Length - 1] == end[1])
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override byte[] GetCheckData(byte[] data)
        {
            throw new NotImplementedException();
        }

        public override string GetCode(byte[] data)
        {
            int codeIndex = data.Mark(0,data.Length,Head);
            
            if (codeIndex == -1)
            {
                return String.Empty;
            }
            else
            {
                byte[] codebytes=new byte[4];
                Buffer.BlockCopy(data,codeIndex+2, codebytes,0,codebytes.Length);
                return System.Text.Encoding.ASCII.GetString(codebytes);
            }
        }

        public override int GetPackageLength(byte[] data, IChannel channel, ref int readTimeout )
        {
            throw new NotImplementedException();
        }

        public override byte[] GetHead(byte[] data)
        {
            throw new NotImplementedException();
        }

        public override byte[] GetEnd(byte[] data)
        {
            throw new NotImplementedException();
        }

        public override byte[] GetCommand(byte[] data)
        {
            throw new NotImplementedException();
        }

        public override int GetAddress(byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}
