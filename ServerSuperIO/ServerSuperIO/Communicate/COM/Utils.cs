using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerSuperIO.Communicate.COM
{
    public class Utils
    {
        public static string PortToString(int port)
        {
            return String.Format("COM{0}", port.ToString());
        }

        public static int PortToInt(string port)
        {
            if (port.Length > 3)
            {
                return int.Parse(port.Substring(3));
            }
            else
            {
                throw new IndexOutOfRangeException("串口字符串无法转换");
            }
        }
    }
}
