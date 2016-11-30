using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;

namespace ServerSuperIO.Common
{

    public enum OperatingSystemType
    {
        Windows = 0,
        Linux
    }

    public static class OperatingSystem
    {
        public static OperatingSystemType GetOperatingSystemType()
        {
            OperatingSystemType type = OperatingSystemType.Windows;
            string osType = Environment.OSVersion.Platform.ToString();
            switch (osType)
            {
                case "Win32NT":
                    type = OperatingSystemType.Windows;
                    break;
                case "Unix":
                    type = OperatingSystemType.Linux;
                    break;
            }
            return type;
        }
    }
}
