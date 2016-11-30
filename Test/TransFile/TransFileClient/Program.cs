using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ServerSuperIO.Common;

namespace TransFileClient
{
    class Program
    {
        private static TcpClient _tcpClient = null;
        private static string _ip = "127.0.0.1";
        private static int _port = 6699;
        private static string _file = "transfile.txt";
        private static int _sendBufferSize = 1024;
        static void Main(string[] args)
        {
            while (true)
            {
                try
                {
                    bool connected = Connect();
                    if (connected)
                    {
                        Console.WriteLine("连接服务器成功");

                        SendFile();
                    }
                    else
                    {
                        Console.WriteLine("连接服务器失败");
                    }
                }
                catch (SocketException ex)
                {
                    Close();
                    Console.WriteLine(ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                System.Threading.Thread.Sleep(5000);
            }
        }

        static bool Connect()
        {
            if (_tcpClient == null)
            {
                _tcpClient = new TcpClient();
            }
            if (!_tcpClient.Connected)
            {
                _tcpClient.Connect(_ip, _port);
            }
            return _tcpClient.Connected;
        }

        static void Close()
        {
            if (_tcpClient != null)
            {
                _tcpClient.Close();
                _tcpClient = null;
            }
        }

        static void SendFile()
        {
            if (!System.IO.File.Exists(_file))
            {
                Console.WriteLine("文件不存在:"+_file);
                return;
            }

            FileStream fs = null;
            try
            {
                Console.WriteLine("开始传输>>");

                string fileName=DateTime.Now.ToString("yyMMddHHmmss") + ".txt";
                int bufferSize = _sendBufferSize;
                byte[] sendBuffer = new byte[bufferSize];
                fs = new FileStream(_file, FileMode.Open,FileAccess.Read,FileShare.Read);

                long length = fs.Length;
                int count = 0;
                Stopwatch watch = new Stopwatch();
                watch.Start();
                while (length > 0)
                {
                    int sendNum = fs.Read(sendBuffer, 0, sendBuffer.Length);

                    byte[] package = GetDataPackage(fileName,sendBuffer, sendNum);

                    count+=_tcpClient.Client.Send(package, 0, package.Length, SocketFlags.None);

                    length -= sendNum;

                    float percent = ((fs.Length - length)/(float) fs.Length)*100.0f;
                    Console.WriteLine("已传:" + percent.ToString("0.00")  + "%");
                    System.Threading.Thread.Sleep(5);
                }
                watch.Stop();
                
                Console.WriteLine("传输完毕!总数:" + count.ToString()+",耗时:"+ watch.Elapsed.TotalSeconds.ToString(CultureInfo.InvariantCulture));
            }
            catch
            {
                throw;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
            }
        }

        static byte[] GetDataPackage(string fileName,byte[] sendBuffer, int sendNum)
        {
            byte[] sendPackage = new byte[sendNum + 24];
            sendPackage[0] = 0x35;
            sendPackage[1] = 0x35;

            string code = "0001";
            byte[] codeBytes = System.Text.Encoding.ASCII.GetBytes(code);
            Buffer.BlockCopy(codeBytes, 0, sendPackage, 2, 4);

            byte[] fileBytes= System.Text.Encoding.ASCII.GetBytes(fileName);
            Buffer.BlockCopy(fileBytes, 0, sendPackage, 6, 16);

            Buffer.BlockCopy(sendBuffer, 0, sendPackage, 22, sendNum);

            sendPackage[sendPackage.Length - 2] = 0x33;
            sendPackage[sendPackage.Length - 1] = 0x33;

            return sendPackage;
        }
    }
}
