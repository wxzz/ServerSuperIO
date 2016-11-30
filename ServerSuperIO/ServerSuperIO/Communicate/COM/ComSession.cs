using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServerSuperIO.DataCache;
using ServerSuperIO.Protocol;
using ServerSuperIO.Protocol.Filter;
using ServerSuperIO.Server;

namespace ServerSuperIO.Communicate.COM
{
    internal class ComSession : BaseChannel, IComSession
    {
        private SerialPort _sp = null;
        private bool _IsDisposed = false;
        //private byte[] _ReadBuffer = null;
        private IReceiveCache _ReceiveCache = null;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="com"></param>
        /// <param name="baud"></param>
        public ComSession(int com, int baud)
            : base()
        {
            SessionID = Guid.NewGuid().ToString();
            _sp = new SerialPort
            {
                PortName = ComUtils.PortToString(com),
                BaudRate = baud,

                DataBits = 8,
                StopBits = StopBits.One,
                Parity = Parity.None
            };
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="com"></param>
        /// <param name="baud"></param>
        /// <param name="databits"></param>
        /// <param name="stopbits"></param>
        /// <param name="parity"></param>
        public ComSession(int com, int baud, int databits, StopBits stopbits, Parity parity) : base()
        {
            SessionID = Guid.NewGuid().ToString();
            _sp = new SerialPort
            {
                PortName = ComUtils.PortToString(com),
                BaudRate = baud,
                DataBits = databits,
                StopBits = stopbits,
                Parity = parity
            };
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~ComSession()
        {
            Dispose(false);
        }

        public override void Initialize()
        {
            _sp.ReadBufferSize = this.Server.ServerConfig.ComReadBufferSize;
            _sp.ReadTimeout = this.Server.ServerConfig.ComReadTimeout;
            _sp.WriteBufferSize = this.Server.ServerConfig.ComWriteBufferSize;
            _sp.WriteTimeout = this.Server.ServerConfig.ComWriteTimeout;
            //_ReadBuffer = new byte[this.Server.Config.ComReadBufferSize];
            _ReceiveCache = new ReceiveCache(this.Server.ServerConfig.ComReadBufferSize);
        }

        /// <summary>
        /// 是否打开串口
        /// </summary>
        public bool IsOpen
        {
            get { return this._sp.IsOpen; }
        }

        /// <summary>
        /// 串口号
        /// </summary>
        public int Port
        {
            get { return ComUtils.PortToInt(this._sp.PortName); }
        }

        /// <summary>
        /// 波特率
        /// </summary>
        public int Baud
        {
            get { return this._sp.BaudRate; }
        }

        /// <summary>
        /// 数据位
        /// </summary>
        public int DataBits
        {
            get { return this._sp.DataBits; }
        }

        /// <summary>
        /// 停止位
        /// </summary>
        public StopBits StopBits
        {
            get { return this._sp.StopBits; }
        }

        /// <summary>
        /// 校验位
        /// </summary>
        public Parity Parity
        {
            get { return this._sp.Parity; }
        }

        /// <summary>
        /// 打开串口
        /// </summary>
        public void Open()
        {
            try
            {
                if (this._sp.IsOpen)
                {
                    this._sp.Close();
                }

                this._sp.Open();
                this._sp.DtrEnable = true;
                this._sp.RtsEnable = true;
                this._sp.DiscardInBuffer();
                this._sp.DiscardOutBuffer();

                if (this.COMOpen != null)
                {
                    this.COMOpen(this, this.Port, this.Baud, this.IsOpen);
                }
            }
            catch (Exception ex)
            {
                if (COMError != null)
                {
                    COMError(this, this.Port, this.Baud, ex.Message);
                }
            }
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public override void Close()
        {
            Dispose(true);

            if (this.COMClose != null)
            {
                this.COMClose(this, this.Port, this.Baud, !this.IsOpen);
            }
        }

        /// <summary>
        /// 读数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int InternalRead(byte[] data)
        {
            if (this._sp.BytesToRead > 0)
            {
                return this._sp.Read(data, 0, data.Length);
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 读数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public int InternalRead(byte[] data, int offset, int length)
        {
            if (data == null)
            {
                throw new ArgumentNullException("缓存区为空");
            }

            if (offset > data.Length)
            {
                throw new ArgumentException("偏移量超出数组大小");
            }

            if (this._sp.BytesToRead > 0)
            {
                return this._sp.Read(data, offset, length);
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int InternalWrite(byte[] data)
        {
            return InternalWrite(data, 0, data.Length);
        }

        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public int InternalWrite(byte[] data, int offset, int length)
        {
            if (data == null)
            {
                throw new ArgumentNullException("数据源为空");
            }
           
            if (offset > data.Length)
            {
                throw new ArgumentException("偏移量超出数组大小");
            }
          
            this._sp.Write(data, offset, length);

            return length;
        }

        /// <summary>
        /// 清空缓存
        /// </summary>
        public void ClearBuffer()
        {
            if (_sp.IsOpen)
            {
                _sp.DiscardInBuffer();
                _sp.DiscardOutBuffer();
            }
        }

        /// <summary>
        /// 配置串口
        /// </summary>
        /// <param name="baud"></param>
        /// <returns></returns>
        public bool Settings(int baud)
        {
            this._sp.BaudRate = baud;
            return true;
        }

        /// <summary>
        /// 串口配置
        /// </summary>
        /// <param name="baud"></param>
        /// <param name="databits"></param>
        /// <param name="stopbits"></param>
        /// <param name="parity"></param>
        public bool Settings(int baud, int databits, StopBits stopbits, Parity parity)
        {
            this._sp.BaudRate = baud;
            this._sp.DataBits = databits;
            this._sp.StopBits = stopbits;
            this._sp.Parity = parity;
            return true;
        }

        /// <summary>
        /// 打开串口事件
        /// </summary>
        public event COMOpenHandler COMOpen;

        /// <summary>
        /// 关闭串口事件
        /// </summary>
        public event COMCloseHandler COMClose;

        /// <summary>
        /// 串口错误事件
        /// </summary>
        public event COMErrorHandler COMError;


        /// <summary>
        /// 串口号为关键字
        /// </summary>
        public override string Key
        {
            get { return this._sp.PortName; }
        }

        /// <summary>
        /// 唯一ID
        /// </summary>
        public override string SessionID { get; protected set; }

        /// <summary>
        /// IO通讯操作实例
        /// </summary>
        public override IChannel Channel
        {
            get { return (IChannel)this; }
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
                while (readLength>0)
                {
                    if (cts.IsCancellationRequested)
                    {
                        break;
                    }

                    int oneLength = dataLength <= this._sp.ReadBufferSize ? dataLength : this._sp.ReadBufferSize;
                    oneLength = InternalRead(_ReceiveCache.ReceiveBuffer, _ReceiveCache.InitOffset, oneLength);

                    _ReceiveCache.DataLength += oneLength;

                    byte[] data = _ReceiveCache.Get();

                    readBytes.AddRange(data);

                    readLength -= oneLength;
                }
                return readBytes.ToArray();
            }, cts.Token);
            return t;
        }

        public override IList<byte[]> Read(IReceiveFilter receiveFilter)
        {
            System.Threading.Thread.Sleep(Server.ServerConfig.ComLoopInterval);
            if (_ReceiveCache != null && _ReceiveCache.ReceiveBuffer != null)
            {
                if (_ReceiveCache.NextOffset >= _ReceiveCache.InitOffset + _ReceiveCache.Capacity)
                {
                    _ReceiveCache.Reset();
                }

                int num = InternalRead(_ReceiveCache.ReceiveBuffer, _ReceiveCache.NextOffset, _ReceiveCache.InitOffset + _ReceiveCache.Capacity - _ReceiveCache.NextOffset);

                if (num > 0)
                {
                    _ReceiveCache.DataLength += num;
                    if (receiveFilter == null)
                    {
                        IList<byte[]> listBytes = new List<byte[]>();
                        listBytes.Add(_ReceiveCache.Get());
                        return listBytes;
                    }
                    else
                    {
                        return _ReceiveCache.Get(receiveFilter);
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 写数据接口
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override int Write(byte[] data)
        {
            int sendBufferSize = this._sp.WriteBufferSize;
            if (data.Length <= sendBufferSize)
            {
                return this.InternalWrite(data);
            }
            else
            {
                int successNum = 0;
                int num = 0;
                while (num < data.Length)
                {
                    int remainLength = data.Length - num;
                    int sendLength = remainLength >= sendBufferSize
                        ? sendBufferSize
                        : remainLength;
                  
                    successNum += InternalWrite(data, num, sendLength);

                    num += sendLength;
                }
                return successNum;
            }
        }

        /// <summary>
        /// 负责通讯的类型
        /// </summary>
        public override CommunicateType CommunicationType
        {
            get { return CommunicateType.COM; }
        }

        /// <summary>
        /// 是否释放了资源
        /// </summary>
        public override bool IsDisposed
        {
            get { return _IsDisposed; }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (!_IsDisposed)
            {
                if (disposing)
                {
                    _sp.Close();
                    _sp.Dispose();
                }

                _IsDisposed = true;
            }
        }
    }
}
