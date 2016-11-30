using System;

namespace ServerSuperIO.Communicate.COM
{
    [Serializable]
    public class COMParameter
    {
        public COMParameter()
        {
            Port = 1;
            Baud = 9600;
        }

        public COMParameter(int port ,int baud)
        {
            Port = port;
            Baud = baud;
        }

        public int Port { get; set; }

        public int Baud { get; set; }
    }
}
