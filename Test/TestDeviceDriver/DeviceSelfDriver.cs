using ServerSuperIO.Communicate;
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
        public override void Initialize(object obj)
        {
            base.Initialize(obj);

            ////如果测试接收文件，把这两行代码去掉
            //this.RunTimerInterval = 1000;
            //this.IsRunTimer = true;
        }

        public override void OnRunTimer()
        {
            IList<IRequestInfo> cmdList = this.GetSendBytes();
            OnSendData(cmdList[0], ServerSuperIO.WebSocket.WebSocketFrameType.Binary);
            base.OnRunTimer();
        }
    }
}
