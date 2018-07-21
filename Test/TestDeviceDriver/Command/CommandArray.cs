using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDeviceDriver
{
    internal enum CommandArray:int
    {
       RealTimeData = 0x61,

       FileData = 0x62,

       ControlCommand=0x63, //控制命令

       BackControlCommand=0x64
    }
}
