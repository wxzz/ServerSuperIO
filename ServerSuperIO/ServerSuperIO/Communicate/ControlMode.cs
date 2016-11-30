using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerSuperIO.Common;

namespace ServerSuperIO.Communicate
{
    /// <summary>
    /// 运行控制方式
    /// </summary>
    public enum ControlMode
    {
        [EnumDescription("循环模式")]
        Loop = 0x00,
        [EnumDescription("并发模式")]
        Parallel,
        [EnumDescription("自主模式")]
        Self,
        [EnumDescription("单例模式")]
        Singleton
    }
}
