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
using ServerSuperIO.Show;
using TestDeviceDriver;
using TestService;
using TestShowForm;

namespace TestSingletonMain
{
    class Program
    {
        private static object _obj=new object();
        private static volatile int _counter = 0;
        static void Main(string[] args)
        {
            //55 AA 00 61 43 7A 00 00 43 B4 15 0D
            DeviceSingletonDriver dev1 = new DeviceSingletonDriver();
            dev1.DeviceParameter.DeviceName = "网络设备";
            dev1.DeviceParameter.DeviceAddr = 0;
            dev1.DeviceParameter.DeviceID = "0";
            dev1.DeviceDynamic.DeviceID = "0";
            dev1.DeviceParameter.DeviceCode = "0";
            dev1.DeviceParameter.NET.RemoteIP = "127.0.0.1";
            dev1.DeviceParameter.NET.RemotePort = 9600;
            dev1.CommunicateType = CommunicateType.NET;
            dev1.Initialize("0");

            IServer server = new ServerManager().CreateServer(new ServerConfig()
            {
                ServerName = "单例服务",
                NetReceiveBufferSize = 1024,
                ControlMode = ControlMode.Singleton,
                MaxConnects = 4000
            });

            server.AddDeviceCompleted += server_AddDeviceCompleted;
            server.DeleteDeviceCompleted += server_DeleteDeviceCompleted;
            server.SocketConnected+=server_SocketConnected;
            server.SocketClosed+=server_SocketClosed;
            server.Start();

            server.AddDevice(dev1);

            while ("exit" == Console.ReadLine())
            {
                server.Stop();
            }
        }

        private static void server_SocketClosed(string ip, int port)
        {

                _counter--;
            Console.WriteLine(String.Format("{0},连接：{1}-{2} 断开", _counter, ip, port));
        }

        private static void server_SocketConnected(string ip, int port)
        {

                _counter++;
                Console.WriteLine(String.Format("{0},连接：{1}-{2} 成功", _counter, ip, port));
        }

        private static void s_AppServiceLog(string log)
        {
            Console.WriteLine(log);
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
