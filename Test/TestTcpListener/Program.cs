using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServerSuperIO.Communicate;
using ServerSuperIO.Communicate.NET;
using ServerSuperIO.Config;
using ServerSuperIO.Server;

namespace TestTcpListener
{
    class Program
    {
        static IServer _server;
        private static int _Counter = 0;
        static void Main(string[] args)
        {
            _server=new ServerManager().CreateServer(new ServerConfig()
            {
                ServerName = "myserver",
                SocketMode = SocketMode.Tcp,
                ControlMode=ControlMode.Parallel,
                CheckSameSocketSession = false,
            });
            _server.SocketConnected += server_SocketConnected;
            _server.SocketClosed += server_SocketClosed;
            _server.Start();

            while (true)
            {
                string str=Console.ReadLine();

                if (str == "stop")
                {
                    _server.Stop();
                    break;
                }
            }

            Console.Read();
        }

        private static void server_SocketClosed(string serverName,string ip, int port)
        {
            lock (_server)
            {
                _Counter--;
                Console.WriteLine(String.Format("{0},连接：{1}-{2} 断开", _Counter, ip, port));
            }
        }

        private static void server_SocketConnected(string serverName, string ip, int port)
        {
            lock (_server)
            {
                _Counter++;
                Console.WriteLine(String.Format("{0},连接：{1}-{2} 成功", _Counter,ip, port));
            }
        }
    }
}
