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
using TestDeviceDriver;
using TestService;

namespace TestSelfMain
{
    class Program
    {
        static void Main(string[] args)
        {

            string deviceID = "0";
            DeviceSelfDriver dev = new DeviceSelfDriver();
            dev.DeviceParameter.DeviceName = "设备2";
            dev.DeviceParameter.DeviceAddr = 0;
            dev.DeviceParameter.DeviceID = deviceID;
            dev.DeviceParameter.DeviceCode = deviceID;
            dev.DeviceDynamic.DeviceID = deviceID;
            dev.DeviceParameter.NET.RemoteIP = "127.0.0.1";
            dev.DeviceParameter.NET.RemotePort = 9600;
            dev.DeviceParameter.NET.ControllerGroup = "G2";
            dev.CommunicateType = CommunicateType.NET;
            dev.DeviceParameter.NET.WorkMode = WorkMode.TcpServer;
            dev.Initialize(deviceID);


            deviceID = "2";
            DeviceSelfDriver dev3 = new DeviceSelfDriver();
            dev3.DeviceParameter.DeviceName = "设备2";
            dev3.DeviceParameter.DeviceAddr = 0;
            dev3.DeviceParameter.DeviceID = deviceID;
            dev3.DeviceParameter.DeviceCode = deviceID;
            dev3.DeviceDynamic.DeviceID = deviceID;
            dev3.DeviceParameter.NET.RemoteIP = "127.0.0.1";
            dev3.DeviceParameter.NET.RemotePort = 9600;
            dev3.DeviceParameter.NET.ControllerGroup = "G2";
            dev3.CommunicateType = CommunicateType.NET;
            dev3.DeviceParameter.NET.WorkMode = WorkMode.TcpServer;
            dev3.Initialize(deviceID);

            deviceID = "3";
            DeviceSelfDriver dev4 = new DeviceSelfDriver();
            dev4.DeviceParameter.DeviceName = "设备3";
            dev4.DeviceParameter.DeviceAddr = 0;
            dev4.DeviceParameter.DeviceID = deviceID;
            dev4.DeviceParameter.DeviceCode = "0";
            dev4.DeviceDynamic.DeviceID = deviceID;
            dev4.DeviceParameter.NET.RemoteIP = "172.16.37.2";
            dev4.DeviceParameter.NET.RemotePort = 9600;
            dev4.DeviceParameter.NET.ControllerGroup = "G3";
            dev4.CommunicateType = CommunicateType.NET;
            dev4.Initialize(deviceID);

            IServer server = new ServerManager().CreateServer(new ServerConfig()
            {
                ServerName = "服务1",
                ComReadTimeout = 1000,
                ComWriteTimeout = 1000,
                NetReceiveTimeout = 1000,
                NetSendTimeout = 1000,
                ControlMode = ControlMode.Self,
                SocketMode = SocketMode.Tcp,
                ReceiveDataFliter = false,
                ClearSocketSession = false,
                CheckPackageLength = true,
                CheckSameSocketSession = false,
                DeliveryMode = DeliveryMode.DeviceCode,
               
            });

            server.AddDeviceCompleted += server_AddDeviceCompleted;
            server.DeleteDeviceCompleted += server_DeleteDeviceCompleted;
            server.Start();

            //server.AddDevice(dev);
            //server.AddDevice(dev3);
            server.AddDevice(dev4);

            //for (int i = 0; i < 100; i++)
            //{
            //    string code = i.ToString();
            //    DeviceSelfDriver rdev = new DeviceSelfDriver();
            //    rdev.DeviceParameter.DeviceName = "网络设备" + code;
            //    rdev.DeviceParameter.DeviceAddr = i;
            //    rdev.DeviceParameter.DeviceID = code;
            //    rdev.DeviceDynamic.DeviceID = code;
            //    rdev.DeviceParameter.DeviceCode = code;
            //    rdev.DeviceParameter.NET.RemoteIP = "127.0.0.1";
            //    rdev.DeviceParameter.NET.RemotePort = 9600;
            //    rdev.CommunicateType = CommunicateType.NET;
            //    rdev.Initialize(code);
            //    server.AddDevice(rdev);
            //}

            //TestService.Service service=new TestService.Service();
            //service.IsAutoStart = true;
            //server.AddService(service);

            //ServerSuperIO.ControlDeviceService.ControlService service = new ServerSuperIO.ControlDeviceService.ControlService();
            //service.IsAutoStart = true;
            //server.AddService(service);

            while ("exit" == Console.ReadLine())
            {
                server.Stop();
            }
        }

        private static void s_AppServiceLog(string log)
        {
            Console.WriteLine(log);
        }

        private static void server_DeleteDeviceCompleted(string serverSession, string devId, string devName, bool isSuccess)
        {
            Console.WriteLine(devName + ",删除:" + isSuccess.ToString());
        }

        private static void server_AddDeviceCompleted(string serverSession, string devId, string devName, bool isSuccess)
        {
            Console.WriteLine(devName + ",增加:" + isSuccess.ToString());
        }
    }
}
