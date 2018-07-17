using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServerSuperIO.Service;
using ServerSuperIO.Service.Connector;
using ServerSuperIO.Data;
using ServerSuperIO.Device;

namespace TestService
{
    public class Service : ServerSuperIO.Service.Service
    {
        private object _SyncObject = new object();
        private Dictionary<string, List<Tag>> _Cache = null;
        private TcpClient _tcpClient = null;
        private Thread _Thread = null;
        private int _SendBufferSize = 2048;
        private int _ReceiveBufferSize = 2048;
        private int _SendTimeout = 1000 * 60;
        private int _ReceiveTimeout = 1000 * 60;
        private byte[] _Buffer = null;
        private bool _IsRun = false;
        public Service()
        {
            _Buffer = new byte[_ReceiveBufferSize];
            _Cache = new Dictionary<string, List<Tag>>();
        }
        public override string ServiceKey
        {
            get { return "TestService"; }
        }

        public override string ServiceName
        {
            get { return "测试服务"; }
        }

        public override void OnClick()
        {
            ConfigForm f1 = new ConfigForm();
            f1.ShowDialog();
        }

        //public override void UpdateDevice(string devCode, object obj)
        //{
        //    lock (_SyncObject)
        //    {
        //        if (obj != null)
        //        {
        //            if (obj is List<Tag>)
        //            {
        //                List<Tag> arr = (List<Tag>)obj;
        //                //OnServiceLog(String.Format("服务：{0} 接收到'{1}'的数据>>{2},{3}", ServiceName, arr[1], arr[2], arr[3]));
        //                //if (arr.Length >= 2)
        //                //{
        //                    if (this._Cache.ContainsKey(devCode)) //判断ID
        //                    {
        //                        this._Cache[devCode] = arr;
        //                    }
        //                    else
        //                    {
        //                        this._Cache.Add(devCode, arr);
        //                    }
        //                //}
        //            }
        //        }
        //    }
        //}

        //public override void RemoveDevice(string devCode)
        //{
        //    this._Cache.Remove(devCode);
        //    OnServiceLog("删除数据");
        //}

        public override void StartService()
        {
            if ((_Thread == null || !_Thread.IsAlive) && !this._IsRun)
            {
                OnServiceLog("启动服务");
                this._IsRun = true;
                this._Thread = new Thread(new ThreadStart(Target_Service));
                this._Thread.IsBackground = true;
                this._Thread.Name = "ClientServiceThread";
                this._Thread.Start();
            }
        }

        public override void StopService()
        {
            OnServiceLog("停止服务");
            if ((_Thread != null && _Thread.IsAlive) && this._IsRun)
            {
                this._IsRun = false;

                this._Thread.Join(1000);

                try
                {
                    this._Thread.Abort();
                }
                catch
                {

                }

                this._Thread = null;
            }

            if (_Cache != null)
            {
                _Cache.Clear();
                _Cache = null;
            }

        }

        public override void Dispose()
        {
            OnServiceLog("释放资源");
        }

        public override void ServiceConnectorCallback(object obj)
        {
            OnServiceLog(obj.ToString());
            OnServiceLog("设备已经处理完成指令");
        }

        public override void ServiceConnectorCallbackError(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        private void Target_Service()
        {
            while (_IsRun)
            {
                try
                {
                    if (_tcpClient != null)
                    {
                        lock (_SyncObject)
                        {
                            string content = String.Empty;
                            foreach (KeyValuePair<string, List<Tag>> kv in _Cache)
                            {
                                foreach (Tag tag in kv.Value)
                                {
                                    content += tag.TagName+":"+tag.TagValue+";";
                                }

                                content += Environment.NewLine;
                            }

                            if (!String.IsNullOrEmpty(content))
                            {
                                byte[] data = System.Text.Encoding.ASCII.GetBytes(content);
                                this.OnSend(data);
                            }
                        }
                    }
                    else
                    {
                        this.ConnectServer();
                    }
                }
                catch (SocketException ex)
                {
                    this.CloseSocket();
                    OnServiceLog(ex.Message);
                }
                catch (Exception ex)
                {
                    OnServiceLog(ex.Message);
                }
                finally
                {
                    System.Threading.Thread.Sleep(2000);
                }
            }
        }

        private void CloseSocket()
        {
            if (_tcpClient != null)
            {
                lock (_tcpClient)
                {
                    try
                    {
                        _tcpClient.Client.Shutdown(SocketShutdown.Both);
                    }
                    catch { }

                    try
                    {
                        _tcpClient.Client.Close();
                        _tcpClient.Close();
                    }
                    catch { }

                    _tcpClient = null;
                }
            }
        }

        private void ConnectServer()
        {
            _tcpClient=new TcpClient();
            _tcpClient.Connect("127.0.0.1", 7001);
            _tcpClient.Client.SendBufferSize = _SendBufferSize;
            _tcpClient.Client.ReceiveBufferSize = _ReceiveBufferSize;
            _tcpClient.Client.ReceiveTimeout = _ReceiveTimeout;
            _tcpClient.Client.SendTimeout = _SendTimeout;

            this.OnServiceLog("连接服务器，启动接收服务");

            this.OnReceive();
        }

        private void OnReceive()
        {
            _tcpClient.Client.BeginReceive(_Buffer, 0, _Buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), _tcpClient);
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            TcpClient socket = (TcpClient)ar.AsyncState;
            try
            {
                if (socket != null)
                {
                    int read = socket.Client.EndReceive(ar);
                    if (read > 0)
                    {
                        //处理数据.....................通知设备
                        string text = System.Text.Encoding.ASCII.GetString(_Buffer, 0, read);
                        OnServiceConnector(new FromService(this.ServiceName,this.ServiceKey,this),new ServiceToDevice("1",text,null,null) );

                        OnReceive();
                    }
                    else
                    {
                        this.CloseSocket();
                    }
                }
            }
            catch (SocketException ex)
            {
                this.CloseSocket();
                this.OnServiceLog(ex.Message);
            }
            catch (Exception ex)
            {
                this.OnServiceLog(ex.Message);
            }
        }

        private void OnSend(byte[] data)
        {
            if (_tcpClient != null)
            {
                if (_tcpClient.Client.Send(data, 0, data.Length, SocketFlags.None) == data.Length)
                {
                    this.OnServiceLog("数据上传成功");
                }
            }
        }

        private void DealData(string deviceCode,object deviceData)
        {
            lock (_SyncObject)
            {
                string devCode = deviceCode;
                object obj = deviceData;
                if (obj != null)
                {
                    if (obj is List<Tag>)
                    {
                        List<Tag> arr = (List<Tag>)obj;
                        //OnServiceLog(String.Format("服务：{0} 接收到'{1}'的数据>>{2},{3}", ServiceName, arr[1], arr[2], arr[3]));
                        //if (arr.Length >= 2)
                        //{
                        if (this._Cache.ContainsKey(devCode)) //判断ID
                        {
                            this._Cache[devCode] = arr;
                        }
                        else
                        {
                            this._Cache.Add(devCode, arr);
                        }
                        //}
                    }
                }
            }
        }

        public override void InternalServerSubscriber(InternalServerPublisherArgs args)
        {
            DealData(args.DeviceCode, args.Object);
        }

        public override void GlobalServerSubscriber(GlobalServerPublisherArgs args)
        {
            DealData(args.DeviceCode, args.Object);
        }

        public override void TagValueSubscriber(TagValuePublishedArgs args)
        {
            OnServiceLog(args.TagName + "," + args.NewValue.ToString());
        }
    }
}
