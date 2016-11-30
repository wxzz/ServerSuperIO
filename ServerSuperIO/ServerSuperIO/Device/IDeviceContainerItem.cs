using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerSuperIO.Communicate;

namespace ServerSuperIO.Device
{
    public interface IDeviceContainerItem
    {
        string DeviceID { set; get; }

        string DeviceCode { set; get; }

        int DeviceAddr { set; get; }

        string DeviceName { set; get; }

        CommunicateType CommunicateType { set; get; }

        string IoParameter1 { set; get; }

        int IoParameter2 { set; get; }

        ChannelState ChannelState { set; get; }

        CommunicateState CommunicateState { set; get; }

        RunState RunState { set; get; }

        string AlertState { set; get; }

        DeviceType DeviceType { set; get; }

        string ModelNumber { set; get; }
    }
}
