using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using ServerSuperIO.Device;
using ServerSuperIO.Persistence;

namespace TestDeviceDriver
{
    [Serializable]
    public class DeviceDyn:DeviceDynamic
    {
        public DeviceDyn() : base()
        {
        }

        //public override string GetAlertState()
        //{
        //    throw new NotImplementedException("无报警信息");
        //}


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
