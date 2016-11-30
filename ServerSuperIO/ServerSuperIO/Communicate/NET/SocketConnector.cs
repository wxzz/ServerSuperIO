using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ServerSuperIO.Common;
using ServerSuperIO.Device;
using ServerSuperIO.Server;

namespace ServerSuperIO.Communicate.NET
{
    internal class SocketConnector : ServerProvider, ISocketConnector
    {
        private bool _IsDisposed = false;
        private bool _IsExited = false;
        private Thread _Thread = null;

        public SocketConnector()
        {
        }

        ~SocketConnector()
        {
            Dispose(false);
        }

        public void Start()
        {
            if (_Thread == null || !_Thread.IsAlive)
            {
                this._Thread = new Thread(new ThreadStart(RunConnector))
                {
                    IsBackground = true,
                    Name = "NetConnectorThread"
                };
                this._Thread.Start();
            }
        }

        public void Stop()
        {
            Dispose(true);
        }

        public event NewClientAcceptHandler NewClientConnected;

        private void OnNewClientConnected(Socket socket, object state)
        {
            if (NewClientConnected != null)
                NewClientConnected.BeginInvoke(this, socket, state, null, null);
        }

        public event ErrorHandler Error;

        private void OnError(Exception e)
        {
            if (Error != null)
                Error(this, e);
        }

        private void OnError(string errorMessage)
        {
            OnError(new Exception(errorMessage));
        }

        private void RunConnector()
        {
            while (!_IsExited)
            {
                IRunDevice[] devList = this.Server.DeviceManager.GetDevices(WorkMode.TcpClient);
                if (devList.Length > 0)
                {
                    #region
                    for (int i = 0; i < devList.Length; i++)
                    {
                        IRunDevice dev = devList[i];
                        IChannel channel = this.Server.ChannelManager.GetChannel(dev.DeviceParameter.NET.RemoteIP,CommunicateType.NET);
                        if (channel == null)
                        {
                            StartConnect(dev.DeviceParameter.NET.RemoteIP, dev.DeviceParameter.NET.RemotePort);
                        }
                    }
                    #endregion

                }
                System.Threading.Thread.Sleep(1000);
            }
        }

        private void StartConnect(string remoteIP, int remotePort)
        {
            SocketAsyncEventArgs connectEventArgs = new SocketAsyncEventArgs();
            connectEventArgs.AcceptSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            connectEventArgs.RemoteEndPoint = new IPEndPoint(IPAddress.Parse(remoteIP), remotePort);
            connectEventArgs.Completed += ConnectSocketAsyncEventArgs_Completed;
            bool willRaiseEvent = connectEventArgs.AcceptSocket.ConnectAsync(connectEventArgs);
            if (!willRaiseEvent)
            {
                ProcessConnect(connectEventArgs);
            }
        }

        private void ConnectSocketAsyncEventArgs_Completed(object sender, SocketAsyncEventArgs e)
        {
            if (e.LastOperation == SocketAsyncOperation.Connect)
            {
                ProcessConnect(e);
            }
        }

        private void ProcessConnect(SocketAsyncEventArgs connectEventArgs)
        {
            if (connectEventArgs.SocketError == SocketError.Success)
            {
                OnNewClientConnected(connectEventArgs.AcceptSocket, null);
            }
            else
            {
                ProcessConnectionError(connectEventArgs);
            }
        }

        private void ProcessConnectionError(SocketAsyncEventArgs connectEventArgs)
        {
            connectEventArgs.AcceptSocket.SafeClose();
            connectEventArgs.Dispose();
            connectEventArgs = null;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_IsDisposed)
            {
                if (disposing)
                {

                }

                if (this._Thread != null && this._Thread.IsAlive)
                {
                    this._IsExited = true;
                    this._Thread.Join(1000);
                    if (this._Thread.IsAlive)
                    {
                        try
                        {
                            _Thread.Abort();
                        }
                        catch
                        {

                        }
                    }
                }

                _IsDisposed = true;
            }
        }
    }
}
