using ServerSuperIO.Communicate;
using ServerSuperIO.Persistence;

namespace ServerSuperIO.Device
{
    public interface IDeviceDynamic : IXmlPersistence
    {
        /// <summary>
        /// 设备ID
        /// </summary>
        string DeviceID { get;set;}

        /// <summary>
        /// 当前备注说明
        /// </summary>
        string Remark { get;set;}

        /// <summary>
        /// 设备运行状态
        /// </summary>
        RunState RunState { get;set;}

        /// <summary>
        /// 设备的通讯状态
        /// </summary>
        CommunicateState CommunicateState { get; set; }

        /// <summary>
        /// IO状态
        /// </summary>
        ChannelState ChannelState { get; set; }

        /// <summary>
        /// 获得报警状态描述
        /// </summary>
        /// <returns></returns>
        string GetAlertState();
    }
}
