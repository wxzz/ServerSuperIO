using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServerSuperIO.Common;
using ServerSuperIO.Device;
using ServerSuperIO.Protocol;
using ServerSuperIO.Protocol.Filter;
using ServerSuperIO.Server;

namespace ServerSuperIO.Communicate.NET
{
    public class TcpSocketSession : SocketSession
    {
        /// <summary>
        /// 设置多长时间后检测网络状态
        /// </summary>
        private byte[] _KeepAliveOptionValues;

        /// <summary>
        /// 设置检测网络状态间隔时间
        /// </summary>
        private byte[] _KeepAliveOptionOutValues;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="remoteEndPoint"></param>
        /// <param name="proxy"></param>
        public TcpSocketSession(Socket socket, IPEndPoint remoteEndPoint, ISocketAsyncEventArgsProxy proxy)
            : base(socket, remoteEndPoint, proxy)
        {
        }

        public override void Initialize()
        {
            if (Client != null)
            {
                //-------------------初始化心跳检测---------------------//
                uint dummy = 0;
                _KeepAliveOptionValues = new byte[Marshal.SizeOf(dummy) * 3];
                _KeepAliveOptionOutValues = new byte[_KeepAliveOptionValues.Length];
                BitConverter.GetBytes((uint)1).CopyTo(_KeepAliveOptionValues, 0);
                BitConverter.GetBytes((uint)(2000)).CopyTo(_KeepAliveOptionValues, Marshal.SizeOf(dummy));

                uint keepAlive = this.Server.ServerConfig.KeepAlive;

                BitConverter.GetBytes((uint)(keepAlive)).CopyTo(_KeepAliveOptionValues, Marshal.SizeOf(dummy) * 2);

                Client.IOControl(IOControlCode.KeepAliveValues, _KeepAliveOptionValues, _KeepAliveOptionOutValues);

                Client.NoDelay = true;
                Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
                //----------------------------------------------------//

                Client.ReceiveTimeout = Server.ServerConfig.NetReceiveTimeout;
                Client.SendTimeout = Server.ServerConfig.NetSendTimeout;
                Client.ReceiveBufferSize = Server.ServerConfig.NetReceiveBufferSize;
                Client.SendBufferSize = Server.ServerConfig.NetSendBufferSize;
            }

            if (SocketAsyncProxy != null)
            {
                SocketAsyncProxy.Initialize(this);
                SocketAsyncProxy.SocketReceiveEventArgsEx.Completed += SocketEventArgs_Completed;
                SocketAsyncProxy.SocketSendEventArgs.Completed += SocketEventArgs_Completed;
            }
        }

        /// <summary>
        /// 异步读取固定长度数据
        /// </summary>
        /// <param name="dataLength"></param>
        /// <param name="cts"></param>
        /// <returns></returns>
        protected override Task<byte[]> ReadAsync(int dataLength, CancellationTokenSource cts)
        {
            Task<byte[]> t = Task.Factory.StartNew(() =>
            {
                int readLength = dataLength;
                List<byte> readBytes = new List<byte>(dataLength);
                while (readLength > 0)
                {
                    if (cts.IsCancellationRequested)
                    {
                        break;
                    }

                    SocketAsyncEventArgsEx saeaEx = SocketAsyncProxy.SocketReceiveEventArgsEx;

                    int oneLength = dataLength <= this.Client.ReceiveBufferSize ? dataLength : this.Client.ReceiveBufferSize;

                    try
                    {
                        if (!this.IsDisposed && this.Client != null)
                        {
                            oneLength = this.Client.Receive(saeaEx.ReceiveBuffer, saeaEx.InitOffset, oneLength,SocketFlags.None);
                        }
                        else
                        {
                            break;
                        }
                    }
                    catch (SocketException ex)
                    {
                        OnCloseSocket();
                        this.Server.Logger.Error(true, "", ex);
                        break;
                    }

                    saeaEx.DataLength += oneLength;

                    byte[] data = saeaEx.Get();

                    readBytes.AddRange(data);

                    readLength -= oneLength;
                }
                return readBytes.ToArray();
            }, cts.Token);
            return t;
        }

