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
using TestDeviceDriver;
using TestService;

namespace TestParallelMain
{
    class Program
    {
        static void Main(string[] args)
        {
            IServer server = new ServerManager().CreateServer(new ServerConfig()
            {
                ServerName = "服务1",
                ComReadTimeout = 1000,
                ComWriteTimeout = 1000,
                NetReceiveTimeout = 1000,
                NetSendTimeout = 1000,
                ControlMode = ControlMode.Parallel,
                SocketMode = SocketMode.Tcp,
                ReceiveDataFliter = false,
                ClearSocketSession = false,
                CheckPackageLength = false,
                CheckSameSocketSession = false,
                DeliveryMode = DeliveryMode.DeviceIP,
                ParallelInterval = 1000
            });

            server.SocketConnected += server_SocketConnected;
            server.SocketClosed += server_SocketClosed;
            server.AddDeviceCompleted += server_AddDeviceCompleted;
            server.DeleteDeviceCompleted += server_DeleteDeviceCompleted;
            server.Start();

            string devCode = "0";
            DeviceDriver dev1 = new DeviceDriver();
            dev1.DeviceParameter.DeviceName = "设备驱动"+ devCode.ToString();
            dev1.DeviceParameter.DeviceAddr = int.Parse(devCode);
            dev1.DeviceParameter.DeviceCode = devCode.ToString();
            dev1.DeviceParameter.DeviceID = devCode.ToString();
            dev1.DeviceDynamic.DeviceID = devCode.ToString();
            dev1.DeviceParameter.NET.RemoteIP = "127.0.0.1";
            dev1.DeviceParameter.NET.RemotePort = 9600;
            dev1.CommunicateType = CommunicateType.NET;
            dev1.Initialize(devCode.ToString());
            server.AddDevice(dev1);

            //devCode = "1";
            //DeviceDriver dev2 = new DeviceDriver();
            //dev2.DeviceParameter.DeviceName = "设备驱动" + devCode.ToString();
            //dev2.DeviceParameter.DeviceAddr = int.Parse(devCode);
            //dev2.DeviceParameter.DeviceCode = devCode.ToString();
            //dev2.DeviceParameter.DeviceID = devCode.ToString();
            //dev2.DeviceDynamic.DeviceID = devCode.ToString();
            //dev2.DeviceParameter.NET.RemoteIP = "192.168.1.102";
            //dev2.DeviceParameter.NET.RemotePort = 9600;
            //dev2.CommunicateType = CommunicateType.NET;
            //dev2.Initialize(devCode.ToString());
            //server.AddDevice(dev2);

            while ("exit" == Console.ReadLine())
            {
                server.Stop();
            }
        }

        private static void s_AppServiceLog(string log)
        {
            Console.WriteLine(log);
        }

        private static void server_AddDeviceCompleted(string sessionId,string devid, string devName, bool isSuccess)
        {
            Console.WriteLine(devName + ",增加:" + isSuccess.ToString());
        }

        private static void server_DeleteDeviceCompleted(string sessionId, string devid, string devName, bool isSuccess)
        {
            Console.WriteLine(devName + ",删除:" + isSuccess.ToString());
        }

        private static void server_SocketClosed(string sessionId, string ip, int port)
        {

               
                Console.WriteLine(String.Format("连接：{0}-{1} 断开", ip, port));
            
        }

        private static void server_SocketConnected(string sessionId, string ip, int port)
        {


                Console.WriteLine(String.Format("连接：{0}-{1} 成功", ip, port));
            
        }
    }
}
