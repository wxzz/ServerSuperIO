using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerSuperIO.Communicate;
using ServerSuperIO.Communicate.NET;
using ServerSuperIO.Config;
using ServerSuperIO.Device;
using ServerSuperIO.Server;
using ServerSuperIO.Service;
using ServerSuperIO.Show;
using TestDeviceDriver;
using TestService;
using TestShowForm;

namespace TestLoopMain
{
    class Program
    {
        static void Main(string[] args)
        {
            DeviceDriver dev1 = new DeviceDriver();
            dev1.DeviceParameter.DeviceName = "串口设备1";
            dev1.DeviceParameter.DeviceAddr = 0;
            dev1.DeviceParameter.DeviceID = "0";
            dev1.DeviceParameter.DeviceCode = "0";
            dev1.DeviceDynamic.DeviceID = "0";
            dev1.DeviceParameter.COM.Port = 1;
            dev1.DeviceParameter.COM.Baud = 9600;
            dev1.CommunicateType = CommunicateType.COM;
            dev1.Initialize("0");

            //DeviceDriver dev2 = new DeviceDriver();
            //dev2.DeviceParameter.DeviceName = "设备2";
            //dev2.DeviceParameter.DeviceAddr = 0;
            //dev2.DeviceParameter.DeviceID = 1;
            //dev2.DeviceDynamic.DeviceID = 1;
            //dev2.DeviceParameter.COM.Port = 3;
            //dev2.DeviceParameter.COM.Baud = 9600;
            //dev2.CommunicateType = CommunicateType.COM;
            //dev2.Initialize(1);

            //DeviceDriver dev3 = new DeviceDriver();
            //dev3.DeviceParameter.DeviceName = "设备3";
            //dev3.DeviceParameter.DeviceAddr = 0;
            //dev3.DeviceParameter.DeviceID = 2;
            //dev3.DeviceDynamic.DeviceID = 2;
            //dev3.DeviceParameter.NET.RemoteIP = "127.0.0.1";
            //dev3.DeviceParameter.NET.RemotePort = 9600;
            //dev3.CommunicateType = CommunicateType.NET;
            //dev3.DeviceParameter.NET.WorkMode = WorkMode.TcpClient;
            //dev3.Initialize(2);

            DeviceDriver dev4 = new DeviceDriver();
            dev4.DeviceParameter.DeviceName = "网络设备2";
            dev4.DeviceParameter.DeviceAddr = 0;
            dev4.DeviceParameter.DeviceID = "3";
            dev4.DeviceDynamic.DeviceID = "3";
            dev4.DeviceParameter.NET.RemoteIP = "127.0.0.1";
            dev4.DeviceParameter.NET.RemotePort = 9600;
            dev4.CommunicateType = CommunicateType.NET;
            dev4.Initialize("3");

            IServer server = new ServerManager().CreateServer(new ServerConfig()
            {
                ServerName = "服务1",
                ComReadTimeout = 1000,
                ComWriteTimeout = 1000,
                NetReceiveTimeout = 1000,
                NetSendTimeout = 1000,
                ControlMode = ControlMode.Loop,
                SocketMode = SocketMode.Tcp,
                StartReceiveDataFliter = false,
                ClearSocketSession = false,
                StartCheckPackageLength = false,
                CheckSameSocketSession = false,
            });

            server.AddDeviceCompleted += server_AddDeviceCompleted;
            server.DeleteDeviceCompleted += server_DeleteDeviceCompleted;
            server.SocketConnected+=server_SocketConnected;
            server.SocketClosed+=server_SocketClosed;
            server.Start();

            server.AddDevice(dev1);
            //server.AddDevice(dev2);
            //server.AddDevice(dev3);
           // server.AddDevice(dev4);
            //server.RemoveDevice(3);//删除设备 

            TestService.Service service =new TestService.Service();
            server.AddService(service);


            while ("exit"==Console.ReadLine())
            {
                 server.Stop();
            }
        }

        private static void server_SocketClosed(string ip, int port)
        {
            Console.WriteLine(String.Format("断开：{0}-{1} 成功", ip, port));
        }

        private static void server_SocketConnected(string ip, int port)
        {
            Console.WriteLine(String.Format("连接：{0}-{1} 成功",ip, port));
        }

        private static void s_AppServiceLog(string log)
        {
           Console.WriteLine(log);
        }

        private static void server_AddDeviceCompleted(string devid, string devName, bool isSuccess)
        {
            Console.WriteLine(devName+",增加:"+isSuccess.ToString());
        }

        private static void server_DeleteDeviceCompleted(string devid, string devName, bool isSuccess)
        {
            Console.WriteLine(devName + ",删除:" + isSuccess.ToString());
        }
    }
}
