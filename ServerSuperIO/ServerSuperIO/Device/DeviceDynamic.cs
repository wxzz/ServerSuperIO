using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerSuperIO.Common;
using ServerSuperIO.Communicate;
using ServerSuperIO.Persistence;

namespace ServerSuperIO.Device
{
    [Serializable]
    public abstract class DeviceDynamic:XmlPersistence,IDeviceDynamic
    {
        protected DeviceDynamic()
        {
            DeviceID = String.Empty;
            Remark = String.Empty;
            RunState=RunState.None;
            CommunicateState=CommunicateState.None;
        }

        /// <summary>
        /// 设备ID
        /// </summary>
        public string DeviceID { get; set; }

        /// <summary>
        /// 实时状态备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 运行状态
        /// </summary>
        public RunState RunState { get; set; }

        /// <summary>
        /// 通讯状态
        /// </summary>
        public CommunicateState CommunicateState { get; set; }

        /// <summary>
        /// IO状态
        /// </summary>
        public ChannelState ChannelState { get; set; }

        /// <summary>
        /// 获得报警状态
        /// </summary>
        /// <returns></returns>
        public abstract string GetAlertState();

        /// <summary>
        /// 保存路径
        /// </summary>
        public override string SavePath
        {
            get
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "ServerSuperIO/Dynamic/";
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                return String.Format("{0}/{1}.xml", path, this.DeviceID.ToString());
            }
        }
    }
}
