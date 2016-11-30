using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerSuperIO.Common;
using ServerSuperIO.Config;

namespace ServerSuperIO.Communicate.COM
{
    public class ComUtils
    {
        private static readonly string WinComPrefix = "COM";
        private static readonly string LinuxUsbComPrefix = "/dev/ttyUSB";
        private static readonly string LinuxSysComPrefix = "/dev/ttyS";

        public static string PortToString(int port)
        {
            string prefix=String.Empty;
            OperatingSystemType plat = Common.OperatingSystem.GetOperatingSystemType();
            if (plat==OperatingSystemType.Windows)
            {
                prefix = WinComPrefix;
            }
            else if (plat == OperatingSystemType.Linux)
            {
                prefix = LinuxSysComPrefix;
                LinuxCom linuxCom = GlobalConfigTool.GlobalConfig.LinuxComList.FirstOrDefault(l => l.LinuxPort == port);
                if (linuxCom != null)
                {
                    if (linuxCom.LinuxComType == LinuxComType.Usb)
                    {
                        prefix = LinuxUsbComPrefix;
                    }
                    else if (linuxCom.LinuxComType == LinuxComType.System)
                    {
                        prefix = LinuxSysComPrefix;
                    }
                }
            }
            return String.Format("{0}{1}",prefix,port.ToString());
        }

        public static int PortToInt(string portString)
        {
            string prefix = String.Empty;
            OperatingSystemType plat = Common.OperatingSystem.GetOperatingSystemType();
            if (plat == OperatingSystemType.Windows)
            {
                prefix = WinComPrefix;
            }
            else if (plat == OperatingSystemType.Linux)
            {
                prefix = LinuxSysComPrefix;
                LinuxCom linuxCom = GlobalConfigTool.GlobalConfig.LinuxComList.FirstOrDefault(l => portString.Contains(l.LinuxPort.ToString()));
                if (linuxCom != null)
                {
                    if (linuxCom.LinuxComType == LinuxComType.Usb)
                    {
                        prefix = LinuxUsbComPrefix;
                    }
                    else if (linuxCom.LinuxComType == LinuxComType.System)
                    {
                        prefix = LinuxSysComPrefix;
                    }
                }
            }

            if (prefix.Length > 0)
            {
                return int.Parse(portString.Substring(prefix.Length));
            }
            else
            {
                throw new IndexOutOfRangeException("串口字符串无法转换");
            }
        }
    }
}
