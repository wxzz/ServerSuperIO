using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerSuperIO.Common;

namespace ServerSuperIO.Communicate
{
    public enum DataOrientation
    {
        [EnumDescription("发送")]
        Send,
        [EnumDescription("接收")]
        Receive
    }
}
