using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerSuperIO.Communicate;
using ServerSuperIO.Communicate.NET;
using ServerSuperIO.Config;
using ServerSuperIO.Server;
using ServerSuperIO.Service;
using TestDeviceDriver;
using TestService;

namespace TestSingletonMain
{
    class Program
    {
        private static object _obj=new object();
        private static volatile int _counter = 0;
        static void Main(string[] args)
        {
            //55 AA 00 61 43 7A 00 00 43 B4 15 0D
            string deviceID = "2";
            DeviceSingletonDriver dev3 = new DeviceSingletonDriver();
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
            DeviceSingletonDriver dev4 = new DeviceSingletonDriver();
            dev4.DeviceParameter.DeviceName = "设备3";
            dev4.DeviceParameter.DeviceAddr = 0;
            dev4.DeviceParameter.DeviceID = deviceID;
            dev4.DeviceParameter.DeviceCode = deviceID;
            dev4.DeviceDynamic.DeviceID = deviceID;
            dev4.DeviceParameter.NET.RemoteIP = "127.0.0.1";
            dev4.DeviceParameter.NET.RemotePort = 9600;
            dev4.DeviceParameter.NET.ControllerGroup = "G3";
            dev4.CommunicateType = CommunicateType.NET;
            dev4.Initialize(deviceID);

            IServer server = new ServerManager().CreateServer(new ServerConfig()
            {
                ServerName = "单例服务",
                NetReceiveBufferSize = 1024,
                DeliveryMode=DeliveryMode.DeviceCode,
                ControlMode = ControlMode.Singleton,
                MaxConnects = 5000
            });

            server.AddDeviceCompleted += server_AddDeviceCompleted;
            server.DeleteDeviceCompleted += server_DeleteDeviceCompleted;
            server.SocketConnected += server_SocketConnected;
            server.SocketClosed += server_SocketClosed;
            server.Start();

            server.AddDevice(dev3);
            //server.AddDevice(dev4);

            while ("exit" == Console.ReadLine())
            {
                server.Stop();
            }
        }

        private static void server_SocketClosed(string serverId,string ip, int port)
        {

                _counter--;
            Console.WriteLine(String.Format("{0},连接：{1}-{2} 断开", _counter, ip, port));
        }

        private static void server_SocketConnected(string serverId, string ip, int port)
        {

                _counter++;
                Console.WriteLine(String.Format("{0},连接：{1}-{2} 成功", _counter, ip, port));
        }

        private static void s_AppServiceLog(string log)
        {
            Console.WriteLine(log);
        }

        private static void server_AddDeviceCompleted(string serverId, string devid, string devName, bool isSuccess)
        {
            Console.WriteLine(devName + ",增加:" + isSuccess.ToString());
        }

        private static void server_DeleteDeviceCompleted(string serverId, string devid, string devName, bool isSuccess)
        {
            Console.WriteLine(devName + ",删除:" + isSuccess.ToString());
        }
    }
}
