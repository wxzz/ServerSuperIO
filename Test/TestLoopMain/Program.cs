using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerSuperIO.Common.Assembly;
using ServerSuperIO.Communicate;
using ServerSuperIO.Communicate.NET;
using ServerSuperIO.Config;
using ServerSuperIO.Device;
using ServerSuperIO.Graphics;
using ServerSuperIO.Server;
using ServerSuperIO.Service;
using TestDeviceDriver;
using TestService;

namespace TestLoopMain
{
    class Program
    {
        static void Main(string[] args)
        {
            string devid = "899d716b-59b7-4b9f-89d7-181017fe897c";
            DeviceDriver dev1 = new DeviceDriver();
            dev1.DeviceParameter.DeviceName = "串口设备1";
            dev1.DeviceParameter.DeviceAddr = 0;
            dev1.DeviceParameter.DeviceID = devid;
            dev1.DeviceParameter.DeviceCode = "0";
            dev1.DeviceDynamic.DeviceID = devid;
            dev1.DeviceParameter.COM.Port = 1;
            dev1.DeviceParameter.COM.Baud = 9600;
            dev1.DeviceParameter.COM.Baud = 8;
            dev1.DeviceParameter.COM.StopBits = System.IO.Ports.StopBits.One;
            dev1.DeviceParameter.COM.Parity = System.IO.Ports.Parity.None;
            dev1.CommunicateType = CommunicateType.COM;
            dev1.Initialize(devid);

            //DeviceDriver dev2 = new DeviceDriver();
            //dev2.DeviceParameter.DeviceName = "设备2";
            //dev2.DeviceParameter.DeviceAddr = 0;
            //dev2.DeviceParameter.DeviceID = 1;
            //dev2.DeviceDynamic.DeviceID = 1;
            //dev2.DeviceParameter.COM.Port = 3;
            //dev2.DeviceParameter.COM.Baud = 9600;
            //dev2.CommunicateType = CommunicateType.COM;
            //dev2.Initialize(1);

            string deviceID = "0";
            //DeviceSelfDriver dev = new DeviceSelfDriver();
            //dev.DeviceParameter.DeviceName = "设备2";
            //dev.DeviceParameter.DeviceAddr = 0;
            //dev.DeviceParameter.DeviceID = deviceID;
            //dev.DeviceParameter.DeviceCode = deviceID;
            //dev.DeviceDynamic.DeviceID = deviceID;
            //dev.DeviceParameter.NET.RemoteIP = "127.0.0.1";
            //dev.DeviceParameter.NET.RemotePort = 9600;
            //dev.DeviceParameter.NET.ControllerGroup = "G2";
            //dev.CommunicateType = CommunicateType.NET;
            //dev.DeviceParameter.NET.WorkMode = WorkMode.TcpServer;
            //dev.Initialize(deviceID);

            deviceID = "2";
            DeviceDriver dev3 = new DeviceDriver();
            dev3.DeviceParameter.DeviceName = "设备2";
            dev3.DeviceParameter.DeviceAddr = 0;
            dev3.DeviceParameter.DeviceID = deviceID;
            dev3.DeviceParameter.DeviceCode = deviceID;
            dev3.DeviceDynamic.DeviceID = deviceID;
            dev3.DeviceParameter.NET.RemoteIP = "127.0.0.1";
            dev3.DeviceParameter.NET.RemotePort = 9600;
            dev3.DeviceParameter.NET.ControllerGroup = "G";
            dev3.CommunicateType = CommunicateType.NET;
            dev3.DeviceParameter.NET.WorkMode = WorkMode.TcpServer;
            dev3.Initialize(deviceID);

            deviceID = "3";
            DeviceDriver dev4 = new DeviceDriver();
            dev4.DeviceParameter.DeviceName = "设备3";
            dev4.DeviceParameter.DeviceAddr = 0;
            dev4.DeviceParameter.DeviceID = deviceID;
            dev4.DeviceParameter.DeviceCode = "0";
            dev4.DeviceDynamic.DeviceID = deviceID;
            dev4.DeviceParameter.NET.RemoteIP = "127.0.0.1";
            dev4.DeviceParameter.NET.RemotePort = 9600;
            dev4.DeviceParameter.NET.ControllerGroup = "G";
            dev4.CommunicateType = CommunicateType.NET;
            dev4.Initialize(deviceID);

            IServer server = new ServerManager().CreateServer(new ServerConfig()
            {
                ServerName = "服务1",
                ComReadTimeout = 1000,
                ComWriteTimeout = 1000,
                NetReceiveTimeout = 1000,
                NetSendTimeout = 1000,
                ControlMode = ControlMode.Loop,
                SocketMode = SocketMode.Tcp,
                DeliveryMode=DeliveryMode.DeviceCode,
                ReceiveDataFliter = true,
                ClearSocketSession = true,
                CheckPackageLength = false,
                CheckSameSocketSession = false,
            });

            server.AddDeviceCompleted += server_AddDeviceCompleted;
            server.DeleteDeviceCompleted += server_DeleteDeviceCompleted;
            server.SocketConnected+=server_SocketConnected;
            server.SocketClosed+=server_SocketClosed;
            server.Start();
            
            server.AddDevice(dev1);
           // server.AddDevice(dev2);
            //server.AddDevice(dev3);
            server.AddDevice(dev4);
            //server.RemoveDevice(3);//删除设备 

            //ServerSuperIO.ControlDeviceService.ControlService service = new ServerSuperIO.ControlDeviceService.ControlService();
            //service.IsAutoStart = true;
            //server.AddService(service);

            while ("exit"==Console.ReadLine())
            {
                 server.Stop();
            }
        }

        private static void server_SocketClosed(string serverSession,string ip, int port)
        {
            Console.WriteLine(String.Format("断开：{0}-{1} 成功", ip, port));
        }

        private static void server_SocketConnected(string serverSession, string ip, int port)
        {
            Console.WriteLine(String.Format("连接：{0}-{1} 成功",ip, port));
        }

        private static void s_AppServiceLog(string log)
        {
           Console.WriteLine(log);
        }

        private static void server_AddDeviceCompleted(string serverSession, string devid, string devName, bool isSuccess)
        {
            Console.WriteLine(devName+",增加:"+isSuccess.ToString());
        }

        private static void server_DeleteDeviceCompleted(string serverSession, string devid, string devName, bool isSuccess)
        {
            Console.WriteLine(devName + ",删除:" + isSuccess.ToString());
        }
    }
}
