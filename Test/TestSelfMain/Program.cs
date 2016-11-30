using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ServerSuperIO.Communicate;
using ServerSuperIO.Communicate.NET;
using ServerSuperIO.Config;
using ServerSuperIO.Server;
using ServerSuperIO.Service;
using ServerSuperIO.Show;
using TestDeviceDriver;
using TestService;
using TestShowForm;

namespace TestSelfMain
{
    class Program
    {
        static void Main(string[] args)
        {
            //DeviceSelfDriver dev1 = new DeviceSelfDriver();
            //dev1.DeviceParameter.DeviceName = "串口设备";
            //dev1.DeviceParameter.DeviceAddr = 0;
            //dev1.DeviceParameter.DeviceID = "0";
            //dev1.DeviceDynamic.DeviceID = "0";
            //dev1.DeviceParameter.DeviceCode = "0";
            //dev1.DeviceParameter.COM.Port = 1;
            //dev1.DeviceParameter.COM.Baud = 9600;
            //dev1.CommunicateType = CommunicateType.COM;
            //dev1.Initialize("0");

            DeviceSelfDriver dev2 = new DeviceSelfDriver();
            dev2.DeviceParameter.DeviceName = "网络设备";
            dev2.DeviceParameter.DeviceAddr = 1;
            dev2.DeviceParameter.DeviceID = "1";
            dev2.DeviceDynamic.DeviceID = "1";
            dev2.DeviceParameter.DeviceCode = "1";
            dev2.DeviceParameter.NET.RemoteIP = "127.0.0.1";
            dev2.DeviceParameter.NET.RemotePort = 9600;
            dev2.CommunicateType = CommunicateType.NET;
            dev2.Initialize("1");

            IServer server = new ServerManager().CreateServer(new ServerConfig()
            {
                ServerName = "服务1",
                ComReadTimeout = 1000,
                ComWriteTimeout = 1000,
                NetReceiveTimeout = 1000,
                NetSendTimeout = 1000,
                ControlMode = ControlMode.Self,
                SocketMode = SocketMode.Tcp,
                StartReceiveDataFliter = true,
                ClearSocketSession = false,
                StartCheckPackageLength = true,
                CheckSameSocketSession = false,
                DeliveryMode = DeliveryMode.DeviceCode,
            });

            server.AddDeviceCompleted += server_AddDeviceCompleted;
            server.DeleteDeviceCompleted+=server_DeleteDeviceCompleted;
            server.Start();

            //server.AddDevice(dev1);
            server.AddDevice(dev2);

            TestService.Service service=new TestService.Service();
            service.IsAutoStart = true;
            server.AddService(service);


            while ("exit" == Console.ReadLine())
            {
                server.Stop();
            }
        }

        private static void s_AppServiceLog(string log)
        {
            Console.WriteLine(log);
        }

        private static void server_DeleteDeviceCompleted(string devid, string devName, bool isSuccess)
        {
            Console.WriteLine(devName + ",删除:" + isSuccess.ToString());
        }

        private static void server_AddDeviceCompleted(string devid, string devName, bool isSuccess)
        {
            Console.WriteLine(devName + ",增加:" + isSuccess.ToString());
        }
    }
}
