using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerSuperIO.Communicate;
using ServerSuperIO.Communicate.NET;
using ServerSuperIO.Config;
using ServerSuperIO.Server;
using TransFileDriver;

namespace TransFileHost
{
    class Program
    {
        static void Main(string[] args)
        {
            
            ReceiveFileDriver dev = new ReceiveFileDriver();
            dev.DeviceParameter.DeviceName = "设备4";
            dev.DeviceParameter.DeviceAddr = 0;
            dev.DeviceParameter.DeviceCode = "0001";
            dev.DeviceParameter.DeviceID = "0";
            dev.DeviceDynamic.DeviceID = "0";
            dev.DeviceParameter.NET.RemoteIP = "127.0.0.1";
            dev.DeviceParameter.NET.RemotePort = 9600;
            dev.CommunicateType = CommunicateType.NET;
            dev.Initialize("0");

            IServer server = new ServerManager().CreateServer(new ServerConfig()
            {
                ServerName = "接收文件服务",
                ListenPort = 6699,
                NetReceiveBufferSize = 2048,
                ControlMode = ControlMode.Self,
                SocketMode = SocketMode.Tcp,
                DeliveryMode = DeliveryMode.DeviceCode,
                StartReceiveDataFliter = true,
                ClearSocketSession = false,
            });

            server.AddDeviceCompleted += server_AddDeviceCompleted;
            server.DeleteDeviceCompleted += server_DeleteDeviceCompleted;
            server.Start();

            server.AddDevice(dev);

            while ("exit" == Console.ReadLine())
            {
                server.Stop();
            }
        }

        private static void server_AddDeviceCompleted(string devid, string devName, bool isSuccess)
        {
            Console.WriteLine(devName + ",增加:" + isSuccess.ToString());
        }

        private static void server_DeleteDeviceCompleted(string devid, string devName, bool isSuccess)
        {
            Console.WriteLine(devName + ",删除:" + isSuccess.ToString());
        }
    }
}