        public override IList<byte[]> Read(IReceiveFilter receiveFilter)
        {
            if (!this.IsDisposed)
            {
                System.Threading.Thread.Sleep(Server.ServerConfig.NetLoopInterval);
                if (this.Client != null &&
                    this.Client.Connected)
                {
                    if (this.Client.Poll(10, SelectMode.SelectRead))
                    {
                        try
                        {
                            SocketAsyncEventArgsEx saeaEx = SocketAsyncProxy.SocketReceiveEventArgsEx;
                            if (saeaEx.NextOffset >= saeaEx.InitOffset + saeaEx.Capacity)
                            {
                                saeaEx.Reset();
                            }

                            #region
                            int num = this.Client.Receive(saeaEx.ReceiveBuffer, saeaEx.NextOffset, saeaEx.InitOffset + saeaEx.Capacity - saeaEx.NextOffset, SocketFlags.None);

                            if (num <= 0)
                            {
                                throw new SocketException((int)SocketError.HostDown);
                            }
                            else
                            {
                                LastActiveTime = DateTime.Now;

                                saeaEx.DataLength += num;
                                if (receiveFilter == null)
                                {
                                    IList<byte[]> listBytes = new List<byte[]>();
                                    listBytes.Add(saeaEx.Get());
                                    return listBytes;
                                }
                                else
                                {
                                    return saeaEx.Get(receiveFilter);
                                }
                            }
                            #endregion
                        }
                        catch (SocketException)
                        {
                            OnCloseSocket();
                            throw new SocketException((int)SocketError.HostDown);
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    OnCloseSocket();
                    throw new SocketException((int)SocketError.HostDown);
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 写操作
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override int Write(byte[] data)
        {
            if (!this.IsDisposed)
            {
                if (this.Client.Connected)
                {
                    try
                    {
                        int successNum = 0;
                        int num = 0;
                        while (num < data.Length)
                        {
                            int remainLength = data.Length - num;
                            int sendLength = remainLength >= this.Client.SendBufferSize ? this.Client.SendBufferSize : remainLength;

                            SocketError error;
                            successNum += this.Client.Send(data, num, sendLength, SocketFlags.None, out error);

                            num += sendLength;

                            if (successNum <= 0 || error != SocketError.Success)
                            {
                                throw new SocketException((int)SocketError.HostDown);
                            }
                        }

                        return successNum;
                    }
                    catch (SocketException)
                    {
                        OnCloseSocket();
                        throw;
                    }
                }
                else
                {
                    OnCloseSocket();
                    throw new SocketException((int)SocketError.HostDown);
                }
            }
            else
            {
                return 0;
            }
        }

        public override void TryReceive()
        {
            if (Client != null)
            {
                try
                {
                    bool willRaiseEvent = this.Client.ReceiveAsync(this.SocketAsyncProxy.SocketReceiveEventArgsEx);
                    if (!willRaiseEvent)
                    {
                        ProcessReceive(this.SocketAsyncProxy.SocketReceiveEventArgsEx);
                    }
                }
                catch (Exception ex)
                {
                    this.Server.Logger.Error(true, ex.Message);
                }
            }
        }

        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            ISocketSession socketSession = (ISocketSession)e.UserToken;
            if (socketSession != null && socketSession.Client != null)
            {
                try
                {
                    if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
                    {
                        SocketAsyncEventArgsEx saeaEx = (SocketAsyncEventArgsEx)e;
                        if (saeaEx.NextOffset >= saeaEx.InitOffset + saeaEx.Capacity)
                        {
                            saeaEx.Reset();
                        }

                        saeaEx.DataLength += saeaEx.BytesTransferred;

                        IList<byte[]> listBytes = InternalReceivePackage(saeaEx);

                        IList<IRequestInfo> listRequestInfos = RequestInfo.ConvertBytesToRequestInfos(Key, Channel,
                            listBytes);

                        if (this.Server.ServerConfig.StartCheckPackageLength)
                        {
                            InternalReceivePackageData(listRequestInfos);
                        }

                        OnSocketReceiveData(new ReceivePackage(RemoteIP, RemotePort, listRequestInfos)); //没有经过检测数据包长度

                        saeaEx.SetBuffer(saeaEx.ReceiveBuffer, saeaEx.NextOffset, saeaEx.InitOffset + saeaEx.Capacity - saeaEx.NextOffset);

                        bool willRaiseEvent = socketSession.Client.ReceiveAsync(this.SocketAsyncProxy.SocketReceiveEventArgsEx);
                        if (!willRaiseEvent)
                        {
                            ProcessReceive(saeaEx);
                        }
                    }
                    else
                    {
                        OnCloseSocket();
                    }
                }
                catch (SocketException ex)
                {
                    OnCloseSocket();
                    this.Server.Logger.Error(true, ex.Message);
                }
                catch (Exception ex)
                {
                    this.Server.Logger.Error(true, ex.Message);
                }
            }
        }

        private IRunDevice InternalCheckCodeDevice(byte[] data)
        {
            IRunDevice dev = null;
            IRunDevice[] devList = this.Server.DeviceManager.GetDevices(CommunicateType.NET);
            if (devList != null && devList.Length > 0)
            {
                if (this.Server.ServerConfig.ControlMode == ControlMode.Loop
                    || this.Server.ServerConfig.ControlMode == ControlMode.Self
                    || this.Server.ServerConfig.ControlMode == ControlMode.Parallel)
                {
                    try
                    {
                        dev = devList.FirstOrDefault(d => d.DeviceParameter.DeviceCode == d.Protocol.GetCode(data));
                    }
                    catch (Exception ex)
                    {
                        this.Server.Logger.Error(true, ex.Message);
                    }
                }
                else if (this.Server.ServerConfig.ControlMode == ControlMode.Singleton)
                {
                    dev = devList[0];
                }
            }
            return dev;
        }

        private IList<byte[]> InternalReceivePackage(SocketAsyncEventArgsEx saeaEx)
        {
            IList<byte[]> listBytes;
            if (this.Server.ServerConfig.StartReceiveDataFliter)
            {
                #region
                byte[] data = new byte[saeaEx.DataLength];
                Buffer.BlockCopy(saeaEx.ReceiveBuffer, saeaEx.InitOffset, data, 0, data.Length);

                IRunDevice dev = InternalCheckCodeDevice(data);

                if (dev != null)
                {
                    listBytes = InternalFliterData(saeaEx, dev);
                }
                else
                {
                    listBytes = InternalFliterData(saeaEx, null);
                }
                #endregion
            }
            else
            {
                listBytes = InternalFliterData(saeaEx, null);
            }
            return listBytes;
        }

        private IList<byte[]> InternalFliterData(SocketAsyncEventArgsEx saeaEx, IRunDevice dev)
        {
            if (dev != null
                && dev.Protocol != null
                && dev.Protocol.ReceiveFilter != null)
            {
                IList<byte[]> listBytes = saeaEx.Get(dev.Protocol.ReceiveFilter);
                if (listBytes != null && listBytes.Count > 0)
                {
                    LastActiveTime = DateTime.Now;
                }
                return listBytes;
            }
            else
            {
                IList<byte[]> listBytes = new List<byte[]>();
                byte[] data = saeaEx.Get();
                if (data.Length > 0)
                {
                    LastActiveTime = DateTime.Now;
                    listBytes.Add(data);
                }
                return listBytes;
            }
        }

        private void InternalReceivePackageData(IList<IRequestInfo> listRequestInfos)
        {
            if (listRequestInfos.Count > 0)
            {
                IRunDevice dev = InternalCheckCodeDevice(listRequestInfos[0].Data);
                if (dev != null)
                {
                    ((RunDevice)dev).InternalReceiveChannelPackageData(this.Channel, listRequestInfos);
                }
            }
        }

        private void ProcessSend(SocketAsyncEventArgs e)
        {
            try
            {
                if (e.SocketError == SocketError.Success)
                {
                    if (e.UserToken == null) return;

                    byte[] data = (byte[])e.UserToken;
                    
                    if (e.BytesTransferred < data.Length)
                    {
                        e.SetBuffer(data, e.BytesTransferred, data.Length - e.BytesTransferred);
                        bool willRaiseEvent = this.Client.SendAsync(e);
                        if (!willRaiseEvent)
                        {
                            ProcessSend(e);
                        }
                    }
                    else
                    {
                        e.UserToken = null;
                    }
                }
                else
                {
                    OnCloseSocket();
                }
            }
            catch (SocketException ex)
            {
                OnCloseSocket();
                this.Server.Logger.Error(true, ex.Message);
            }
            catch (Exception ex)
            {
                this.Server.Logger.Error(true, ex.Message);
            }
        }

        protected override void SendAsync(byte[] data)
        {
            if (Client != null)
            {
                try
                {
                    this.SocketAsyncProxy.SocketSendEventArgs.UserToken = data;
                    this.SocketAsyncProxy.SocketSendEventArgs.SetBuffer(data, 0, data.Length);
                    bool willRaiseEvent = this.Client.SendAsync(this.SocketAsyncProxy.SocketSendEventArgs);

                    if (!willRaiseEvent)
                    {
                        ProcessSend(this.SocketAsyncProxy.SocketSendEventArgs);
                    }
                }
                catch (Exception ex)
                {
                    this.Server.Logger.Error(true, ex.Message);
                }
            }
        }

        protected override void SendSync(byte[] data)
        {
            if (Client != null)
            {
                try
                {
                    this.Client.SendData(data);
                }
                catch (SocketException ex)
                {
                    OnCloseSocket();
                    this.Server.Logger.Error(true, ex.Message);
                }
                catch (Exception ex)
                {
                    this.Server.Logger.Error(true, ex.Message);
                }
            }
        }

        protected override void SocketEventArgs_Completed(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    ProcessReceive(e);
                    break;
                case SocketAsyncOperation.Send:
                    ProcessSend(e);
                    break;
                default:
                    this.Server.Logger.Info(false, "不支持接收和发送的操作");
                    break;
            }
        }
    }
}
