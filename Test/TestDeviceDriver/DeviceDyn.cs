using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using ServerSuperIO.Device;

namespace TestDeviceDriver
{
    [Serializable]
    public class DeviceDyn:DeviceDynamic
    {
        public DeviceDyn() : base()
        {
            Dyn=new Dyn();
        }

        public override string GetAlertState()
        {
            throw new NotImplementedException("无报警信息");
        }

        public override object Repair()
        {
            return new DeviceDyn();
        }

        public Dyn Dyn { get; set; }
    }
}
