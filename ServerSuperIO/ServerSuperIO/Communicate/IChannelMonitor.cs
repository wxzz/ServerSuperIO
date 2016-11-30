using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerSuperIO.Communicate
{
    public interface IChannelMonitor
    {
        /// <summary>
        /// 数据监视器接口
        /// </summary>
        /// <param name="dataOrientation"></param>
        /// <param name="data"></param>
        void DataMonitor(DataOrientation dataOrientation, byte[] data);
    }
}
