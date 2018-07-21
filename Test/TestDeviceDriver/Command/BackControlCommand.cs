using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerSuperIO.Common;
using ServerSuperIO.Device;

namespace TestDeviceDriver
{
    /// <summary>
    /// 设备实时数据命令
    /// </summary>
    internal class BackControlCommand : ProtocolCommand
    {
        public override string Name
        {
            get { return CommandArray.BackControlCommand.ToString(); }
        }

        public override void ExcuteCommand<T>(T t)
        {
            throw new NotImplementedException();
        }

        public override void ExcuteCommand<T1, T2>(T1 t1, T2 t2)
        {
            throw new NotImplementedException();
        }

        public override dynamic Analysis<T1,T2>(byte[] data, T1 t1,T2 t2)
        {
            throw new NotImplementedException();
        }

        public override byte[] Package<T>(string code, T t)
        {
            //发送：0x55 0xaa 0x00 0x61 0x61 0x0d
            byte[] data = new byte[6];
            data[0] = 0x55;
            data[1] = 0xaa;
            data[2] = byte.Parse(code);
            data[3] = 0x61;
            data[4] = this.ProtocolDriver.GetCheckData(data)[0];
            data[5] = 0x0d;
            return data;
        }

        public override byte[] Package<T1,T2>(string code, T1 t1,T2 t2)
        {
            throw new NotImplementedException();
        }

      

        public override dynamic Analysis<T>(byte[] data, T t)
        {
            return data[3];
        }

       
    }
}
