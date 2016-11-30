using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDeviceDriver
{
    [Serializable]
    public class Dyn
    {
        private DateTime _CurDT = DateTime.Now;

        public DateTime CurDT
        {
            get { return _CurDT; }
            set { _CurDT = value; }
        }

        private byte[] _ProHead = new byte[] { };
        /// <summary>
        /// 协议头
        /// </summary>
        public byte[] ProHead
        {
            get { return _ProHead; }
            set { _ProHead = value; }
        }

        private int _DeviceAddr = -1;
        /// <summary>
        /// 解析的地址
        /// </summary>
        public int DeviceAddr
        {
            get { return _DeviceAddr; }
            set { _DeviceAddr = value; }
        }

        private byte[] _Command = new byte[] { };
        /// <summary>
        /// 协议命令
        /// </summary>
        public byte[] Command
        {
            get { return _Command; }
            set { _Command = value; }
        }

        private byte[] _ProEnd = new byte[] { };
        /// <summary>
        /// 协议结束
        /// </summary>
        public byte[] ProEnd
        {
            get { return _ProEnd; }
            set { _ProEnd = value; }
        }

        private object _State = null;
        /// <summary>
        /// 状态
        /// </summary>
        public object State
        {
            get { return _State; }
            set { _State = value; }
        }

        private float _Flow = 0.0f;
        /// <summary>
        /// 流量
        /// </summary>
        public float Flow
        {
            get { return _Flow; }
            set { _Flow = value; }
        }
        private float _Signal = 0.0f;
        /// <summary>
        /// 信号
        /// </summary>
        public float Signal
        {
            get { return _Signal; }
            set { _Signal = value; }
        }
    }
}
