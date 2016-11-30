using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDeviceDriver
{
    public class DeviceSelfDriver:DeviceDriver
    {
        public DeviceSelfDriver() : base()
        {
            
        }
        public override void Initialize(string devid)
        {
            base.Initialize(devid);

            //如果测试接收文件，把这两行代码去掉
            //this.RunTimerInterval = 5000;
            //this.IsRunTimer = true;
        }

        public override void OnRunTimer()
        {
            byte[] data = this.GetSendBytes();
            OnSendData(data);
            base.OnRunTimer();
        }
    }
}
